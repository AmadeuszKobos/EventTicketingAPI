using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using Repositories.Repos;
using Xunit;

namespace EventTicketingAPI.Tests.Integration;

public class VenuesApiTests : IClassFixture<TestApiFactory>
{
  private readonly HttpClient _client;

  public VenuesApiTests(TestApiFactory factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task GetAll_ReturnsSeededVenueFromHttpPipeline()
  {
    // Arrange is handled by the seeded web application factory.

    // Act
    var response = await _client.GetAsync("/api/v1/Venues/GetAll");

    // Assert
    response.EnsureSuccessStatusCode();
    var venues = await response.Content.ReadFromJsonAsync<List<VenueReadDto>>();
    Assert.NotNull(venues);
    var venue = Assert.Single(venues!);
    Assert.Equal(TestApiFactory.SeededVenueId, venue.VenueId);
    Assert.Equal("Test Arena", venue.Name);
    Assert.Equal("Metropolis", venue.City);
    Assert.Equal(5000, venue.Capacity);
  }
}

public class TestApiFactory : WebApplicationFactory<Program>
{
  private readonly string _databaseName = $"ApiIntegrationTests_{Guid.NewGuid():N}";

  public static readonly Guid SeededVenueId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Testing");

    builder.ConfigureAppConfiguration((_, configBuilder) =>
    {
      var testingSettings = new Dictionary<string, string?>
      {
        ["Testing:InMemoryDatabaseName"] = _databaseName
      };

      configBuilder.AddInMemoryCollection(testingSettings);
    });

    builder.ConfigureServices(services =>
    {
      using var scope = services.BuildServiceProvider().CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

      context.Database.EnsureCreated();

      if (!context.Venues.Any(v => v.VenueId == SeededVenueId))
      {
        context.Venues.Add(new Venue
        {
          VenueId = SeededVenueId,
          Name = "Test Arena",
          City = "Metropolis",
          Capacity = 5000
        });
        context.SaveChanges();
      }
    });
  }
}
