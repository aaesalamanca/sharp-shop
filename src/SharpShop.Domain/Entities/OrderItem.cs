namespace SharpShop.Domain.Entities;

public class OrderItem
{
    public Guid OrderId { get; private set; }
    public Guid BookId { get; private set; }
    public Book Book { get; private set; } = null!;
    public int Quantity { get; private set; }

    private OrderItem() { }

    public OrderItem(Book book)
    {
        ArgumentNullException.ThrowIfNull(book, nameof(book));

        Book = book;
        BookId = book.Id;
        Quantity = 1;
    }

    public OrderItem(Book book, Guid orderId)
        : this(book)
    {
        OrderId = orderId;
    }

    public decimal CalculateTotal() => Book.UnitPrice * Quantity;

    public void AddOne() => Quantity += 1;

    public void DeleteOne() => Quantity -= 1;

    public bool IsEmpty() => Quantity == 0;
}
