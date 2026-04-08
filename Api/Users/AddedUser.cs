namespace Api.Users;

public record AddedUser(
    User User,
    bool IsProcessed,
    DateTime CreatedAt
)
{
    public static AddedUser FromUser(User user) => new AddedUser(user, false, DateTime.UtcNow);
}
