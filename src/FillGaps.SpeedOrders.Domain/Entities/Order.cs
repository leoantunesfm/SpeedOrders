namespace FillGaps.SpeedOrders.Domain.Entities;

public enum OrderStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    ManualInterventionRequired = 5
}

public class Order : Entity
{
    public Guid CustomerId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }

    public Order(Guid customerId, decimal totalAmount) : base() // Chama o construtor da base
    {
        CustomerId = customerId;
        TotalAmount = totalAmount;
        Status = OrderStatus.Pending;
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        Status = newStatus;
    }
}