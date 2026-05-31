using FillGaps.SpeedOrders.Application.DTOs;
using FillGaps.SpeedOrders.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FillGaps.SpeedOrders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderAppService _orderAppService;
    private readonly IOrderQueries _orderQueries;

    public OrdersController(IOrderAppService orderAppService, IOrderQueries orderQueries)
    {
        _orderAppService = orderAppService;
        _orderQueries = orderQueries;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderInput input, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _orderAppService.CreateOrderAsync(input, cancellationToken);
            
            return Accepted(new { message = "Pedido recebido e em processamento.", data = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetCustomerOrders(Guid customerId, CancellationToken cancellationToken)
    {
        var orders = await _orderQueries.GetOrdersByCustomerAsync(customerId, cancellationToken);
        
        if (!orders.Any())
            return NotFound(new { message = "Nenhum pedido encontrado para este cliente." });

        return Ok(orders);
    }
}