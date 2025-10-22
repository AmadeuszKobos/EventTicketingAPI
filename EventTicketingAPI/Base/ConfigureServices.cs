using Repositories.Repos;
using Services.Services;

namespace EventTicketingAPI.DI
{
  public static partial class ConfigureServices
  {
    public static IServiceCollection RegisterAppServices(this IServiceCollection services)
    {
      RegisterCustomDependencies(services);

      return services;
    }

    private static void RegisterCustomDependencies(IServiceCollection services)
    {
      services.AddScoped<ICustomerRepository, CustomerRepository>();
      services.AddScoped<ICustomerService, CustomerService>();

      services.AddScoped<IEventRepository, EventRepository>();
      services.AddScoped<IEventService, EventService>();
      
      services.AddScoped<IOrderRepository, OrderRepository>();
      services.AddScoped<IOrderService, OrderService>();

      services.AddScoped<IPriceTierRepository, PriceTierRepository>();
      services.AddScoped<IPriceTierService, PriceTierService>();

      services.AddScoped<ITicketRepository, TicketRepository>();
      services.AddScoped<ITicketService, TicketService>();

      services.AddScoped<IVenueRepository, VenueRepository>();
      services.AddScoped<IVenueService, VenueService>();

    }
  }
}