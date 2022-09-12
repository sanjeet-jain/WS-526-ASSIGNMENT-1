using System.Diagnostics;
using ImageSharingWithUpload.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImageSharingWithUpload.Controllers;

public class HomeController : Controller
{
    protected void CheckAda()
    {
        var cookie = Request.Cookies["ADA"];
        if (cookie != null && "true".Equals(cookie))
            ViewBag.isADA = true;
        else
            ViewBag.isADA = false;
    }

    [HttpGet]
    public IActionResult Index(string id = "Stranger")
    {
        CheckAda();
        ViewBag.Title = "Welcome!";
        ViewBag.Id = id;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public IActionResult Privacy()
    {
        CheckAda();
        return View();
    }
}