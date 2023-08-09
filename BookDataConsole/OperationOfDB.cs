using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

namespace BookDataConsole;

public static class OperationOfDB
{
    public static async Task Insert(EfCoreContext context)
    {
        var book4 = new Book
        {
            Title = "Quantum Networking",
            Description = "Entangled quantum networking provides faster-than-light data communications",
            PublishedOn = new DateTime(2057, 1, 1),
            Price = 220,
            Tags = new List<Tag>{new Tag{ TagId = "Quantum Entanglement" } },
            Publisher = "Apress",
        };
        
        book4.AuthorsLink = new List<BookAuthor>
            {new BookAuthor {Author = new Author {Name = "Future Person"}, Book = book4}};
        
        book4.Reviews = new List<Review>
        {
            new Review
            {
                VoterName = "Jon P Smith", NumStars = 5,
                Comment = "I look forward to reading this book, if I am still alive!"
            },
            new Review
            {
                VoterName = "Albert Einstein", NumStars = 5, Comment = "I write this book if I was still alive!"
            }
        };
        
        book4.Promotion = new PriceOffer {NewPrice = 219, PromotionalText = "Save $1 if you order 40 years ahead!"};
        
        
        context.Books.Add(book4);
        await context.SaveChangesAsync();
        Console.WriteLine("Inserted success!!!");
    }

    public static async Task SearchAll(EfCoreContext context)
    {
        // 读取数据库中所有数据？不包括关系部分
        var books = await context.Books.ToListAsync();
        foreach (var book in books)
        {
            Console.WriteLine($"{book.Title} - {book.Price}");
            Console.WriteLine("============");
        }
    }

    public static void SearchPartion1(EfCoreContext context)
    {
        var quantumBooks = context.Books.Where(p => p.Title.StartsWith("Quantum")).ToList();
        foreach (var book in quantumBooks)
        {
            Console.WriteLine($"{book.Title} - {book.Price}");
            Console.WriteLine("============");
        }
    }

    public static void SearchPartion2(EfCoreContext context)
    {
        // 不同价格的书，分别有多少
        var bookshaveprice = context.Books.GroupBy(p => p.Price)
            .Select(p => new {Price = p.Key, Count = p.Count()}).ToList();
        foreach (var books in bookshaveprice)
        {
            Console.WriteLine($"{books.Price} - {books.Count}");
            Console.WriteLine("============");
        }
    }

    public static async Task UpdateAsync(EfCoreContext context)
    {
        var book = context.Books.First(p => p.Price == -1);
        book.Price = 56.8m;
        await  context.SaveChangesAsync();
    }

    public static async Task DeleteAsync(EfCoreContext context)
    {
        var book = context.Books.First(p => p.Title == "Quantum Networking" );
        context.Books.Remove(book);
        await context.SaveChangesAsync();
    }
    
    public static void EagerLoading(EfCoreContext  context)
    {
        // Eager loading of first book with the corresponding Reviews relationship
        var firstBook1 = context.Books
            .Include(p => p.Reviews)
            .FirstOrDefault();
        
        var firstBook2 = context.Books
            .Include(book => book.AuthorsLink)
                .ThenInclude(bookAuthor => bookAuthor.Author)
            .Include(book => book.Reviews)
            .Include(book => book.Tags) 
            .Include(book => book.Promotion) .
            FirstOrDefault();
        
        var firstBook3 = context.Books
            .Include(book => book.AuthorsLink
                .OrderBy(bookAuthor => bookAuthor.Order))
                .ThenInclude(bookAuthor => bookAuthor.Author) 
            .Include(book => book.Reviews
                .Where(review => review.NumStars == 5)) //load only Reviews in which NumStars is 5
            .Include(book => book.Promotion)
            .First();
    }

    public static void ExplicitLoading(EfCoreContext context)
    {
        var firstBook = context.Books.First();
        
        // Explicit loading of first book with the corresponding AuthorsLink relationship
        context.Entry(firstBook).
            Collection(p => p.AuthorsLink).Load();
        foreach (var authorLink in firstBook.AuthorsLink)
        {
            context.Entry(authorLink)
                .Reference(bookAuthor => bookAuthor.Author).Load();
        }
        
        context.Entry(firstBook).Collection(book => book.Tags).Load(); 
        context.Entry(firstBook).Reference(book => book.Promotion).Load();
        
        
        // count reviews for this book
        var numReviews = context.Entry(firstBook)
            .Collection(book => book.Reviews)
            .Query().Count();
        
        // get all the star ratings for the book
        var starRatings = context.Entry(firstBook)
            .Collection(book => book.Reviews)
            .Query().Select(review => review.NumStars)
            .ToList();
    }

    public static void SelectLoading(EfCoreContext context)
    {
        var books = context.Books
            .Select(book => new
            {
                book.Title, 
                book.Price, 
                NumReviews = book.Reviews.Count,
            } )
            .ToList();
    }

    public static void ClientVsServerEvaluation(EfCoreContext context)
    {
        // (a) extract all the authors’ names, in order, from the Authors table
        // (b) turn them into one string with commas between names.
        var firstBook = context.Books 
            .Select(book => new
            {
                book.BookId, 
                book.Title, 
                AuthorsString = string.Join(", ", 
                    book.AuthorsLink 
                        .OrderBy(ba => ba.Order) 
                        .Select(ba => ba.Author.Name))
            } ).First();
        Console.WriteLine($"{firstBook.BookId} - {firstBook.Title} - {firstBook.AuthorsString}");
    }
}