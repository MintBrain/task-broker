using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

public class TaskController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}