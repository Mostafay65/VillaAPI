using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VillaMVC.Models;
using VillaMVC.Models.DTO;
using VillaMVC.Service.IService;
using VillaUtility;

namespace VillaMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IVillaService _villaService;

    public HomeController(ILogger<HomeController> logger, IVillaService villaService)
    {
        _logger = logger;
        _villaService = villaService;
    }
    
    
    public async Task<IActionResult> Index()
    {
        APIResponse response =  await _villaService.GetAll<APIResponse>();
        if (!response.Success)
        {
            return Json(response.ErrorMessages);
        }
        List<VillaDTO> res = JsonConvert.DeserializeObject<List<VillaDTO>>(response.Result.ToString());
        return View(res);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}