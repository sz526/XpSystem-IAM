namespace XpSystem;

public class PlayerModel
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";   // Keycloak 的 sub
    public string Name { get; set; } = "";
    public int Level { get; set; } = 1;
    public int Xp { get; set; }
}
