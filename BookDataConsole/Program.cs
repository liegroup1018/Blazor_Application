// See https://aka.ms/new-console-template for more information


using BookDataConsole;
using BookDataConsole.Service;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

String JsonPathName = "seedData/Apress books, dollar converted at 1.2, roundup to 5 dollars - 16-12-06 12-10-46.json";

/*
var books = BookJsonLoader.LoadBooks(JsonPathName);
Console.WriteLine($"Loaded {books.Count()} books");

Console.WriteLine("=======");

Console.WriteLine($"Loaded {books.Count(e=>e.Description==null)} books without descriptions");

Console.WriteLine("=======");

Console.WriteLine($"Loaded {books.Count(e=>e.Description!=null)} books with descriptions");
*/

var connectionString = "server=localhost;database=BookSite;user=root;password=111111";
var serverVersion = new MySqlServerVersion(new Version(5, 7, 33));

var optionalBuilder = new DbContextOptionsBuilder<EfCoreContext>();
optionalBuilder.UseMySql(connectionString, serverVersion)
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors();
var options = optionalBuilder.Options;

using (var context = new EfCoreContext(options))
{
    //初始数据填入数据库
    //await context.SeedDatabaseIfNoBooksAsync(JsonPathName);
    
    // 插入数据
    //await OperationOfDB.Insert(context);
    
    // 打印出全部数据, 不包括关系
    //await OperationOfDB.SearchAll(context);

    //OperationOfDB.SearchPartion1(context);
    
    //OperationOfDB.SearchPartion2(context);
    
    // await  OperationOfDB.UpdateAsync(context);
    
    // await OperationOfDB.DeleteAsync(context);
    
    //OperationOfDB.EagerLoading(context);
    
    //OperationOfDB.ClientVsServerEvaluation(context);


}

//Console.WriteLine("Done");