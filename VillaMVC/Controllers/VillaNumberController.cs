// using ASP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using VillaMVC.Models;
using VillaMVC.Models.DTO;
using VillaMVC.Service.IService;
using VillaUtility;

namespace VillaMVC.Controllers;

public class VillaNumberController : Controller
{
    private readonly IVillaNumberService _villaNumberService;
    private readonly IVillaService _villaService;
    private readonly ITokenProvider _tokenProvider;

    public VillaNumberController(IVillaNumberService villaNumberService, IVillaService villaService, ITokenProvider tokenProvider)
    {
        _villaNumberService = villaNumberService;
        _villaService = villaService;
        _tokenProvider = tokenProvider;
    }
    // GET
    public async Task<IActionResult> Index()
    {
        var response = await _villaNumberService.GetAll<APIResponse>();
        var res = JsonConvert.DeserializeObject<List<VillaNumberDto>>(response.Result.ToString());
        return View(res);
    }
    

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        APIResponse response = await _villaService.GetAll<APIResponse>();
        List<VillaDTO> villaDtos = JsonConvert.DeserializeObject<List<VillaDTO>>(response.Result.ToString());
        var selectlest = new SelectList(villaDtos, "ID", "Name");
        TempData["Villas"] = selectlest; 
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(VillaNumberDto villaNumberDto)
    {
        if (ModelState.IsValid)
        {
            var response = await _villaNumberService.Create<APIResponse>(villaNumberDto);
            if (response is not null && response.Success)
            {
                TempData["Success"] = "Villa Created Successfully";
                return RedirectToAction("Index");    
            }
            ModelState.AddModelError("", response.ErrorMessages[0]);
        }
         var Response = await _villaService.GetAll<APIResponse>();
        List<VillaDTO> villaDtos = JsonConvert.DeserializeObject<List<VillaDTO>>(Response.Result.ToString());
        var selectlest = new SelectList(villaDtos, "ID", "Name");
        TempData["Villas"] = selectlest; 
        TempData["Error"] = "There are some error creating the villa";
        return View(villaNumberDto);
    }

    
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        APIResponse response = await _villaService.GetAll<APIResponse>();
        List<VillaDTO> villaDtos = JsonConvert.DeserializeObject<List<VillaDTO>>(response.Result.ToString());
        var selectlest = new SelectList(villaDtos, "ID", "Name");
        TempData["Villas"] = selectlest;
        APIResponse response2 = await _villaNumberService.Get<APIResponse>(id);
        VillaNumberDto villaNumberDto = JsonConvert.DeserializeObject<VillaNumberDto>(response2.Result.ToString());
        return View(villaNumberDto);
    }
    [HttpPost]
    public async Task<IActionResult> Update(VillaNumberDto villaNumberDto, int id)
    {
        if (ModelState.IsValid)
        {
            var response = await _villaNumberService.Update<APIResponse>(id, villaNumberDto);
            if (response is not null && response.Success)
            {
                // var res = JsonConvert.DeserializeObject<VillaNumberDto>(response.Result.ToString());
                TempData["Success"] = "Villa Updated Successfully";
                return RedirectToAction("Index");    
            }
            ModelState.AddModelError("", response.ErrorMessages[0]);
        }
        var Response = await _villaService.GetAll<APIResponse>();
        List<VillaDTO> villaDtos = JsonConvert.DeserializeObject<List<VillaDTO>>(Response.Result.ToString());
        var selectlest = new SelectList(villaDtos, "ID", "Name");
        TempData["Villas"] = selectlest; 
        return View(villaNumberDto);
    }

    
    public async Task<IActionResult> Delete(int id)
    {
        await _villaNumberService.Delete<APIResponse>(id);
        TempData["Error"] = "Villa Deleted Successfully";
        return RedirectToAction("Index");
    }
}