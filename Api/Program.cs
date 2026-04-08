using Api.Users;
using MongoDB.Driver;

namespace Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();
        builder.Services.AddSingleton<IMongoClient>(
            new MongoClient(builder.Configuration.GetConnectionString("MongoDb")));
        builder.Services.AddSingleton<IMongoDatabase>(provider =>
            provider.GetRequiredService<IMongoClient>().GetDatabase("Transactions"));
        builder.Services.AddSingleton<UserService>();

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
            app.MapOpenApi();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapPost("/users", async (User user, UserService userService, CancellationToken cancellationToken) =>
            {
                await userService.AddUser(user, cancellationToken);
                return Results.Ok(user);
            })
            .WithName("AddUser");
        
        app.MapGet("/users", async (IMongoDatabase mongoDatabase, CancellationToken cancellationToken) =>
            {
                var usersCollection = mongoDatabase.GetCollection<User>("users");
                return Results.Ok(await usersCollection.Find(Builders<User>.Filter.Empty).ToListAsync(cancellationToken));
            })
            .WithName("ListUsers");

        app.Run();
    }
}