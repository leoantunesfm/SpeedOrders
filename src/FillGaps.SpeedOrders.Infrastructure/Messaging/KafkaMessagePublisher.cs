using Confluent.Kafka;
using System.Text.Json;
using FillGaps.SpeedOrders.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FillGaps.SpeedOrders.Infrastructure.Messaging;

public class KafkaMessagePublisher : IMessagePublisher
{
    private readonly IProducer<Null, string> _producer;

    public KafkaMessagePublisher(IConfiguration configuration)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] 
        };
        
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
    }

    public async Task PublishAsync<TEvent>(string topic, TEvent message, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(message);
        var kafkaMessage = new Message<Null, string> { Value = payload };

        await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);
    }
}