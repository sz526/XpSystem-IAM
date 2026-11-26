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

    public PlayerModel? GetById(int id, string userId)
        => _players.FirstOrDefault(p => p.Id == id && p.UserId == userId);

    public List<PlayerModel> GetByUser(string userId)
        => _players.Where(p => p.UserId == userId).ToList();

    public List<PlayerModel> GetAll() => _players.ToList();

    public PlayerModel Update(PlayerModel player)
    {
        var existing = _players.FirstOrDefault(p => p.Id == player.Id && p.UserId == player.UserId)
            ?? throw new InvalidOperationException("Spiller ikke funnet");

        existing.Name = player.Name;
        existing.Level = player.Level;
        existing.Xp = player.Xp;
        return existing;
    }

    public bool Delete(int id, string userId)
    {
        var existing = _players.FirstOrDefault(p => p.Id == id && p.UserId == userId);
        if (existing is null) return false;
        _players.Remove(existing);
        return true;
    }
}
