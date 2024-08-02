using BlazorApp1.Pages;
using Nop.Core.Infrastructure;
using Nop.Web.Themes.SaljiDalje.Data;

namespace Nop.Web.Themes.SaljiDalje.InfraStructure;

public class SaljiDaljeStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISaljiDaljeClient, SaljiDaljeClientServer>();
    }

    public void Configure(IApplicationBuilder application)
    {
        
    }

    public int Order { get; } = int.MaxValue;
}