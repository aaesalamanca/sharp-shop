namespace SharpShop.Domain.Exceptions;

public class OrderMustHaveAtLeastOneBookException : DomainException
{
    public OrderMustHaveAtLeastOneBookException()
        : base("Order must have at least one book") { }

    public OrderMustHaveAtLeastOneBookException(string message)
        : base(message) { }

    public OrderMustHaveAtLeastOneBookException(string message, Exception innerException)
        : base(message, innerException) { }
}
