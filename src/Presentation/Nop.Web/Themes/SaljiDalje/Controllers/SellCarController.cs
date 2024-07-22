using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Controllers;

namespace Nop.Web.Themes.SaljiDalje.Controllers;

public partial class SellCarController : BasePublicController
{
    public IActionResult Index()
    {
        return View("~/Themes/SaljiDalje/Views/SellCar/Index.cshtml");
        //return Ok("sdfsdfsdfdfdf");
    }
}