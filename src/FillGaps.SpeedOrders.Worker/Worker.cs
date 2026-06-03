using System.Text.Json;
using Confluent.Kafka;
using FillGaps.SpeedOrders.Application.Events;
using FillGaps.SpeedOrders.Domain.Entities;
using FillGaps.SpeedOrders.Domain.Interfaces;
using FillGaps.SpeedOrders.Worker.Services;
using Polly;
using Polly.Wrap;

namespace FillGaps.SpeedOrders.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly AsyncPolicyWrap _resiliencePipeline;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(2, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    var orderId = context.ContainsKey("OrderId") ? context["OrderId"] : "Desconhecido";
                    _logger.LogWarning("[Polly - Retry] Pedido {OrderId} | Tentativa {RetryCount} falhou. Aguardando {Seconds}s...", orderId, retryCount, timeSpan.TotalSeconds);
                });

        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(15),
                onBreak: (exception, timeSpan) => _logger.LogError("[Polly - Circuit Breaker] ABERTO! Serviço de pagamento fora do ar. Rejeitando requisições por 15 segundos."),
                onReset: () => _logger.LogInformation("[Polly - Circuit Breaker] FECHADO! O serviço voltou a responder."),
                onHalfOpen: () => _logger.LogWarning("[Polly - Circuit Breaker] MEIO-ABERTO! Testando se o serviço voltou..."));

        var fallbackPolicy = Policy
            .Handle<Exception>()
            .FallbackAsync(
                fallbackAction: async (context, cancellationToken) => 
                {
                    if (context.TryGetValue("OrderId", out var orderIdObj) && orderIdObj is Guid orderId)
                    {
                        _logger.LogInformation("[Polly - Fallback Action] Iniciando resgate. Atualizando banco de dados para o Pedido {OrderId}...", orderId);
                        
                        using var scope = _serviceProvider.CreateScope();
                        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);
                        
                        if (order != null)
                        {
                            order.UpdateStatus(OrderStatus.ManualInterventionRequired);
                            
                            orderRepository.Update(order);
                            await unitOfWork.CommitAsync(cancellationToken);
                            
                            _logger.LogInformation("[Polly - Fallback Action] SUCESSO! Pedido {OrderId} salvo como ManualInterventionRequired.", orderId);
                        }
                    }
                },
                onFallbackAsync: async (exception, context) =>
                {
                    var orderId = context.ContainsKey("OrderId") ? context["OrderId"] : "Desconhecido";
                    var reason = exception is Polly.CircuitBreaker.BrokenCircuitException 
                        ? "Disjuntor Aberto (Fail Fast)" 
                        : "Todas as tentativas esgotadas";

                    _logger.LogCritical("[Polly - Fallback] Desistindo temporariamente do Pedido {OrderId}. Motivo: {Reason}", orderId, reason);
                });

        _resiliencePipeline = fallbackPolicy.WrapAsync(circuitBreakerPolicy).WrapAsync(retryPolicy);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            GroupId = "speedorders-payment-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe("order-created-topic");

        _logger.LogInformation("Worker iniciado. Escutando o tópico 'order-created-topic'...");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(consumeResult.Message.Value);

                if (orderEvent != null)
                {
                    _logger.LogInformation("Nova mensagem capturada no Kafka: Pedido {OrderId}", orderEvent.OrderId);
                    await ProcessOrderWithResilienceAsync(orderEvent, stoppingToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            consumer.Close();
        }
    }

    private async Task ProcessOrderWithResilienceAsync(OrderCreatedEvent orderEvent, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var paymentGateway = scope.ServiceProvider.GetRequiredService<IExternalPaymentGateway>();

        var context = new Context { { "OrderId", orderEvent.OrderId } };

        await _resiliencePipeline.ExecuteAsync(async (ctx) =>
        {
            await paymentGateway.ProcessPaymentAsync(orderEvent.OrderId, orderEvent.TotalAmount, cancellationToken);
        }, context);
    }
}