namespace Api;

public record Transaction(
    string Id,
    string Type,
    decimal Amount,
    DateTime CreatedAt
);
    