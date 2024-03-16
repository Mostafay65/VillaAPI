using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VillaMVC.Models;
using VillaMVC.Models.DTO;
using VillaMVC.Service.IService;
using VillaUtility;


namespace VillaMVC.Controllers;

// [Authorize(Roles = "Admin")]
public class VillaController : Controller
{
    private readonly IVillaService _villaService;
    private readonly ITokenProvider _tokenProvider;

    public VillaController(IVillaService villaService, ITokenProvider tokenProvider)
    {
        _villaService = villaService;
        _tokenProvider = tokenProvider;
    }
    // GET
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        List<VillaDTO> res = null;
        APIResponse response = await _villaService.GetAll<APIResponse>();
        if (response.Result is not null)
        {
            res = JsonConvert.DeserializeObject<List<VillaDTO>>(response.Result.ToString());
        }
        return View(res);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Custom")]
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(VillaDTO villa)
    {
        if (ModelState.IsValid)
        {
            villa.ID = 1;
            await _villaService.Create<APIResponse>(villa);
            TempData["Success"] = "Villa Created Successfully";
            return RedirectToAction("Index");
        }
        TempData["Error"] = "There are some error creating the villa";
        return View(villa);
    }
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id)
    {
        APIResponse response = await _villaService.Get<APIResponse>(id);
        if (response.Result is null)
        {
            TempData["NotValidId"] = "<strong>Oh snap!</strong> " + response.ErrorMessages;
            return RedirectToAction("Index");
        }
        return View(JsonConvert.DeserializeObject<VillaDTO>(response.Result.ToString()));
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromForm] VillaDTO villa)
    {
        if (ModelState.IsValid)
        {
            APIResponse response = await _villaService.update<APIResponse>(id, villa);
            if (response.Result is not null)
            {
                TempData["Success"] = "Villa Updated Successfully";
                return RedirectToAction("Index");
            }    
            ModelState.AddModelError("", response.ErrorMessages[0]);
        }
        return View(villa);
    }

    [HttpGet]
    [Authorize(Roles = "Custom")]
    public async Task<IActionResult> Delete(int id)
    {
        await _villaService.Delete<APIResponse>(id);
        TempData["Error"] = "Villa Deleted Successfully";
        return RedirectToAction("Index");
    }
}