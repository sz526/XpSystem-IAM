using XpSystem;

namespace XpSystem.Tests;

public class FakeCurrentUser : ICurrentUser
{
    public string Id { get; set; } = "test-user";
}

public class PlayerTests
{
    private static PlayerService CreateService(FakePlayerRepository repo, FakeCurrentUser user)
        => new(repo, user);

    [Fact]
    public void CreatePlayer_ShouldInitializeWithUniqueIdLevelOneAndZeroXp()
    {
        var repo = new FakePlayerRepository();
        var user = new FakeCurrentUser();
        var service = CreateService(repo, user);

        var player1 = service.CreatePlayer("Hero");
        var player2 = service.CreatePlayer("Mage");

        Assert.Equal(1, player1.Id);
        Assert.Equal(2, player2.Id);
        Assert.Equal(1, player1.Level);
        Assert.Equal(0, player1.Xp);
        Assert.Equal("test-user", player1.UserId);
    }

    [Fact]
    public void GainXp_SpecificPlayerId_ShouldIncreaseXpAndLevelUp()
    {
        var repo = new FakePlayerRepository();
        var user = new FakeCurrentUser();
        var service = CreateService(repo, user);
        var player = service.CreatePlayer("Hero");

        service.GainXp(player.Id, 40);
        Assert.Equal(40, player.Xp);

        service.GainXp(player.Id, 60);
        Assert.Equal(2, player.Level);
        Assert.Equal(0, player.Xp);
    }

    [Fact]
    public void GetMyPlayers_ShouldReturnOnlyCurrentUsersPlayers()
    {
        var repo = new FakePlayerRepository();
        var alice = new FakeCurrentUser { Id = "alice-sub" };
        var bob = new FakeCurrentUser { Id = "bob-sub" };

        var aliceService = CreateService(repo, alice);
        var bobService = CreateService(repo, bob);

        aliceService.CreatePlayer("Hero");
        aliceService.CreatePlayer("Mage");
        bobService.CreatePlayer("Thief");

        Assert.Equal(2, aliceService.GetMyPlayers().Count);
        Assert.Single(bobService.GetMyPlayers());
    }

    [Fact]
    public void GainXp_NegativeAmount_ShouldThrowArgumentOutOfRangeException()
    {
        var repo = new FakePlayerRepository();
        var user = new FakeCurrentUser();
        var service = CreateService(repo, user);
        var player = service.CreatePlayer("Hero");

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => service.GainXp(player.Id, -50));
        Assert.Contains("amount", exception.Message);
    }
}
