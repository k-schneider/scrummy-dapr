namespace Scrummy.GameService.Api.Actors;

public class GameStatus
{
    public static readonly GameStatus None = new(0, nameof(None));
    public static readonly GameStatus InProgress = new(1, nameof(InProgress));
    public static readonly GameStatus GameOver = new(2, nameof(GameOver));

    public int Id { get; set; }

    public string Name { get; set; }

    public GameStatus()
        : this(None.Id, None.Name)
    {
    }

    public GameStatus(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
