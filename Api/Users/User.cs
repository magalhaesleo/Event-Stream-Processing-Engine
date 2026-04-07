namespace Api.Users;

public record User(
    string Id,
    string Category,
    decimal Amount,
    DateTime CreatedAt
);
