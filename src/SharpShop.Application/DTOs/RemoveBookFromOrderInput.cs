namespace SharpShop.Application.DTOs;

public record RemoveBookFromOrderInput(Guid OrderId, Guid BookId);
