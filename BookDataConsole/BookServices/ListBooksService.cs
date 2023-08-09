using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

namespace BookDataConsole.BookServices;

public class ListBooksService
{
    private readonly EfCoreContext _context;

    public ListBooksService(EfCoreContext context)
    {
        _context = context;
    }

    public IQueryable<BookListDto> SortFilterPage
        (SortFilterPageOptions options)
    {
        var booksQuery = _context.Books //#A
            .AsNoTracking() //#B
            .MapBookToDto() //#C
            .OrderBooksBy(options.OrderByOptions) //#D
            .FilterBooksBy(options.FilterBy, //#E
                options.FilterValue); //#E

        options.SetupRestOfDto(booksQuery); //#F

        // 这里EFCore应该缓存从数据库中读取的数据
        // 翻页的时候，显然还会调用一次这个函数。
        // 而上面显然是一次性把filter后的数据都读取出来
        // 所以不可能同样的数据再从读取一次吧
        // 如何判断EFCore是从缓存中读取的数据，还是从数据库中读取的？
        // LOg吗？
        return booksQuery.Page(options.PageNum - 1, //#G
            options.PageSize); //#G
    }
}

/*********************************************************
    #A This starts by selecting the Books property in the Application's DbContext 
    #B Because this is a read-only query I add .AsNoTracking(). It makes the query faster
    #C It then uses the Select query object which will pick out/calculate the data it needs
    #D It then adds the commands to order the data using the given options
    #E Then it adds the commands to filter the data
    #F This stage sets up the number of pages and also makes sure PageNum is in the right range
    #G Finally it applies the paging commands
* *****************************************************/