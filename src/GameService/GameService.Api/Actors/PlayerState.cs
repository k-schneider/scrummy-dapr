namespace Scrummy.GameService.Api.Actors;

public class PlayerState
{
    public string Sid { get; set; } = string.Empty;
    public int PlayerId { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public DateTimeOffset JoinDate { get; set; } = DateTimeOffset.UtcNow;
    public bool IsHost { get; set; }
    public bool IsConnected { get; set; }
    public string? Vote { get; set; }
}
