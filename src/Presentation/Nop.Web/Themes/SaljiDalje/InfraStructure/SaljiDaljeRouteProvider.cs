using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Web.Themes.SaljiDalje.InfraStructure;

public class SaljiDaljeRouteProvider : BaseRouteProvider, IRouteProvider
{
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var lang = GetLanguageRoutePattern();
        //customer account links
        endpointRouteBuilder.MapControllerRoute(name: "CustomerMyCars",
            pattern: $"{lang}/customer/mycars",
            defaults: new { controller = "Customer", action = "MyCars" });
        
        endpointRouteBuilder.MapControllerRoute(name: "CustomerWishList",
            pattern: $"{lang}/customer/wishlist",
            defaults: new { controller = "Customer", action = "WishList" });
        
        endpointRouteBuilder.MapControllerRoute(name: "CustomerNotifications",
            pattern: $"{lang}/customer/notifications",
            defaults: new { controller = "Customer", action = "Notifications" });
        
        endpointRouteBuilder.MapControllerRoute(name: "CustomerHelpDesk",
            pattern: $"{lang}/customer/helpdesk",
            defaults: new { controller = "Customer", action = "HelpCenter" });
        
        endpointRouteBuilder.MapControllerRoute(name: "SellCar",
            pattern: $"{lang}/sellcar",
            defaults: new { controller = "SellCar", action = "Index" });
    }

    public int Priority { get; } = 1;
}