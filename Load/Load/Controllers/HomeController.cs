using Microsoft.AspNetCore.Mvc;

namespace Load.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}