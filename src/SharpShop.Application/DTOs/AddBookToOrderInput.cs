namespace SharpShop.Application.DTOs;

public record AddBookToOrderInput(Guid OrderId, Guid BookId);
