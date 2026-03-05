namespace SharpShop.Domain.Exceptions;

public class CannotModifyConfirmedOrderException : DomainException
{
    public CannotModifyConfirmedOrderException()
        : base("Cannot modify a confirmed order") { }

    public CannotModifyConfirmedOrderException(string message)
        : base(message) { }

    public CannotModifyConfirmedOrderException(string message, Exception innerException)
        : base(message, innerException) { }
}
