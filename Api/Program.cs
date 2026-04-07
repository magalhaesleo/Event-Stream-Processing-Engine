using MongoDB.Driver;

namespace Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddSingleton<IMongoClient>(
            new MongoClient(builder.Configuration.GetConnectionString("MongoDb")));
        builder.Services.AddSingleton<IMongoDatabase>(provider =>
            provider.GetRequiredService<IMongoClient>().GetDatabase("Transactions"));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapPost("/transactions", async (Transaction transaction, IMongoDatabase mongoDatabase, CancellationToken cancellationToken) =>
            {
                var transactionsCollection = mongoDatabase.GetCollection<Transaction>("transactions");
                await transactionsCollection.InsertOneAsync(transaction, null, cancellationToken);
                return Results.Ok();
            })
            .WithName("AddTransaction");
        
        app.MapGet("/transactions", async (IMongoDatabase mongoDatabase, CancellationToken cancellationToken) =>
            {
                var transactionsCollection = mongoDatabase.GetCollection<Transaction>("transactions");
                return Results.Ok(await transactionsCollection.Find(Builders<Transaction>.Filter.Empty).ToListAsync(cancellationToken));
            })
            .WithName("ListTransactions");

        app.Run();
    }
}