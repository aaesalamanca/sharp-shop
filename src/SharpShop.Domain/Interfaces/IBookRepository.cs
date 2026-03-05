using SharpShop.Domain.Entities;

namespace SharpShop.Domain.Interfaces;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id);
    Task<IEnumerable<Book>> GetAllAsync();
    Task AddAsync(Book book);
    Task<bool> ExistsAsync(Guid id);
}
