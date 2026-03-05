namespace SharpShop.Domain.Exceptions;

public class BookNotFoundException : DomainException
{
    public BookNotFoundException()
        : base("Book not found") { }

    public BookNotFoundException(Guid bookId)
        : base($"Book with id {bookId} not found") { }

    public BookNotFoundException(Guid bookId, Exception innerException)
        : base($"Book with id {bookId} not found", innerException) { }
}
