namespace Services.Services
{
  public class PriceTierUpdateDto
  {
    public Guid EventId { get; set; }
    public string TierName { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
  }
}
