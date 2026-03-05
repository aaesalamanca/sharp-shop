using SharpShop.Application.DTOs;
using SharpShop.Domain.Entities;
using SharpShop.Domain.Exceptions;
using SharpShop.Domain.Interfaces;

namespace SharpShop.Application.Services;

public class OrderService(
    IOrderRepository orderRepository,
    IBookRepository bookRepository,
    ICurrencyService currencyService
) : IOrderService
{
    private readonly IOrderRepository _orderRepository =
        orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly IBookRepository _bookRepository =
        bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
    private readonly ICurrencyService _currencyService =
        currencyService ?? throw new ArgumentNullException(nameof(currencyService));

    public async Task<GetOrderOutput> CreateOrder()
    {
        var order = new Order();
        await _orderRepository.AddAsync(order);

        return await MapToOutput(order);
    }

    public async Task<GetOrderOutput> AddBookToOrder(AddBookToOrderInput input)
    {
        var order =
            await _orderRepository.GetByIdAsync(input.OrderId)
            ?? throw new OrderNotFoundException(input.OrderId);

        var book =
            await _bookRepository.GetByIdAsync(input.BookId)
            ?? throw new BookNotFoundException(input.BookId);

        order.AddBook(book.Id);
        await _orderRepository.UpdateAsync(order);

        return await MapToOutput(order);
    }

    public async Task<GetOrderOutput> RemoveBookFromOrder(RemoveBookFromOrderInput input)
    {
        var order =
            await _orderRepository.GetByIdAsync(input.OrderId)
            ?? throw new OrderNotFoundException(input.OrderId);

        var book =
            await _bookRepository.GetByIdAsync(input.BookId)
            ?? throw new BookNotFoundException(input.BookId);

        order.RemoveBook(book.Id);
        await _orderRepository.UpdateAsync(order);

        return await MapToOutput(order);
    }

    public async Task<GetOrderOutput> ConfirmOrder(Guid orderId)
    {
        var order =
            await _orderRepository.GetByIdAsync(orderId)
            ?? throw new OrderNotFoundException(orderId);

        order.Confirm();
        await _orderRepository.UpdateAsync(order);

        return await MapToOutput(order);
    }

    public async Task<GetOrderOutput> ShipOrder(Guid orderId)
    {
        var order =
            await _orderRepository.GetByIdAsync(orderId)
            ?? throw new OrderNotFoundException(orderId);

        order.Ship();
        await _orderRepository.UpdateAsync(order);

        return await MapToOutput(order);
    }

    public async Task<GetOrderOutput> GetOrderById(GetOrderInput input)
    {
        var order =
            await _orderRepository.GetByIdAsync(input.OrderId)
            ?? throw new OrderNotFoundException(input.OrderId);

        return await MapToOutput(order, input.Currency ?? "EUR");
    }

    private async Task<GetOrderOutput> MapToOutput(Order order, string currency = "EUR")
    {
        var bookIds = order.Items.Select(i => i.BookId).ToList();
        var books = (await _bookRepository.GetAllAsync())
            .Where(b => bookIds.Contains(b.Id))
            .ToDictionary(b => b.Id);

        var unitPricesEur = books.ToDictionary(b => b.Key, b => b.Value.UnitPrice);
        var totalInEuros = order.CalculateTotal(unitPricesEur);

        decimal convertedTotal;
        if (currency.Equals("EUR", StringComparison.CurrentCultureIgnoreCase))
        {
            convertedTotal = totalInEuros;
        }
        else
        {
            convertedTotal = await _currencyService.ConvertAsync(totalInEuros, currency);
        }

        var items = await Task.WhenAll(
            order.Items.Select(async item =>
            {
                var book = books.GetValueOrDefault(item.BookId);
                decimal unitPrice = book?.UnitPrice ?? 0;
                if (!currency.Equals("EUR", StringComparison.CurrentCultureIgnoreCase))
                {
                    unitPrice = await _currencyService.ConvertAsync(unitPrice, currency);
                }

                return new GetOrderItemOutput(
                    item.BookId,
                    book?.Title ?? "Unknown",
                    book?.Author ?? "Unknown",
                    Math.Round(unitPrice, 2),
                    item.Quantity
                );
            })
        );

        return new GetOrderOutput(
            order.Id,
            order.Status.ToString(),
            order.CreatedAt,
            order.ConfirmedAt,
            order.ShippedAt,
            items.ToList(),
            Math.Round(convertedTotal, 2),
            currency.ToUpper()
        );
    }
}

public interface IOrderService
{
    Task<GetOrderOutput> CreateOrder();
    Task<GetOrderOutput> AddBookToOrder(AddBookToOrderInput input);
    Task<GetOrderOutput> RemoveBookFromOrder(RemoveBookFromOrderInput input);
    Task<GetOrderOutput> ConfirmOrder(Guid orderId);
    Task<GetOrderOutput> ShipOrder(Guid orderId);
    Task<GetOrderOutput> GetOrderById(GetOrderInput input);
}
