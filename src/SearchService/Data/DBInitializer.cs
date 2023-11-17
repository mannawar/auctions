using System.Text.Json;
using MongoDB.Entities;
using SearchService.Services;

namespace SearchService.Data;

public class DBInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();

        var count = await DB.CountAsync<Item>();
        using var scope = app.Services.CreateScope();
        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();
        var items = await httpClient.GetItemsForSearchDb();
        Console.WriteLine(items.Count+ " returned from auction db");

        if (items.Count > 0) await DB.SaveAsync(items);
    }
}