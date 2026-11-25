namespace XpSystem;

public interface IPlayerRepository
{
    PlayerModel Add(PlayerModel player);
    PlayerModel? GetById(int id);
    List<PlayerModel> GetAll();
    PlayerModel Update(PlayerModel player);
}
