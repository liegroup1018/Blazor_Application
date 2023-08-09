using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

namespace BookDataConsole.Service;

public static class SetupHelpers
{

    // extension method
    public static async Task<int> SeedDatabaseIfNoBooksAsync(this EfCoreContext context, string JsonPath)
    {
        var numBooks = await context.Books.CountAsync();
        if (numBooks == 0)
        {
            //the database is empty so we fill it from a json file
            var books = BookJsonLoader.LoadBooks(JsonPath);
            context.Books.AddRange(books);
            await context.SaveChangesAsync();
        }

        return numBooks;
    }

}