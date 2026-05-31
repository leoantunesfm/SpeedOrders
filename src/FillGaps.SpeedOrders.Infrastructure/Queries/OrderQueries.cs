using System.Data;
using Dapper;
using FillGaps.SpeedOrders.Application.DTOs;
using FillGaps.SpeedOrders.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FillGaps.SpeedOrders.Infrastructure.Queries;

public class OrderQueries(IConfiguration configuration) : IOrderQueries
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") 
        ?? throw new ArgumentNullException("Connection string not found");

    public async Task<IEnumerable<OrderSummaryDto>> GetOrdersByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        const string sql = @"
            SELECT 
                Id AS OrderId, 
                CustomerId, 
                TotalAmount, 
                Status, 
                CreatedAt 
            FROM Orders WITH (NOLOCK)
            WHERE CustomerId = @CustomerId
            ORDER BY CreatedAt DESC";

        var command = new CommandDefinition(
            sql, 
            new { CustomerId = customerId }, 
            cancellationToken: cancellationToken);

        return await connection.QueryAsync<OrderSummaryDto>(command);
    }
}