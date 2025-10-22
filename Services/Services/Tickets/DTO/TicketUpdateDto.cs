namespace Services.Services
{
  public class TicketUpdateDto
  {
    public Guid PriceTierId { get; set; }
    public string Status { get; set; } = "available";
  }
}