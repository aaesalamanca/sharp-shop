namespace SharpShop.Domain.Entities;

public class OrderItem
{
    public Book Book { get; }
    public int Quantity { get; private set; }

    public OrderItem(Book book)
    {
        ArgumentNullException.ThrowIfNull(book, nameof(book));

        Book = book;
        Quantity = 1;
    }

    public decimal CalculateTotal() => Book.UnitPrice * Quantity;

    public void AddOne() => Quantity += 1;

    public void DeleteOne() => Quantity -= 1;

    public bool IsEmpty() => Quantity == 0;
}
