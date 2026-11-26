namespace XpSystem;

public interface IPlayerRepository
{
    PlayerModel Add(PlayerModel player);
    PlayerModel? GetById(int id, string userId);
    List<PlayerModel> GetByUser(string userId);
    List<PlayerModel> GetAll();                       // 给后面的 admin 用
    PlayerModel Update(PlayerModel player);
    bool Delete(int id, string userId);
}
