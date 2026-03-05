using SharpShop.Application.DTOs;
using SharpShop.Domain.Entities;
using SharpShop.Domain.Interfaces;

namespace SharpShop.Application.Services;

public class BookService(IBookRepository bookRepository, ICurrencyService currencyService)
    : IBookService
{
    private readonly IBookRepository _bookRepository =
        bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
    private readonly ICurrencyService _currencyService =
        currencyService ?? throw new ArgumentNullException(nameof(currencyService));

    public async Task<GetBookOutput> AddBookToStock(AddBookToStockInput input)
    {
        var book = new Book(input.Title, input.Author, input.UnitPrice);
        await _bookRepository.AddAsync(book);

        return new GetBookOutput(book.Id, book.Title, book.Author, book.UnitPrice, "EUR");
    }

    public async Task<GetAllBooksOutput> GetAllBooks(GetAllBooksInput input)
    {
        var books = await _bookRepository.GetAllAsync();
        var currency = input.Currency ?? "EUR";

        var bookOutputs = new List<GetBookOutput>();

        foreach (var book in books)
        {
            decimal unitPrice = book.UnitPrice;
            if (!currency.Equals("EUR", StringComparison.CurrentCultureIgnoreCase))
            {
                unitPrice = await _currencyService.ConvertAsync(book.UnitPrice, currency);
            }

            bookOutputs.Add(
                new GetBookOutput(
                    book.Id,
                    book.Title,
                    book.Author,
                    Math.Round(unitPrice, 2),
                    currency.ToUpper()
                )
            );
        }

        return new GetAllBooksOutput(bookOutputs);
    }
}

public interface IBookService
{
    Task<GetBookOutput> AddBookToStock(AddBookToStockInput input);
    Task<GetAllBooksOutput> GetAllBooks(GetAllBooksInput input);
}
