namespace XpSystem;

public class FakePlayerRepository : IPlayerRepository
{
    private readonly List<PlayerModel> _players = new();
    private int _nextId = 1;

    public PlayerModel Add(PlayerModel player)
    {
        player.Id = _nextId++;
        _players.Add(player);
        return player;
    }

    public PlayerModel? GetById(int id)
        => _players.FirstOrDefault(p => p.Id == id);

    public List<PlayerModel> GetAll()
        => _players.ToList();

    public PlayerModel Update(PlayerModel player)
    {
        var existing = GetById(player.Id)
            ?? throw new InvalidOperationException("Spiller ikke funnet");

        existing.Name = player.Name;
        existing.Level = player.Level;
        existing.Xp = player.Xp;
        return existing;
    }
}
