namespace SharpShop.Application.DTOs;

public record GetOrderItemOutput(
    Guid Id,
    string Title,
    string Author,
    decimal UnitPrice,
    int Quantity
);
