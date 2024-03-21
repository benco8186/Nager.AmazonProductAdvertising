namespace Nager.AmazonProductAdvertising.Model
{
    public class BrowseNodesRequest
    {
        public string[] BrowseNodeIds { get; set; }
        public LanguageCodes[]? LanguagesOfPreference { get; set; }
        public string[]? Resources { get; set; } = Array.Empty<string>();
        public BrowseNodesRequest(string[] browseNodeIds)
        {
            BrowseNodeIds = browseNodeIds;
        }
    }
}