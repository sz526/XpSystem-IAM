namespace XpSystem;

public class PlayerService
{
    private readonly IPlayerRepository _repository;
    private readonly ICurrentUser _currentUser;

    public PlayerService(IPlayerRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public PlayerModel CreatePlayer(string name)
        => _repository.Add(new PlayerModel { Name = name, UserId = _currentUser.Id });

    public List<PlayerModel> GetMyPlayers() => _repository.GetByUser(_currentUser.Id);

    public List<PlayerModel> GetAllPlayers() => _repository.GetAll();

    public PlayerModel? GetPlayer(int id) => _repository.GetById(id, _currentUser.Id);

    public void GainXp(int id, int amount)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "amount kan ikke være negativ");

        var player = _repository.GetById(id, _currentUser.Id)
            ?? throw new InvalidOperationException("Spiller ikke funnet");

        player.Xp += amount;
        while (player.Xp >= 100) { player.Xp -= 100; player.Level++; }
        _repository.Update(player);
    }

    public void DeletePlayer(int id)
    {
        if (!_repository.Delete(id, _currentUser.Id))
            throw new InvalidOperationException("Spiller ikke funnet");
    }
}
