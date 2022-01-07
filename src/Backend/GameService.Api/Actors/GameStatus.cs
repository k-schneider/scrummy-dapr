namespace Scrummy.GameService.Api.Actors;

public class GameStatus
{
    public static readonly GameStatus New = new(0, nameof(New));
    public static readonly GameStatus Voting = new(1, nameof(Voting));
    public static readonly GameStatus Results = new(2, nameof(Results));

    public int Id { get; set; }

    public string Name { get; set; }

    public GameStatus()
        : this(New.Id, New.Name)
    {
    }

    public GameStatus(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
