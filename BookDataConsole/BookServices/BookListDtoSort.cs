using System.ComponentModel.DataAnnotations;

namespace BookDataConsole.BookServices;

public enum OrderByOptions
{
    [Display(Name = "sort by...")] SimpleOrder = 0,
    [Display(Name = "Votes ↑")] ByVotes,
    [Display(Name = "Publication Date ↑")] ByPublicationDate,
    [Display(Name = "Price ↓")] ByPriceLowestFirst,
    [Display(Name = "Price ↑")] ByPriceHigestFirst
}


// Sorting books by orderByOptions
public static class BookListDtoSort
{
    public static IQueryable<BookListDto> OrderBooksBy
    (this IQueryable<BookListDto> books,
        OrderByOptions orderByOptions)
    {
        switch (orderByOptions)
        {
            case OrderByOptions.SimpleOrder: //#A
                return books.OrderByDescending( //#A
                    x => x.BookId); //#A
            case OrderByOptions.ByVotes: //#B
                return books.OrderByDescending(x => //#B
                    x.ReviewsAverageVotes); //#B
            case OrderByOptions.ByPublicationDate: //#C
                return books.OrderByDescending( //#C
                    x => x.PublishedOn); //#C
            case OrderByOptions.ByPriceLowestFirst: //#D
                return books.OrderBy(x => x.ActualPrice); //#D
            case OrderByOptions.ByPriceHigestFirst: //#D
                return books.OrderByDescending( //#D
                    x => x.ActualPrice); //#D
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(orderByOptions), orderByOptions, null);
        }
    }

    /************************************************************
    #A Because of paging we always need to sort. I default to showing latest entries first
    #B This orders the book by votes. Books without any votes (null return) go at the bottom
    #C Order by publication date - latest books at the top
    #D Order by actual price, which takes into account any promotional price - both lowest first and highest first
     * ********************************************************/
}