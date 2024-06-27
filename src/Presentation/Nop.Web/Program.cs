using Autofac.Extensions.DependencyInjection;
using BlazorApp1;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;
using Westwind.AspNetCore.LiveReload;

namespace Nop.Web;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile(NopConfigurationDefaults.AppSettingsFilePath, true, true);
        if (!string.IsNullOrEmpty(builder.Environment?.EnvironmentName))
        {
            var path = string.Format(NopConfigurationDefaults.AppSettingsEnvironmentFilePath, builder.Environment.EnvironmentName);
            builder.Configuration.AddJsonFile(path, true, true);
        }
        builder.Configuration.AddEnvironmentVariables();

        //load application settings
        builder.Services.ConfigureApplicationSettings(builder);

        var appSettings = Singleton<AppSettings>.Instance;
        var useAutofac = appSettings.Get<CommonConfig>().UseAutofac;

        if (useAutofac)
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        else
            builder.Host.UseDefaultServiceProvider(options =>
            {
                //we don't validate the scopes, since at the app start and the initial configuration we need 
                //to resolve some services (registered as "scoped") through the root container
                options.ValidateScopes = false;
                options.ValidateOnBuild = true;
            });

        //add services to the application and configure service provider
        builder.Services.ConfigureApplicationServices(builder);
        builder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();
            
        builder.Services.AddLiveReload(config =>
        {
            config.LiveReloadEnabled = builder.Environment.IsDevelopment();
            config.ClientFileExtensions = ".cshtml,.css,.js,.htm,.html,.ts,.razor,.cs"; // track also cs file
            config.FolderToMonitor = "/Users/goransipic/Developer/RiderProjects/nopCommerce/src";
        });

        var app = builder.Build();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            //.AddAdditionalAssemblies(typeof(BlazorApp1._Imports).Assembly);
        ;
        //configure the application HTTP request pipeline
        app.ConfigureRequestPipeline();
        await app.StartEngineAsync();

        await app.RunAsync();
    }
}