using XpSystem;

namespace XpSystem.Api.Data;

public class EfPlayerRepository : IPlayerRepository
{
    private readonly XpSystemDbContext _context;

    public EfPlayerRepository(XpSystemDbContext context) => _context = context;

    public PlayerModel Add(PlayerModel player)
    {
        _context.Players.Add(player);
        _context.SaveChanges();
        return player;
    }

    public PlayerModel? GetById(int id, string userId)
        => _context.Players.FirstOrDefault(p => p.Id == id && p.UserId == userId);

    public List<PlayerModel> GetByUser(string userId)
        => _context.Players.Where(p => p.UserId == userId).ToList();

    public List<PlayerModel> GetAll() => _context.Players.ToList();

    public PlayerModel Update(PlayerModel player)
    {
        _context.Players.Update(player);
        _context.SaveChanges();
        return player;
    }

    public bool Delete(int id, string userId)
    {
        var player = _context.Players.FirstOrDefault(p => p.Id == id && p.UserId == userId);
        if (player is null) return false;

        _context.Players.Remove(player);
        _context.SaveChanges();
        return true;
    }
}
