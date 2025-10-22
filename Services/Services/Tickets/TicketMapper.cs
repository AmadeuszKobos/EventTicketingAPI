using Domain;
using Services.Services;

namespace Repositories.Repos
{
  public static partial class Mapper
  {
    public static TicketReadDto MapTicket(Ticket t) => new()
    {
      TicketId = t.TicketId,
      Code = t.Code,
      Status = t.Status,
      EventId = t.EventId,
      EventName = t.Event?.Name ?? "",
      TierName = t.PriceTier?.TierName ?? "",
      Amount = t.PriceTier?.Amount ?? 0
    };

    public static Ticket Map(TicketCreateDto dto) => new()
    {
      TicketId = Guid.NewGuid(),
      EventId = dto.EventId,
      PriceTierId = dto.PriceTierId,
      Code = dto.Code,
      Status = "available"
    };

    public static void Map(TicketUpdateDto dto, Ticket entity)
    {
      entity.PriceTierId = dto.PriceTierId;
      entity.Status = dto.Status;
    }
  }
}