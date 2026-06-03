using FillGaps.SpeedOrders.Application.Interfaces;
using FillGaps.SpeedOrders.Application.Services;
using FillGaps.SpeedOrders.Domain.Interfaces;
using FillGaps.SpeedOrders.Infrastructure.Data;
using FillGaps.SpeedOrders.Infrastructure.Messaging;
using FillGaps.SpeedOrders.Infrastructure.Queries;
using FillGaps.SpeedOrders.Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SpeedOrdersDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOrderQueries, OrderQueries>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton<IMessagePublisher, KafkaMessagePublisher>();

builder.Services.AddScoped<IOrderAppService, OrderAppService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "sql-server",
        tags: new[] { "ready" })
    .AddKafka(
        setup =>
        {
            setup.BootstrapServers = builder.Configuration["Kafka:BootstrapServers"];
            setup.MessageTimeoutMs = 5000;
        },
        name: "kafka-broker",
        tags: new[] { "ready" });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false 
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.ToString(),
            dependencies = report.Entries.Select(e => new
            {
                dependency = e.Key,
                status = e.Value.Status.ToString(),
                responseTime = e.Value.Duration.ToString()
            })
        });
        await context.Response.WriteAsync(result);
    }
});

app.Run();