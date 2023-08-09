using System.Text.Json;
using DataLayer.EfClasses;

namespace BookDataConsole.Service;


public class BookJsonLoader
{
    public static IEnumerable<Book> LoadBooks(string FileName)
    //public static void LoadBooks(string FileName)
    {
        FileStream stream = new(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        using StreamReader reader = new(stream); // 使用using, 有unmanaged memory
        string jsons = reader.ReadToEnd();
        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
        var Jsondecodes = JsonSerializer.Deserialize<ICollection<BookInfoJson>>(jsons, options);
        //Console.WriteLine(Jsondecodes.Count);
        
        var authorDict = new Dictionary<string, Author>();
        var tagDict = new Dictionary<string, Tag>();
        
        foreach (var bookInfoJson in Jsondecodes)
        {
            foreach (var author in bookInfoJson.authors)
            {
                if (!authorDict.ContainsKey(author))
                    authorDict[author] = new Author {Name = author};
            }
            foreach (var category in bookInfoJson.categories)
            {
                if (!tagDict.ContainsKey(category))
                    tagDict[category] = new Tag { TagId = category };
            }
        }

        var books = Jsondecodes.Select(x => 
                CreateBookWithRefs(x, authorDict, tagDict));
        return books;
    }

    /// <summary>
    /// 依一个bookInfoJson转化成一个Json实例
    /// </summary>
    /// <param name="bookInfoJson"></param>
    /// <param name="authorDict"></param>
    /// <param name="tagsDict"></param>
    /// <returns></returns>
    private static Book CreateBookWithRefs(BookInfoJson bookInfoJson,
        Dictionary<string, Author> authorDict,
        Dictionary<string, Tag> tagsDict)
    {
        var book = new Book
        {
            Title = bookInfoJson.title,
            Description = bookInfoJson.description,
            PublishedOn = DecodePubishDate(bookInfoJson.publishedDate),
            Publisher = bookInfoJson.publisher,
            Price = (decimal) (bookInfoJson.saleInfoListPriceAmount ?? -1),
            ImageUrl = bookInfoJson.imageLinksThumbnail
        };
        
        byte i = 0;
        book.AuthorsLink = new List<BookAuthor>();
        foreach (var author in bookInfoJson.authors)
        {
            book.AuthorsLink.Add(new BookAuthor {Book = book, Author = authorDict[author], Order = i++});
        }
        
        book.Tags = new List<Tag>();
        foreach (var category in bookInfoJson.categories)
        {
            book.Tags.Add(tagsDict[category]);
        }
        
        if (bookInfoJson.averageRating != null)
            book.Reviews =
                CalculateReviewsToMatch((double) bookInfoJson.averageRating, (int) bookInfoJson.ratingsCount);

        return book;

    }
    
    /// <summary>
    ///     This create the right number of reviews that add up to the average rating
    /// </summary>
    /// <param name="averageRating"></param>
    /// <param name="ratingsCount"></param>
    /// <returns></returns>
    internal static IList<Review> CalculateReviewsToMatch(double averageRating, int ratingsCount)
    {
        var reviews = new List<Review>();
        var currentAve = averageRating;
        for (int i = 0; i < ratingsCount; i++)
        {
            reviews.Add(new Review
            {
                VoterName = "anonymous",
                NumStars = (int) (currentAve > averageRating
                    ? Math.Truncate(averageRating)
                    : Math.Ceiling(averageRating))
            });
            currentAve = reviews.Average(x => x.NumStars);
        }

        return reviews;
    }
    
    private static DateTime DecodePubishDate(string publishedDate)
    {
        var split = publishedDate.Split('-');
        switch (split.Length)
        {
            case 1:
                return new DateTime(int.Parse(split[0]), 1, 1);
            case 2:
                return new DateTime(int.Parse(split[0]), int.Parse(split[1]), 1);
            case 3:
                return new DateTime(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
        }

        throw new InvalidOperationException($"The json publishedDate failed to decode: string was {publishedDate}");
    }
}