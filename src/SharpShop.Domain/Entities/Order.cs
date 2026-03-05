using SharpShop.Domain.Enums;
using SharpShop.Domain.Exceptions;

namespace SharpShop.Domain.Entities;

public class Order
{
    public Guid Id { get; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public List<OrderItem> Items { get; private set; } = [];

    private const decimal DiscountThreshold = 200m;
    private const decimal DiscountPercentage = 0.10m;

    public Order()
    {
        Id = Guid.NewGuid();
        Status = OrderStatus.Created;
        CreatedAt = DateTime.UtcNow;
        Items = [];
    }

    public static Order CreateNew() => new();

    public void AddBook(Book book)
    {
        if (Status == OrderStatus.Confirmed)
            throw new CannotModifyConfirmedOrderException();

        if (Status == OrderStatus.Shipped)
            throw new OrderAlreadyShippedException();

        var existingItem = Items.SingleOrDefault(i => i.BookId == book.Id);

        if (existingItem is not null)
            existingItem.AddOne();
        else
            Items.Add(new OrderItem(book));
    }

    public void RemoveBook(Guid bookId)
    {
        if (Status == OrderStatus.Confirmed)
            throw new CannotModifyConfirmedOrderException();

        if (Status == OrderStatus.Shipped)
            throw new OrderAlreadyShippedException();

        var item = Items.FirstOrDefault(i => i.BookId == bookId);

        if (item is null)
            return;

        item.DeleteOne();

        if (item.IsEmpty())
        {
            Items.Remove(item);
        }
    }

    public void Confirm()
    {
        if (Items.Count == 0)
            throw new OrderMustHaveAtLeastOneBookException();

        if (Status == OrderStatus.Confirmed)
            throw new OrderAlreadyConfirmedException();

        if (Status == OrderStatus.Shipped)
            throw new OrderAlreadyShippedException();

        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
    }

    public void Ship()
    {
        if (Status == OrderStatus.Shipped)
            throw new OrderAlreadyShippedException();

        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Order must be confirmed before shipping");

        Status = OrderStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
    }

    public decimal CalculateTotal()
    {
        var subtotal = Items.Sum(i => i.CalculateTotal());

        if (subtotal > DiscountThreshold)
            subtotal *= 1 - DiscountPercentage;

        return Math.Round(subtotal, 2);
    }
}
