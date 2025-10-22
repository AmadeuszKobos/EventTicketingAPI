using Domain;
using Services.Services;

namespace Repositories.Repos
{
  public static partial class Mapper
  {
    public static PriceTierReadDto Map(PriceTier p) => new()
    {
      PriceTierId = p.PriceTierId,
      EventId = p.EventId,
      TierName = p.TierName,
      Amount = p.Amount,
      Currency = p.Currency,
      EventName = p.Event?.Name ?? ""
    };

    public static PriceTier Map(PriceTierCreateDto dto) => new()
    {
      PriceTierId = Guid.NewGuid(),
      EventId = dto.EventId,
      TierName = dto.TierName,
      Amount = dto.Amount,
      Currency = dto.Currency
    };

    public static void Map(PriceTierUpdateDto dto, PriceTier entity)
    {
      entity.EventId = dto.EventId;
      entity.TierName = dto.TierName;
      entity.Amount = dto.Amount;
      entity.Currency = dto.Currency;
    }
  }
}
