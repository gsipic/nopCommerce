using BlazorApp1.Pages;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Themes.SaljiDalje.Controllers;
using Nop.Web.Themes.SaljiDalje.Data;
using CustomerController = Nop.Web.Controllers.CustomerController;

namespace Nop.Web.Themes.SaljiDalje.InfraStructure;

public class SaljiDaljeStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISaljiDaljeClient, SaljiDaljeClientServer>();
        services.AddScoped<CustomerController, CustomCustomerController>();
        services.AddScoped<IProductService, CostumeProductService>();
        services.AddScoped<ICatalogModelFactory, CostumeCatalogModelFactory>();
        services.AddScoped<CatalogController, CostumeCatalogController>();
    }

    public void Configure(IApplicationBuilder application)
    {
        
    }

    public int Order { get; } = int.MaxValue;
}