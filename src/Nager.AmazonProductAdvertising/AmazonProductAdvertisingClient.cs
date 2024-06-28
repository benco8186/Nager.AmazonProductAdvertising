using Nager.AmazonProductAdvertising.Auth;
using Nager.AmazonProductAdvertising.Model;
using Nager.AmazonProductAdvertising.Model.Paapi;
using Nager.AmazonProductAdvertising.Model.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;

namespace Nager.AmazonProductAdvertising;

/// <summary>
/// Amazon Product Advertising Client
/// </summary>
public class AmazonProductAdvertisingClient
{
    private readonly RestClient HttpClient;
    private readonly string PartnerTag;
    private readonly string AccessKey;
    private readonly string SecretKey;
    private readonly JsonSerializerSettings JsonSerializerSettingsResponse;
    private readonly JsonSerializerSettings JsonSerializerSettingsRequest;
    private readonly AmazonEndpointConfig AmazonEndpointConfig;
    private readonly AmazonResourceValidator AmazonResourceValidator;
    private readonly AmazonLanguageValidator AmazonLanguageValidator;
    private readonly AmazonEndpoint AmazonEndpoint;
    private readonly IAwsSigner AwsSigner;
    private readonly string Path = "/paapi5/";
    private readonly string PartnerType = "Associates";
    private readonly string ServiceName = "ProductAdvertisingAPI";
    private readonly string Marketplace;

    /// <summary>
    /// Amazon Product Advertising Client
    /// </summary>
    /// <param name="awsSigner"></param>
    /// <param name="amazonEndpoint"></param>
    /// <param name="partnerTag"></param>
    /// <param name="accessKey"></param>
    /// <param name="secretKey"></param>
    /// <param name="userAgent"></param>
    /// <param name="strictJsonMapping"></param>
    public AmazonProductAdvertisingClient(IAwsSigner awsSigner, string accessKey, string secretKey, AmazonEndpoint amazonEndpoint, string partnerTag, string? userAgent = null, bool strictJsonMapping = false)
    {
        AwsSigner = awsSigner;
        AccessKey = accessKey;
        SecretKey = secretKey;
        HttpClient = new RestClient(new LoggingHandler(new HttpClientHandler()));
        // Causes problems with RestSharp 111.3.0: parameter already set 
        // HttpClient.AddDefaultHeader("User-Agent", userAgent ?? "Nager.AmazonProductAdvertising");

        AmazonEndpoint = amazonEndpoint;
        PartnerTag = partnerTag;

        JsonSerializerSettingsRequest = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new DefaultNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Ignore
        };

        JsonSerializerSettingsResponse = new JsonSerializerSettings
        {
            MissingMemberHandling = strictJsonMapping ? MissingMemberHandling.Error : MissingMemberHandling.Ignore
        };

        var amazonConfigEndpointConfigRepository = new AmazonEndpointConfigRepository();
        AmazonEndpointConfig = amazonConfigEndpointConfigRepository.Get(amazonEndpoint);

        AmazonResourceValidator = new AmazonResourceValidator();
        AmazonLanguageValidator = new AmazonLanguageValidator();
        Marketplace = $"www.{AmazonEndpointConfig.Host}";
    }

    /// <summary>
    /// Search items with a keyword
    /// </summary>
    /// <param name="keyword"></param>
    /// <returns></returns>
    public Task<SearchItemResponse> SearchItemsAsync(string keyword)
    {
        var resources = new[]
        {
            "BrowseNodeInfo.BrowseNodes",
            "BrowseNodeInfo.BrowseNodes.Ancestor",
            "BrowseNodeInfo.BrowseNodes.SalesRank",

            "Images.Primary.Small",
            "Images.Primary.Medium",
            "Images.Primary.Large",

            "Images.Variants.Small",
            "Images.Variants.Medium",
            "Images.Variants.Large",

            "ItemInfo.ByLineInfo",
            "ItemInfo.Classifications",
            "ItemInfo.ContentInfo",
            "ItemInfo.ContentRating",
            "ItemInfo.ExternalIds",
            "ItemInfo.Features",
            "ItemInfo.ProductInfo",
            "ItemInfo.TechnicalInfo",
            "ItemInfo.Title",
            "ItemInfo.TradeInInfo",

            "Offers.Listings.Availability.MinOrderQuantity",
            "Offers.Listings.Availability.MaxOrderQuantity",
            "Offers.Listings.Availability.Message",
            "Offers.Listings.Availability.Type",
            "Offers.Listings.Condition",
            "Offers.Listings.Condition.SubCondition",
            "Offers.Listings.DeliveryInfo.IsAmazonFulfilled",
            "Offers.Listings.DeliveryInfo.IsFreeShippingEligible",
            "Offers.Listings.DeliveryInfo.IsPrimeEligible",
            "Offers.Listings.IsBuyBoxWinner",
            "Offers.Listings.LoyaltyPoints.Points",
            "Offers.Listings.MerchantInfo",
            "Offers.Listings.Price",
            "Offers.Listings.ProgramEligibility.IsPrimeExclusive",
            "Offers.Listings.ProgramEligibility.IsPrimePantry",
            "Offers.Listings.Promotions",
            "Offers.Listings.SavingBasis",
            "Offers.Summaries.HighestPrice",
            "Offers.Summaries.LowestPrice",
            "Offers.Summaries.OfferCount",

            "ParentASIN",
            "SearchRefinements"
        };

        return SearchItemsAsync(resources, keyword);
    }

    /// <summary>
    /// Search items with a keyword
    /// </summary>
    /// <param name="resources"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    public async Task<SearchItemResponse> SearchItemsAsync(string[] resources, string keyword)
    {
        var request = new SearchRequest(keyword) { Resources = resources };
        return await SearchItemsAsync(request);
    }

    /// <summary>
    /// Search items with a search request
    /// </summary>
    /// <param name="searchRequest"></param>
    /// <returns></returns>
    public async Task<SearchItemResponse> SearchItemsAsync(SearchRequest searchRequest)
    {
        if (!AmazonResourceValidator.IsResourcesValid(searchRequest.Resources, "SearchItems"))
        {
            return new SearchItemResponse { Successful = false, ErrorMessage = "Resources has wrong values" };
        }

        var request = new SearchItemRequest(PartnerTag, PartnerType, Marketplace, keywords: searchRequest.Keywords)
        {
            Brand = searchRequest.Brand,
            Title = searchRequest.Title,
            Author = searchRequest.Author,
            Actor = searchRequest.Actor,
            Artist = searchRequest.Artist,
            Resources = searchRequest.Resources,
            ItemPage = searchRequest.ItemPage,
            SortBy = searchRequest.SortBy,
            BrowseNodeId = searchRequest.BrowseNodeId,
            Merchant = searchRequest.Merchant,
            SearchIndex = searchRequest.SearchIndex,
            Condition = searchRequest.Condition
        };

        var json = JsonConvert.SerializeObject(request, JsonSerializerSettingsRequest);
        if (string.IsNullOrEmpty(json))
        {
            return new SearchItemResponse { Successful = false, ErrorMessage = "Cannot serialize object" };
        }

        var response = await RequestAsync("SearchItems", json);
        SearchItemResponse? test = null;

        try
        {
            test = DeserializeObject<SearchItemResponse>(response);
        }
        catch (Exception ex)
        {
            var z = ex.Message;
            throw;
        }

        return test;
    }

    /// <summary>
    /// Get items via id
    /// </summary>
    /// <param name="itemIds"></param>
    /// <returns></returns>
    public Task<GetItemsResponse> GetItemsAsync(params string[] itemIds)
    {
        var resources = new[]
        {
            "BrowseNodeInfo.BrowseNodes",
            "BrowseNodeInfo.BrowseNodes.Ancestor",
            "BrowseNodeInfo.BrowseNodes.SalesRank",

            "Images.Primary.Small",
            "Images.Primary.Medium",
            "Images.Primary.Large",

            "Images.Variants.Small",
            "Images.Variants.Medium",
            "Images.Variants.Large",

            "ItemInfo.ByLineInfo",
            "ItemInfo.Classifications",
            "ItemInfo.ContentInfo",
            "ItemInfo.ContentRating",
            "ItemInfo.ExternalIds",
            "ItemInfo.Features",
            "ItemInfo.ManufactureInfo",
            "ItemInfo.ProductInfo",
            "ItemInfo.TechnicalInfo",
            "ItemInfo.Title",
            "ItemInfo.TradeInInfo",

            "Offers.Listings.Availability.MinOrderQuantity",
            "Offers.Listings.Availability.MaxOrderQuantity",
            "Offers.Listings.Availability.Message",
            "Offers.Listings.Availability.Type",
            "Offers.Listings.Condition",
            "Offers.Listings.Condition.SubCondition",
            "Offers.Listings.DeliveryInfo.IsAmazonFulfilled",
            "Offers.Listings.DeliveryInfo.IsFreeShippingEligible",
            "Offers.Listings.DeliveryInfo.IsPrimeEligible",
            "Offers.Listings.LoyaltyPoints.Points",
            "Offers.Listings.MerchantInfo",
            "Offers.Listings.Price",
            "Offers.Listings.ProgramEligibility.IsPrimeExclusive",
            "Offers.Listings.ProgramEligibility.IsPrimePantry",
            "Offers.Listings.Promotions",
            "Offers.Listings.SavingBasis",
            "Offers.Summaries.HighestPrice",
            "Offers.Summaries.LowestPrice",
            "Offers.Summaries.OfferCount",

            "ParentASIN"
        };

        return GetItemsAsync(resources, itemIds);
    }

    /// <summary>
    /// Get items via id
    /// </summary>
    /// <param name="resources"></param>
    /// <param name="itemIds"></param>
    /// <returns></returns>
    public async Task<GetItemsResponse> GetItemsAsync(string[] resources, params string[] itemIds)
    {
        var request = new ItemsRequest(itemIds) { Resources = resources };
        return await GetItemsAsync(request);
    }

    /// <summary>
    /// Get items with a item request
    /// </summary>
    /// <param name="itemsRequest"></param>
    /// <returns></returns>
    public async Task<GetItemsResponse> GetItemsAsync(ItemsRequest itemsRequest)
    {
        if (!AmazonResourceValidator.IsResourcesValid(itemsRequest.Resources, "GetItems"))
        {
            return new GetItemsResponse { Successful = false, ErrorMessage = "Resources has wrong values" };
        }

        var request = new GetItemsRequest(itemsRequest.ItemIds, PartnerTag, PartnerType, Marketplace)
        {
            Resources = itemsRequest.Resources,
            Merchant = itemsRequest.Merchant,
            Condition = itemsRequest.Condition,
        };

        var json = JsonConvert.SerializeObject(request, JsonSerializerSettingsRequest);
        if (string.IsNullOrEmpty(json))
        {
            return new GetItemsResponse { Successful = false, ErrorMessage = "Cannot serialize object" };
        }

        var response = await RequestAsync("GetItems", json);
        return DeserializeObject<GetItemsResponse>(response);
    }

    /// <summary>
    /// Get variations via asin
    /// </summary>
    /// <param name="asin"></param>
    /// <returns></returns>
    public Task<GetVariationsResponse> GetVariationsAsync(string asin)
    {
        var resources = new[]
        {
            "ItemInfo.Title",

            "VariationSummary.VariationDimension",

            "Images.Primary.Small",
            "Images.Primary.Medium",
            "Images.Primary.Large",

            "Images.Variants.Small",
            "Images.Variants.Medium",
            "Images.Variants.Large"
        };

        return GetVariationsAsync(resources, asin);
    }

    /// <summary>
    /// Get variations via asin
    /// </summary>
    /// <param name="resources"></param>
    /// <param name="asin"></param>
    /// <returns></returns>
    public async Task<GetVariationsResponse> GetVariationsAsync(string[] resources, string asin)
    {
        var request = new VariationsRequest(asin) { Resources = resources };
        return await GetVariationsAsync(request);
    }

    /// <summary>
    /// Get variations via asin
    /// </summary>
    /// <param name="variationsRequest"></param>
    /// <returns></returns>
    public async Task<GetVariationsResponse> GetVariationsAsync(VariationsRequest variationsRequest)
    {
        var request = new GetVariationsRequest(variationsRequest.Asin, PartnerTag, PartnerType, Marketplace)
        {
            Merchant = variationsRequest.Merchant,
            Resources = variationsRequest.Resources
        };

        if (!AmazonResourceValidator.IsResourcesValid(request.Resources, "GetVariations"))
        {
            return new GetVariationsResponse { Successful = false, ErrorMessage = "Resources has wrong values" };
        }

        var json = JsonConvert.SerializeObject(request, JsonSerializerSettingsRequest);
        if (string.IsNullOrEmpty(json))
        {
            return new GetVariationsResponse { Successful = false, ErrorMessage = "Cannot serialize object" };
        }

        var response = await RequestAsync("GetVariations", json);
        return DeserializeObject<GetVariationsResponse>(response);
    }

    /// <summary>
    /// Get browseNodes via browseNodeIds
    /// </summary>
    /// <param name="browseNodeIds"></param>
    /// <returns>GetBrowseNodesResponse</returns>
    public Task<GetBrowseNodesResponse> GetBrowseNodesAsync(string[] browseNodeIds)
    {
        var resources = new[]
        {
            "BrowseNodes.Ancestor",
            "BrowseNodes.Children"
        };

        return GetBrowseNodesAsync(resources, browseNodeIds);
    }

    /// <summary>
    /// Get browseNodes via browseNodeIds
    /// </summary>
    /// <param name="resources"></param>
    /// <param name="browseNodeIds"></param>
    /// <returns>GetBrowseNodesResponse</returns>
    public async Task<GetBrowseNodesResponse> GetBrowseNodesAsync(string[] resources, string[] browseNodeIds)
    {
        var request = new BrowseNodesRequest(browseNodeIds) { Resources = resources };
        return await GetBrowseNodesAsync(request);
    }

    /// <summary>
    /// Get browseNode with a browseNodesRequest
    /// </summary>
    /// <param name="browseNodesRequest"></param>
    /// <returns>GetBrowseNodesResponse</returns>
    public async Task<GetBrowseNodesResponse> GetBrowseNodesAsync(BrowseNodesRequest browseNodesRequest)
    {
        var request = new GetBrowseNodesRequest(browseNodesRequest.BrowseNodeIds, PartnerTag, PartnerType, Marketplace)
        {
            LanguagesOfPreference = browseNodesRequest.LanguagesOfPreference,
            Resources = browseNodesRequest.Resources
        };

        if (!AmazonResourceValidator.IsResourcesValid(request.Resources, "GetBrowseNodes"))
        {
            return new GetBrowseNodesResponse { Successful = false, ErrorMessage = "Resources has wrong values" };
        }

        if (!AmazonLanguageValidator.IsLanguageValid(request.LanguagesOfPreference, AmazonEndpoint))
        {
            return new GetBrowseNodesResponse { Successful = false, ErrorMessage = "LanguagesOfPreference contains a language that is not available for this endpoint" };
        }

        var json = JsonConvert.SerializeObject(request, JsonSerializerSettingsRequest);
        if (string.IsNullOrEmpty(json))
        {
            return new GetBrowseNodesResponse { Successful = false, ErrorMessage = "Cannot serialize object" };
        }

        var response = await RequestAsync("GetBrowseNodes", json);
        return DeserializeObject<GetBrowseNodesResponse>(response);
    }

    private T DeserializeObject<T>(HttpResponse response) where T : AmazonResponse
    {
        var amazonResponse = JsonConvert.DeserializeObject<T>(response.Content, JsonSerializerSettingsResponse);
        amazonResponse.Successful = response.Successful;
        if (amazonResponse.Errors != null)
        {
            amazonResponse.Successful = false;
            amazonResponse.ErrorMessage = string.Join(Environment.NewLine, amazonResponse.Errors.Select(o => o.Message));
        }

        return amazonResponse;
    }

    private async Task<HttpResponse> RequestAsync(string type, string json)
    {
        var host = $"webservices.{AmazonEndpointConfig.Host}";
        var date = DateTime.UtcNow;
        var amzTarget = $"com.amazon.paapi5.v1.ProductAdvertisingAPIv1.{type}";

        var headerToSign = new Dictionary<string, string>
        {
            { "Content-Encoding", "amz-1.0" },
            { "Host", host },
            { "X-Amz-Target", amzTarget },
            { "X-Amz-Date", date.ToAmzDateStr() },
        };
        var authorization = AwsSigner.CreateAuthorizationHeader(date, AccessKey, SecretKey, headerToSign, "POST", $"{Path}{type.ToLower()}", json, AmazonEndpointConfig.Region, ServiceName);

        var requestUri = $"https://webservices.{AmazonEndpointConfig.Host}{Path}{type.ToLower()}";

        var request = new RestRequest(new Uri(requestUri), Method.Post);
        request.AddJsonBody(json);
        request.AddHeader("Authorization", authorization);
        request.AddHeader("Content-Encoding", "amz-1.0");
        request.AddHeader("Host", host);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("x-Amz-Target", amzTarget);
        request.AddHeader("x-Amz-Date", date.ToAmzDateStr());

        var responseMessage = await HttpClient.ExecuteAsync(request);
        //System.IO.File.WriteAllText($"{DateTime.Now:hhmmssfff}.json", content);
        return new HttpResponse { Successful = responseMessage.IsSuccessStatusCode, StatusCode = responseMessage.StatusCode, Content = responseMessage.Content ?? "" };
    }
}
