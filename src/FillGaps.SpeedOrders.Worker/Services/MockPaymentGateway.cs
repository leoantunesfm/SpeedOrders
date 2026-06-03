namespace FillGaps.SpeedOrders.Worker.Services;

public interface IExternalPaymentGateway
{
    Task ProcessPaymentAsync(Guid orderId, decimal amount, CancellationToken cancellationToken);
}

public class MockPaymentGateway : IExternalPaymentGateway
{
    private readonly ILogger<MockPaymentGateway> _logger;

    public MockPaymentGateway(ILogger<MockPaymentGateway> logger)
    {
        _logger = logger;
    }

    public async Task ProcessPaymentAsync(Guid orderId, decimal amount, CancellationToken cancellationToken)
    {
        _logger.LogInformation("--- [Gateway] Iniciando chamada na API de Pagamento para o pedido {OrderId} ---", orderId);
        
        await Task.Delay(500, cancellationToken);

        var random = new Random();
        if (random.Next(1, 100) <= 70)
        {
            _logger.LogWarning("--- [Gateway] FALHA! O serviço de pagamento retornou HTTP 500 (Internal Server Error) ---");
            throw new HttpRequestException("Serviço de pagamento indisponível.");
        }

        _logger.LogInformation("--- [Gateway] SUCESSO! Pagamento aprovado para o pedido {OrderId} ---", orderId);
    }
}