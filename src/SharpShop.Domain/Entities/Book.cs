using SharpShop.Domain.Exceptions;

namespace SharpShop.Domain.Entities;

public class Book
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }

    private Book() { }

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
