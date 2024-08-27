using Microsoft.AspNetCore.Mvc;
using Nop.Web.Controllers;
using Nop.Web.Models.Customer;

namespace Nop.Web.Themes.SaljiDalje.Controllers;

public sealed partial class CustomerController : BasePublicController
{
    public async Task<IActionResult> MyCars()
    {
        return View();
    }

    public async Task<IActionResult> WishList()
    {
        return View();
    }

    public async Task<IActionResult> Notifications()
    {
        return View();
    }
    
    public async Task<IActionResult> HelpCenter()
    {
        return View();
    }
}