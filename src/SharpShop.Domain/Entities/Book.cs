using SharpShop.Domain.Exceptions;

namespace SharpShop.Domain.Entities;

public class Book
{
    public Guid Id { get; }
    public string Title { get; }
    public string Author { get; }
    public decimal UnitPrice { get; }

    public Book(string title, string author, decimal unitPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        ArgumentException.ThrowIfNullOrWhiteSpace(author, nameof(author));
        if (unitPrice <= 0)
            throw new UnitPriceMustBeLargerThanZero();

        Id = Guid.NewGuid();
        Title = title;
        Author = author;
        UnitPrice = unitPrice;
    }
}
