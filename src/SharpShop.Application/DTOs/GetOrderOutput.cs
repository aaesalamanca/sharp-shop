namespace SharpShop.Application.DTOs;

public record GetOrderOutput(
    Guid Id,
    string Status,
    DateTime CreatedAt,
    DateTime? ConfirmedAt,
    DateTime? ShippedAt,
    List<GetOrderItemOutput> Items,
    decimal TotalPrice,
    string Currency
);
