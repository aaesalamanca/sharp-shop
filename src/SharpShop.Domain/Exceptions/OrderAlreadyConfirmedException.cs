namespace SharpShop.Domain.Exceptions;

public class OrderAlreadyConfirmedException : DomainException
{
    public OrderAlreadyConfirmedException()
        : base("Order is already confirmed") { }

    public OrderAlreadyConfirmedException(string message)
        : base(message) { }

    public OrderAlreadyConfirmedException(string message, Exception innerException)
        : base(message, innerException) { }
}
