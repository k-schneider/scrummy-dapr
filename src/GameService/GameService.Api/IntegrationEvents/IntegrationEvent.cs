namespace Scrummy.GameService.Api.IntegrationEvents;

public record IntegrationEvent
{
    public Guid Id { get; }
    public DateTimeOffset CreationDate { get; }

    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTimeOffset.UtcNow;
    }
}
