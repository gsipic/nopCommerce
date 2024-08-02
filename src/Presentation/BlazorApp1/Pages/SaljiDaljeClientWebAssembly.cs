namespace BlazorApp1.Pages;

public class SaljiDaljeClientWebAssembly : ISaljiDaljeClient
{
    public HttpClient HttpClient { get; set; }

    public SaljiDaljeClientWebAssembly(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }
}