using FillGaps.SpeedOrders.Application.Interfaces;
using FillGaps.SpeedOrders.Application.Services;
using FillGaps.SpeedOrders.Domain.Interfaces;
using FillGaps.SpeedOrders.Infrastructure.Data;
using FillGaps.SpeedOrders.Infrastructure.Messaging;
using FillGaps.SpeedOrders.Infrastructure.Queries;
using FillGaps.SpeedOrders.Infrastructure.Repositories;
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();