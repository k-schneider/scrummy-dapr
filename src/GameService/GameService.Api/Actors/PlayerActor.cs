namespace Scrummy.GameService.Api.Actors;

public class PlayerActor : Actor, IPlayerActor
{
    private const string PlayerStateName = "PlayerState";

    private readonly IEventBus _eventBus;
    private PlayerState _playerState = null!;

    private string Sid => Id.GetId();

    public PlayerActor(ActorHost host, IEventBus eventBus)
        : base(host)
    {
        _eventBus = eventBus;
    }

    protected override async Task OnActivateAsync()
    {
        var playerState = await StateManager.TryGetStateAsync<PlayerState>(PlayerStateName);
        _playerState = playerState.HasValue ? playerState.Value : new PlayerState(Sid);

        await base.OnActivateAsync();
    }

    public async Task AddConnection(string connectionId, CancellationToken cancellationToken = default)
    {
        if (_playerState.GameId is null)
        {
            throw new InvalidOperationException("Player has not joined a game");
        }

        if (_playerState.ConnectionIds.Add(connectionId))
        {
            await SavePlayerState(cancellationToken);

            await _eventBus.PublishAsync(
                new PlayerConnectedIntegrationEvent(
                    connectionId,
                    Sid,
                    _playerState.GameId,
                    _playerState.PlayerId,
                    _playerState.ConnectionIds.Count),
                cancellationToken);
        }
    }

    public async Task CastVote(string vote, CancellationToken cancellationToken = default)
    {
        EnsureGame();

        if (_playerState.IsSpectator)
        {
            throw new InvalidOperationException("Spectators cannot vote");
        }

        var previousVote = _playerState.Vote;
        _playerState.Vote = vote;

        await SavePlayerState(cancellationToken);

        await _eventBus.PublishAsync(
            new PlayerVoteCastIntegrationEvent(
                Sid,
                _playerState.PlayerId,
                vote,
                previousVote,
                _playerState.GameId!),
            cancellationToken);
    }

    public async Task FlipCards(CancellationToken cancellationToken = default)
    {
        EnsureHost("Only the host can flip the cards");

        await GetGameActor(_playerState.GameId!).ShowResults(cancellationToken);

        await _eventBus.PublishAsync(
            new CardsFlippedIntegrationEvent(_playerState.GameId!),
            cancellationToken);
    }

    public Task<PlayerState> GetPlayerState(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_playerState);
    }

    public async Task<int> JoinGame(string gameId, string nickname, CancellationToken cancellationToken = default)
    {
        if (_playerState.GameId is not null)
        {
            throw new InvalidOperationException("Player is already in a game");
        }

        var addPlayerResult = await GetGameActor(gameId).AddPlayer(Sid, cancellationToken);

        _playerState.PlayerId = addPlayerResult.PlayerId;
        _playerState.Nickname = nickname;
        _playerState.GameId = gameId;
        _playerState.IsHost = addPlayerResult.PlayerCount == 1;

        await SavePlayerState(cancellationToken);

        await _eventBus.PublishAsync(
            new PlayerJoinedIntegrationEvent(
                Sid,
                _playerState.PlayerId,
                nickname,
                gameId),
            cancellationToken);

        return _playerState.PlayerId;
    }

    public async Task LeaveGame(CancellationToken cancellationToken = default)
    {
        EnsureGame();

        var playerCount = await GetGameActor(_playerState.GameId!).RemovePlayer(Sid, cancellationToken);

        var playerId = _playerState.PlayerId;
        var gameId = _playerState.GameId!;
        var isHost = _playerState.IsHost;

        await Reset(cancellationToken);

        await _eventBus.PublishAsync(
            new PlayerLeftIntegrationEvent(
                Sid,
                playerId,
                gameId,
                isHost,
                playerCount),
            cancellationToken);
    }

    public async Task NudgePlayer(int playerId, CancellationToken cancellationToken = default)
    {
        EnsureGame();

        var gameState = await GetGameActor(_playerState.GameId!).GetGameState(cancellationToken);

        var player = gameState.Players.FirstOrDefault(p => p.PlayerId == playerId);

        if (player == default)
        {
            throw new InvalidOperationException("Player not found");
        }

        await _eventBus.PublishAsync(
            new PlayerNudgedIntegrationEvent(
                Sid,
                _playerState.PlayerId,
                player.Sid,
                player.PlayerId,
                _playerState.GameId!),
            cancellationToken);
    }

    public async Task PlayAgain(CancellationToken cancellationToken = default)
    {
        EnsureHost("Only the host can choose to play again");

        await GetGameActor(_playerState.GameId!).BeginVoting(cancellationToken);

        await _eventBus.PublishAsync(
            new NewVoteStartedIntegrationEvent(_playerState.GameId!),
            cancellationToken);
    }

    public async Task PromotePlayer(int playerId, CancellationToken cancellationToken = default)
    {
        EnsureHost("Only the host can promote other players");

        var gameState = await GetGameActor(_playerState.GameId!).GetGameState(cancellationToken);

        var player = gameState.Players.FirstOrDefault(p => p.PlayerId == playerId);

        if (player == default)
        {
            throw new InvalidOperationException("Player not found");
        }

        await GetPlayerActor(player.Sid).PromoteToHost(cancellationToken);

        _playerState.IsHost = false;
        await SavePlayerState(cancellationToken);
    }

    public async Task PromoteToHost(CancellationToken cancellationToken = default)
    {
        if (_playerState.GameId is null)
        {
            // Because this is invoked by event handlers we won't throw here
            return;
        }

        _playerState.IsHost = true;

        await SavePlayerState(cancellationToken);

        await _eventBus.PublishAsync(
                new HostChangedIntegrationEvent(
                    Sid,
                    _playerState.PlayerId,
                    _playerState.GameId),
                cancellationToken);
    }

    public async Task RecallVote(CancellationToken cancellationToken = default)
    {
        EnsureGame();

        if (_playerState.IsSpectator)
        {
            throw new InvalidOperationException("Spectators cannot vote");
        }

        var previousVote = _playerState.Vote;
        _playerState.Vote = null;

        await SavePlayerState(cancellationToken);

        await _eventBus.PublishAsync(
            new PlayerVoteRecalledIntegrationEvent(
                Sid,
                _playerState.PlayerId,
                previousVote,
                _playerState.GameId!),
            cancellationToken);
    }

    public async Task RemoveConnection(string connectionId, CancellationToken cancellationToken = default)
    {
        if (_playerState.ConnectionIds.Remove(connectionId))
        {
            await SavePlayerState(cancellationToken);

            await _eventBus.PublishAsync(
                new PlayerDisconnectedIntegrationEvent(
                    connectionId,
                    Sid,
                    _playerState.GameId!,
                    _playerState.PlayerId,
                    _playerState.ConnectionIds.Count),
                cancellationToken);
        }
    }

    public async Task RemovePlayer(int playerId, CancellationToken cancellationToken = default)
    {
        EnsureHost("Only host can remove players");

        if (_playerState.PlayerId == playerId)
        {
            throw new InvalidOperationException("Cannot remove yourself");
        }

        var gameState = await GetGameActor(_playerState.GameId!).GetGameState(cancellationToken);

        var player = gameState.Players.FirstOrDefault(p => p.PlayerId == playerId);

        if (player == default)
        {
            throw new InvalidOperationException("Player not found");
        }

        await GetGameActor(_playerState.GameId!).RemovePlayer(player.Sid, cancellationToken);

        await _eventBus.PublishAsync(
            new PlayerRemovedIntegrationEvent(
                player.Sid,
                player.PlayerId,
                _playerState.GameId!),
            cancellationToken);
    }

    public async Task Reset(CancellationToken cancellationToken = default)
    {
        await StateManager.TryRemoveStateAsync(PlayerStateName, cancellationToken);
        _playerState = new(Sid);
    }

    public async Task ResetVote(CancellationToken cancellationToken = default)
    {
        _playerState.Vote = null;
        await SavePlayerState(cancellationToken);
    }

    public async Task ResetVotes(CancellationToken cancellationToken = default)
    {
        EnsureHost("Only the host can reset votes");

        var gameState = await GetGameActor(_playerState.GameId!).GetGameState(cancellationToken);

        // Reset my vote
        await ResetVote(cancellationToken);

        // Reset other player votes
        await Task.WhenAll(gameState.Players
            .Where(p => p.Sid != Sid)
            .Select(p => GetPlayerActor(p.Sid).ResetVote(cancellationToken)));

        await _eventBus.PublishAsync(
            new VotesResetIntegrationEvent(_playerState.GameId!),
            cancellationToken);
    }

    public async Task StartSpectating(CancellationToken cancellationToken = default)
    {
        EnsureGame();

        _playerState.IsSpectator = true;
        _playerState.Vote = null;

        await SavePlayerState(cancellationToken);

        await _eventBus.PublishAsync(
            new PlayerIsSpectatorChangedIntegrationEvent(
                Sid,
                _playerState.PlayerId,
                _playerState.IsSpectator,
                _playerState.GameId!),
            cancellationToken);
    }

    public async Task StopSpectating(CancellationToken cancellationToken = default)
    {
        EnsureGame();

        _playerState.IsSpectator = false;
        _playerState.Vote = null;

        await SavePlayerState(cancellationToken);

        await _eventBus.PublishAsync(
            new PlayerIsSpectatorChangedIntegrationEvent(
                Sid,
                _playerState.PlayerId,
                _playerState.IsSpectator,
                _playerState.GameId!),
            cancellationToken);
    }

    public async Task UpdateNickname(string nickname, CancellationToken cancellationToken = default)
    {
        EnsureGame();

        _playerState.Nickname = nickname;

        await SavePlayerState(cancellationToken);

        await _eventBus.PublishAsync(
            new PlayerNicknameChangedIntegrationEvent(
                _playerState.Sid,
                _playerState.PlayerId,
                nickname,
                _playerState.GameId!),
            cancellationToken);
    }

    public void EnsureGame(string error = "Player is not in a game")
    {
        if (_playerState.GameId is null)
        {
            throw new InvalidOperationException(error);
        }
    }

    private void EnsureHost(string error = "Not the host")
    {
        EnsureGame();

        if (!_playerState.IsHost)
        {
            throw new InvalidOperationException(error);
        }
    }

    private IGameActor GetGameActor(string gameId) =>
        ProxyFactory.CreateActorProxy<IGameActor>(
            new ActorId(gameId),
            typeof(GameActor).Name);

    private IPlayerActor GetPlayerActor(string sid) =>
        ProxyFactory.CreateActorProxy<IPlayerActor>(
            new ActorId(sid),
            typeof(PlayerActor).Name);

    private Task SavePlayerState(CancellationToken cancellationToken = default) =>
        StateManager.SetStateAsync(PlayerStateName, _playerState, cancellationToken);
}
