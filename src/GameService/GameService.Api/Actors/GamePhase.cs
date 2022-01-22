namespace Scrummy.GameService.Api.Actors;

public class GamePhase
{
    public static readonly GamePhase Voting = new(0, nameof(Voting));
    public static readonly GamePhase Results = new(1, nameof(Results));

    public int Id { get; set; }

    public string Name { get; set; }

    public GamePhase()
        : this(Voting.Id, Voting.Name)
    {
    }

    public GamePhase(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
