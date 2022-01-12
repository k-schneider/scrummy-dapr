namespace Scrummy.GameService.Api.Actors;

public class PlayerState
{
    public string Sid { get; set; } = string.Empty;
    public int PlayerId { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
}
