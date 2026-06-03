using FillGaps.SpeedOrders.Domain.Interfaces;
using FillGaps.SpeedOrders.Infrastructure.Data;
using FillGaps.SpeedOrders.Infrastructure.Repositories;
using FillGaps.SpeedOrders.Worker;
using FillGaps.SpeedOrders.Worker.Services;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<SpeedOrdersDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IExternalPaymentGateway, MockPaymentGateway>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
