using Microsoft.EntityFrameworkCore;
using SharpShop.Domain.Entities;
using SharpShop.Domain.Interfaces;
using SharpShop.Infrastructure.Data;

namespace SharpShop.Infrastructure.Repositories;

public class BookRepository(AppDbContext context) : IBookRepository
{
    public async Task<Book?> GetByIdAsync(Guid id)
    {
        return await context.Books.FindAsync(id);
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await context.Books.ToListAsync();
    }

    public async Task AddAsync(Book book)
    {
        await context.Books.AddAsync(book);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.Books.AnyAsync(b => b.Id == id);
    }
}
