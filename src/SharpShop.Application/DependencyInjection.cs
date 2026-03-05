using Microsoft.Extensions.DependencyInjection;
using SharpShop.Application.Services;

namespace SharpShop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IBookService, BookService>();

        return services;
    }
}
