using System.Net.Http.Headers;
using FacebookLoader.Common;
using Newtonsoft.Json.Linq;

namespace FacebookLoader.Loader;


public abstract class FacebookLoaderBase
{
    public FacebookParameters FacebookParameters { get; }
    public ILogging Logger { get; }

    public FacebookLoaderBase(FacebookParameters facebookParameters, ILogging logger)
    {
        this.FacebookParameters = facebookParameters;
        this.Logger = logger;
    }

    protected async Task<JObject> CallGraphApiAsync(string url)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        try
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseContent);
        }
        catch (HttpRequestException httpEx) when (httpEx.StatusCode.HasValue)
        {
            var responseText = httpEx.Message;
            Console.Error.WriteLine($"HTTP error occurred: {httpEx} while calling graph api");
            throw new FacebookHttpException((int)httpEx.StatusCode.Value, responseText);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"An error occurred: {ex} while calling graph api");
            throw new Exception($"Other error occurred: {ex.Message}");
        }
    }

    public static string ExtractString(JObject obj, string tag)
    {
        JToken value;
        if (!obj.TryGetValue(tag, out value)) return string.Empty;
        return value.ToString() ?? string.Empty;
    }

    public static bool ExtractBoolean(JObject obj, string tag)
    {
        JToken value;
        if (!obj.TryGetValue(tag, out value)) return false;
        return value.ToObject<bool>();
    }

    public static JObject ExtractObject(JObject obj, string tag)
    {
        JToken value;
        if (!obj.TryGetValue(tag, out value)) return null;
        return value as JObject;
    }

    public static List<JObject> ExtractObjectArray(JObject obj, string tag)
    {
        JToken arrayToken;
        if (!obj.TryGetValue(tag, out arrayToken) || !(arrayToken is JArray arrayElement))
            return new List<JObject>();

        var list = new List<JObject>();
        foreach (var element in arrayElement)
        {
            list.Add(element as JObject);
        }
        return list;
    }
}
