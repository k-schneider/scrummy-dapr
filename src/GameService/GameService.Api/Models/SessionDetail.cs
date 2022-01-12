namespace Scrummy.GameService.Api.Models;

public class SessionDetail
{
    public string GameId { get; set; } = string.Empty;
    public int PlayerId { get; set; }
    public string Sid { get; set; } = string.Empty;
}
