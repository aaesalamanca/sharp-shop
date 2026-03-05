namespace SharpShop.Domain.Exceptions;

public class OrderAlreadyShippedException : DomainException
{
    public OrderAlreadyShippedException()
        : base("Order is already shipped") { }

    public OrderAlreadyShippedException(string message)
        : base(message) { }

    public OrderAlreadyShippedException(string message, Exception innerException)
        : base(message, innerException) { }
}
