using Serilog;
using SharpShop.Application;
using SharpShop.Application.DTOs;
using SharpShop.Application.Services;
using SharpShop.Domain.Exceptions;
using SharpShop.Infrastructure;
using SharpShop.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/sharp-shop-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddSerilog();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (app.Environment.IsDevelopment())
    {
        await AppDbContext.SeedAsync(dbContext);
    }
    else
    {
        await dbContext.Database.EnsureCreatedAsync();
    }
}

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet(
        "/api/orders/{orderId}",
        async (
            Guid orderId,
            string? currency,
            IOrderService orderService,
            ILogger<Program> logger
        ) =>
        {
            var input = new GetOrderInput(orderId, currency ?? "EUR");
            var result = await orderService.GetOrderById(input);
            return Results.Ok(MapToOrderResponse(result));
        }
    )
    .WithName("GetOrder");

app.MapPost(
        "/api/orders",
        async (IOrderService orderService, ILogger<Program> logger) =>
        {
            var result = await orderService.CreateOrder();
            logger.LogInformation("Created order {OrderId}", result.Id);
            return Results.Created($"/api/orders/{result.Id}", MapToOrderResponse(result));
        }
    )
    .WithName("CreateOrder");

app.MapPost(
        "/api/orders/{orderId}/books",
        async (
            Guid orderId,
            AddBookToOrderRequest request,
            IOrderService orderService,
            ILogger<Program> logger
        ) =>
        {
            var input = new AddBookToOrderInput(orderId, request.BookId);
            var result = await orderService.AddBookToOrder(input);
            logger.LogInformation(
                "Added book {BookId} to order {OrderId}",
                request.BookId,
                orderId
            );
            return Results.Ok(MapToOrderResponse(result));
        }
    )
    .WithName("AddBookToOrder");

app.MapDelete(
        "/api/orders/{orderId}/books/{bookId}",
        async (Guid orderId, Guid bookId, IOrderService orderService, ILogger<Program> logger) =>
        {
            var input = new RemoveBookFromOrderInput(orderId, bookId);
            var result = await orderService.RemoveBookFromOrder(input);
            logger.LogInformation("Removed book {BookId} from order {OrderId}", bookId, orderId);
            return Results.Ok(MapToOrderResponse(result));
        }
    )
    .WithName("RemoveBookFromOrder");

app.MapPatch(
        "/api/orders/{orderId}",
        async (
            Guid orderId,
            UpdateOrderStatusRequest request,
            IOrderService orderService,
            ILogger<Program> logger
        ) =>
        {
            GetOrderOutput result;
            if (request.Status.Equals("Confirmed", StringComparison.CurrentCultureIgnoreCase))
            {
                result = await orderService.ConfirmOrder(orderId);
                logger.LogInformation("Confirmed order {OrderId}", orderId);
            }
            else if (request.Status.Equals("Shipped", StringComparison.CurrentCultureIgnoreCase))
            {
                result = await orderService.ShipOrder(orderId);
                logger.LogInformation("Shipped order {OrderId}", orderId);
            }
            else
            {
                return Results.BadRequest(
                    new { error = "Invalid status. Use 'Confirmed' or 'Shipped'." }
                );
            }

            return Results.Ok(MapToOrderResponse(result));
        }
    )
    .WithName("UpdateOrderStatus");

app.MapGet(
        "/api/books",
        async (string? currency, IBookService bookService, ILogger<Program> logger) =>
        {
            var input = new GetAllBooksInput(currency ?? "EUR");
            var result = await bookService.GetAllBooks(input);
            return Results.Ok(new BooksResponse(result.Books.Select(MapToBookResponse).ToList()));
        }
    )
    .WithName("GetAllBooks");

app.MapPost(
        "/api/books",
        async (AddBookToStockRequest request, IBookService bookService, ILogger<Program> logger) =>
        {
            if (request.PriceInEuros <= 0)
            {
                return Results.BadRequest(new { error = "Price must be greater than 0" });
            }

            var input = new AddBookToStockInput(
                request.Title,
                request.Author,
                request.PriceInEuros
            );
            var result = await bookService.AddBookToStock(input);
            logger.LogInformation("Added book {Title} to stock", result.Title);
            return Results.Created($"/api/books/{result.Id}", MapToBookResponse(result));
        }
    )
    .WithName("AddBookToStock");

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var exception = context
            .Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()
            ?.Error;

        switch (exception)
        {
            case OrderNotFoundException:
            case BookNotFoundException:
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync(new { error = exception.Message });
                break;
            case OrderMustHaveAtLeastOneBookException:
            case OrderAlreadyConfirmedException:
            case OrderAlreadyShippedException:
            case CannotModifyConfirmedOrderException:
            case DomainException:
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new { error = exception.Message });
                break;
            default:
                await context.Response.WriteAsJsonAsync(
                    new { error = "An unexpected error occurred" }
                );
                break;
        }
    });
});

app.Run();

static OrderResponse MapToOrderResponse(GetOrderOutput output)
{
    return new OrderResponse(
        output.Id,
        output.Status,
        output.CreatedAt,
        output.ConfirmedAt,
        output.ShippedAt,
        output
            .Items.Select(i => new OrderItemResponse(
                i.Id,
                i.Title,
                i.Author,
                i.UnitPrice,
                i.Quantity
            ))
            .ToList(),
        output.TotalPrice,
        output.Currency
    );
}

static BookResponse MapToBookResponse(GetBookOutput output)
{
    return new BookResponse(
        output.Id,
        output.Title,
        output.Author,
        output.UnitPrice,
        output.Currency
    );
}

record AddBookToOrderRequest(Guid BookId);

record UpdateOrderStatusRequest(string Status);

record AddBookToStockRequest(string Title, string Author, decimal PriceInEuros);

record OrderResponse(
    Guid Id,
    string Status,
    DateTime CreatedAt,
    DateTime? ConfirmedAt,
    DateTime? ShippedAt,
    List<OrderItemResponse> Items,
    decimal TotalPrice,
    string Currency
);

record OrderItemResponse(Guid BookId, string Title, string Author, decimal UnitPrice, int Quantity);

record BooksResponse(List<BookResponse> Books);

record BookResponse(Guid Id, string Title, string Author, decimal UnitPrice, string Currency);
