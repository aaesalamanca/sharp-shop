namespace SharpShop.Application.DTOs;

public record GetBookOutput(
    Guid Id,
    string Title,
    string Author,
    decimal UnitPrice,
    string Currency
);
