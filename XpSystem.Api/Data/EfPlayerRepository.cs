using Microsoft.EntityFrameworkCore;
using XpSystem;

namespace XpSystem.Api.Data;

public class EfPlayerRepository : IPlayerRepository
{
    private readonly XpSystemDbContext _context;

    public EfPlayerRepository(XpSystemDbContext context)
    {
        _context = context;
    }

    public PlayerModel Add(PlayerModel player)
    {
        _context.Players.Add(player);
        _context.SaveChanges();
        return player;
    }

    public PlayerModel? GetById(int id)
        => _context.Players.FirstOrDefault(p => p.Id == id);

    public List<PlayerModel> GetAll()
        => _context.Players.ToList();

    public PlayerModel Update(PlayerModel player)
    {
        _context.Players.Update(player);
        _context.SaveChanges();
        return player;
    }
}
