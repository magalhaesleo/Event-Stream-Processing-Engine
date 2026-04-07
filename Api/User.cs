namespace Api;

public record User(
    string Id,
    string Category,
    decimal Amount,
    DateTime CreatedAt
);
