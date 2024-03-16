using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Models.DTO;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
// [ApiVersion("1.0")]
// [ApiVersion("2.0")]
public class VillaController : ControllerBase
{
    private readonly IVillaRepository _villaRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly APIResponse _response;

    public VillaController(IVillaRepository villaRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment)
    {
        _villaRepository = villaRepository;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
        _response = new APIResponse();
    }

    [HttpGet("Exception")]
    public IActionResult Error()
    {
        throw new BadImageFormatException();
    }

    [HttpGet]
    [Route("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        try
        {
            var villa = await _villaRepository.GetAsync(id);
            if (villa is null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages = new List<string>() { "No villa with the provided ID" };
                return NotFound(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = villa;
            _response.Success = true;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.ErrorMessages = new List<string>() { e.ToString() };
            return BadRequest(_response);
        }
    }

    // [MapToApiVersion("2.0")]
    // [HttpGet]
    // public IActionResult GetAll2()
    // {
    // return Ok(new List<String>() { "sads", "sad" });
    // }


    [ResponseCache(Duration = 30)]
    // [MapToApiVersion("1.0")]
    [HttpGet]
    // [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        // IEnumerable<Villa> villas = await _context.Villas.ToListAsync();
        // return Ok(_mapper.Map<List<VillaDTO>>(villas));
        try
        {
            var villas = await _villaRepository.GetAllAsync();
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = villas;
            _response.Success = true;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.ErrorMessages = new List<string>() { e.ToString() };
            return BadRequest(_response);
        }
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateVilla([FromForm] VillaDTO villaDto)
    {
        try
        {
            if (villaDto is null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { "Villa is null" };
                return BadRequest(_response);
            }

            if (await _villaRepository.GetAsync(v => v.Name == villaDto.Name) is not null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { "There is a Villa with the same name" };
                return BadRequest(_response);
            }

            var ImgUrl = "";
            if (villaDto.Image is not null && villaDto.Image.Length > 0)
            {
                var FilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", villaDto.Image.FileName);
                if (System.IO.File.Exists(FilePath)) System.IO.File.Delete(FilePath);
                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    await villaDto.Image.CopyToAsync(stream);
                }

                ImgUrl = FilePath;
            }


            var villa = new Villa() { Name = villaDto.Name, Details = villaDto.Details };
            if (!string.IsNullOrEmpty(ImgUrl))
            {
                villa.ImageUrl = Path.Combine(HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value,
                                                    HttpContext.Request.PathBase.Value,
                                                    "Images", villaDto.Image.FileName);
            }
            await _villaRepository.AddAsync(villa);
            _response.StatusCode = HttpStatusCode.Created;
            _response.Result = _mapper.Map<VillaDTO>(villa);
            _response.Success = true;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.ErrorMessages = new List<string>() { e.ToString() };
            return BadRequest(_response);
        }
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    [Route("{id}")]
    public async Task<IActionResult> update([FromRoute] int id,[FromForm] VillaDTO villaDto)
    {
        try
        {
            if (villaDto is null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { "Villa is null" };
                return BadRequest(_response);
            }

            var v = await _villaRepository.GetAsync(v => v.Name == villaDto.Name);
            if (v is not null && v.ID != villaDto.ID)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { "There is a Villa with the same name" };
                return BadRequest(_response);
            }

            var villa = await _villaRepository.GetAsync(id);
            if (villa is null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { "No villa with the provided ID" };
                return NotFound(_response);
            }

            villa.Name = villaDto.Name;
            villa.Details = villaDto.Details;
            // villa.ImageUrl = villaDto.ImageUrl;
            if (villaDto.Image is not null)
            {
                var file = new FileInfo(Path.Combine(_webHostEnvironment.WebRootPath, "Images",
                    villa.ImageUrl.Split('/').Last()));
                if (file.Exists)
                {
                    file.Delete();
                }

                var FilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", villaDto.Image.FileName);
                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    await villaDto.Image.CopyToAsync(stream);
                }

                villa.ImageUrl = Path.Combine(HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value,
                    HttpContext.Request.PathBase, "Images", villaDto.Image.FileName);
            }
            await _villaRepository.UpdateAsync(villa);
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = _mapper.Map<VillaDTO>(villa);
            _response.Success = true;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.ErrorMessages = new List<string>() { e.ToString() };
            return BadRequest(_response);
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Custom")]
    [Route("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var villa = await _villaRepository.GetAsync(id);
            if (villa is null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages = new List<string>() { "No villa with the provided ID" };
                return NotFound(_response);
            }

            var file = new FileInfo(Path.Combine(_webHostEnvironment.WebRootPath, "Images",
                villa.ImageUrl.Split('/').Last()));
            if (file.Exists)
            {
                file.Delete();
            }
            await _villaRepository.DeleteAsync(villa);
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = _mapper.Map<VillaDTO>(villa);
            _response.Success = true;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.ErrorMessages = new List<string>() { e.ToString() };
            return BadRequest(_response);
        }
    }
}