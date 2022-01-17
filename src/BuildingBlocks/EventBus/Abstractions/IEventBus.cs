namespace Scrummy.BuildingBlocks.EventBus.Abstractions;

public interface IEventBus
{
    Task PublishAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
