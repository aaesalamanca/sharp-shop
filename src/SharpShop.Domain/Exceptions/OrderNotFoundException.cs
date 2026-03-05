namespace SharpShop.Domain.Exceptions;

public class OrderNotFoundException : DomainException
{
    public OrderNotFoundException()
        : base("Order not found") { }

    public OrderNotFoundException(Guid orderId)
        : base($"Order with id {orderId} not found") { }

    public OrderNotFoundException(Guid orderId, Exception innerException)
        : base($"Order with id {orderId} not found", innerException) { }
}
