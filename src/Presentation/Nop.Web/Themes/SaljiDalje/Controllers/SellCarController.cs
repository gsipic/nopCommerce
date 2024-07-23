using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Themes.SaljiDalje.Controllers;

[Authorize]
public partial class SellCarController : BasePublicController
{
    public IActionResult Index()
    {
        return View("~/Themes/SaljiDalje/Views/SellCar/Index.cshtml");
        //return Ok("sdfsdfsdfdfdf");
    }
}