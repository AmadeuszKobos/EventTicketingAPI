using EventTicketingAPI.DI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Repositories;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment;
var configuration = builder.Configuration;

builder.Services.AddDbContext<AppDbContext>(options =>
{
  if (environment.IsEnvironment("Testing"))
  {
    var databaseName = configuration.GetValue<string>("Testing:InMemoryDatabaseName") ?? "EventTicketingApiTests";
    options.UseInMemoryDatabase(databaseName);
  }
  else
  {
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
