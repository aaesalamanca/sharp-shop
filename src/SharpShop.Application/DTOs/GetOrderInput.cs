namespace SharpShop.Application.DTOs;

public record GetOrderInput(Guid OrderId, string? Currency = "EUR");
