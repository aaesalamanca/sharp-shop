namespace SharpShop.Domain.Exceptions;

public class UnitPriceMustBeLargerThanZero : DomainException
{
    public UnitPriceMustBeLargerThanZero()
        : base("Unit price must be larger than zero") { }

    public UnitPriceMustBeLargerThanZero(string message)
        : base(message) { }

    public UnitPriceMustBeLargerThanZero(string message, Exception innerException)
        : base(message, innerException) { }
}
