namespace BookDataConsole.Service;

public class BookInfoJson
{
    public string? title { get; set; }

    public List<string>? authors { get; set; }

    public string? publisher { get; set; }

    public string? publishedDate { get; set; }

    public string? description { get; set; }

    public int pageCount { get; set; }

    public List<string> categories { get; set; }

    public double? averageRating { get; set; } // 平均评分

    public int? ratingsCount { get; set; } // 有多少人评分

    public string? imageLinksThumbnail { get; set; }

    public string? saleInfoCountry { get; set; }

    public bool? saleInfoForSale { get; set; }

    public string? saleInfoBuyLink { get; set; }

    public double? saleInfoListPriceAmount { get; set; }

    public string? saleInfoListPriceCurrencyCode { get; set; }
}