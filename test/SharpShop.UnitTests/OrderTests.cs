using SharpShop.Domain.Entities;
using SharpShop.Domain.Enums;
using SharpShop.Domain.Exceptions;

namespace SharpShop.UnitTests;

public class OrderTests
{
    [Fact]
    public void CalculateTotal_WithoutDiscount_ReturnsSumOfItems()
    {
        // Arrange.
        var order = Order.CreateNew();
        var book1 = new Book("Book 1", "Author 1", 50m);
        var book2 = new Book("Book 2", "Author 2", 75m);

        order.AddBook(book1.Id);
        order.AddBook(book2.Id);

        // Act.
        var unitPrices = new Dictionary<Guid, decimal> { [book1.Id] = 50m, [book2.Id] = 75m };
        var total = order.CalculateTotal(unitPrices);

        // Assert.
        Assert.Equal(125m, total);
    }

    [Fact]
    public void CalculateTotal_WithDiscountAbove200_ReturnsDiscountedSum()
    {
        // Arrange.
        var order = Order.CreateNew();
        var book1 = new Book("Book 1", "Author 1", 120m);
        var book2 = new Book("Book 2", "Author 2", 100m);

        order.AddBook(book1.Id);
        order.AddBook(book2.Id);

        // Act.
        var unitPrices = new Dictionary<Guid, decimal> { [book1.Id] = 120m, [book2.Id] = 100m };
        var total = order.CalculateTotal(unitPrices);

        // Assert.
        Assert.Equal(198m, total);
    }

    [Fact]
    public void Confirm_WithItems_SetsStatusToConfirmed()
    {
        // Arrange.
        var order = Order.CreateNew();
        var book = new Book("Book 1", "Author 1", 50m);
        order.AddBook(book.Id);

        // Act.
        order.Confirm();

        // Assert.
        Assert.Equal(OrderStatus.Confirmed, order.Status);
        Assert.NotNull(order.ConfirmedAt);
    }

    [Fact]
    public void Ship_AfterConfirmed_SetsStatusToShipped()
    {
        // Arrange.
        var order = Order.CreateNew();
        var book = new Book("Book 1", "Author 1", 50m);
        order.AddBook(book.Id);
        order.Confirm();

        // Act.
        order.Ship();

        // Assert.
        Assert.Equal(OrderStatus.Shipped, order.Status);
        Assert.NotNull(order.ShippedAt);
    }

    [Fact]
    public void AddBook_AfterConfirmed_ThrowsCannotModifyConfirmedOrderException()
    {
        // Arrange.
        var order = Order.CreateNew();
        var book = new Book("Book 1", "Author 1", 50m);
        order.AddBook(book.Id);
        order.Confirm();
        var book2Id = Guid.NewGuid();

        // Act & Assert.
        Assert.Throws<CannotModifyConfirmedOrderException>(() => order.AddBook(book2Id));
    }

    [Fact]
    public void AddBook_AfterShipped_ThrowsOrderAlreadyShippedException()
    {
        // Arrange.
        var order = Order.CreateNew();
        var book = new Book("Book 1", "Author 1", 50m);
        order.AddBook(book.Id);
        order.Confirm();
        order.Ship();
        var book2Id = Guid.NewGuid();

        // Act & Assert.
        Assert.Throws<OrderAlreadyShippedException>(() => order.AddBook(book2Id));
    }

    [Fact]
    public void Confirm_WithoutItems_ThrowsOrderMustHaveAtLeastOneBookException()
    {
        // Arrange.
        var order = Order.CreateNew();

        // Act & Assert.
        Assert.Throws<OrderMustHaveAtLeastOneBookException>(() => order.Confirm());
    }
}
