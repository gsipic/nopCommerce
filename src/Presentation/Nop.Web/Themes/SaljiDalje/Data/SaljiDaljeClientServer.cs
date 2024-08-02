using BlazorApp1.Pages;

namespace Nop.Web.Themes.SaljiDalje.Data;

public class SaljiDaljeClientServer : ISaljiDaljeClient
{
    public HttpClient HttpClient { get; set; }

    public SaljiDaljeClientServer(IHttpContextAccessor httpContextAccessor)
    {
        var foo = httpContextAccessor.HttpContext.Request.Cookies;
        string bar = "";
        foreach (var fooKey in foo.Keys)
        {
            foo.TryGetValue(fooKey, out var value);
            bar += fooKey + "=" + value + ";";
        }
        HttpClient = new HttpClient(new HttpClientHandler { UseCookies = false });
        HttpClient.DefaultRequestHeaders.Add("Cookie", bar);
    }
}