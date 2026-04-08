using MongoDB.Driver;

namespace Api.Users;

public class UserService(
    IMongoClient mongoClient,
    IMongoDatabase mongoDatabase)
{
    public async Task AddUser(User user, CancellationToken cancellationToken)
    {
        var usersCollection = mongoDatabase.GetCollection<User>("users");
        var userChangesCollection = mongoDatabase.GetCollection<AddedUser>("userChanges");
        using var sessionHandle = await mongoClient.StartSessionAsync(null, cancellationToken);
        await sessionHandle.WithTransactionAsync(async (session, token) =>
        {
            await usersCollection.InsertOneAsync(session, user, null, token);
            var addedUser = AddedUser.FromUser(user);
            await userChangesCollection.InsertOneAsync(session, addedUser, null, token);
            return new object();
        }, null, cancellationToken);
    }
}