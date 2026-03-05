namespace SharpShop.Domain.Entities;

public class OrderItem
{
    public Guid OrderId { get; private set; }
    public Guid BookId { get; private set; }
    public int Quantity { get; private set; }

    private OrderItem() { }

    public OrderItem(Guid bookId, Guid orderId, int quantity = 1)
    {
        BookId = bookId;
        OrderId = orderId;
        Quantity = quantity;
    }

    public decimal CalculateTotal(decimal unitPrice) => unitPrice * Quantity;

    public void AddOne() => Quantity += 1;

    public void DeleteOne() => Quantity -= 1;

    public void SetQuantity(int quantity) => Quantity = quantity;

    public bool IsEmpty() => Quantity == 0;
}
