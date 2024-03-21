namespace Nager.AmazonProductAdvertising.Model
{
    public class SearchRequest
    {
        public string? Keywords { get; set; }
        public string? Brand { get; set; }
        public string? Title { get; set; }
        public string? Actor { get; set; }
        public string? Artist { get; set; }
        public string? Author { get; set; }
        public int? ItemPage { get; set; }
        public SortBy? SortBy { get; set; }
        public string? BrowseNodeId { get; set; }
        public string[]? Resources { get; set; } = Array.Empty<string>();
        public Merchant? Merchant { get; set; }
        public SearchIndex? SearchIndex { get; set; }
        public Condition? Condition { get; set; }
        public SearchRequest(string? keywords = null, string? brand = null, string? title = null, SearchIndex? searchIndex = null, string? actor = null, string? artist = null, string? author = null, string? browseNodeId = null)
        {
            Keywords = keywords;
            Brand = brand;
            Title = title;
            SearchIndex = searchIndex;
            Actor = actor;
            Artist = artist;
            Author = author;
            BrowseNodeId = browseNodeId;
            if(keywords is null && brand is null && title is null && searchIndex is null && actor is null && artist is null && author is null && browseNodeId is null)
            {
                throw new ArgumentNullException("At least one of Actor, Artist, Author, Brand, BrowseNodeId, Keywords, SearchIndex, Title should be provided.");
            }

        }

    }
}
