using EventTicketingAPI.DI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
  var env = serviceProvider.GetRequiredService<IHostEnvironment>();
  if (env.IsEnvironment("Testing"))
  {
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var databaseName = configuration.GetValue<string>("Testing:InMemoryDatabaseName") ?? "EventTicketingApiTests";
    options.UseInMemoryDatabase(databaseName);
  }
  else
  {
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
  }
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterAppServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
