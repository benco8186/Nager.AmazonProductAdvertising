namespace Nager.AmazonProductAdvertising.Model
{
    public class VariationsRequest
    {
        public string Asin { get; set; }
        public string[]? Resources { get; set; } = Array.Empty<string>();
        public Merchant? Merchant { get; set; }
        public VariationsRequest(string asin)
        {
            Asin = asin;
        }

    }
}
