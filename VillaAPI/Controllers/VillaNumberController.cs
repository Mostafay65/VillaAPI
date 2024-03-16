using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VillaAPI.Models;
using VillaAPI.Models.DTO;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaNumberController : ControllerBase
    {
        
        private readonly APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IVillaNumberRepository _villaNumberRepository;
        private readonly IVillaRepository _villaRepository;

        public VillaNumberController(IMapper mapper, IVillaNumberRepository villaNumberRepository, IVillaRepository villaRepository)
        {
            _mapper = mapper;
            _response = new APIResponse();
            _villaNumberRepository = villaNumberRepository;
            _villaRepository = villaRepository;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                VillaNumber villaNumber= await _villaNumberRepository.GetAsync(id);
                if (villaNumber is null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "No Villa With the provided ID" };
                    return BadRequest(_response);
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<VillaNumberDto>(villaNumber);
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { e.ToString() };
                return BadRequest(_response);
            }
        }
        
        [HttpGet]
        // [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            // List<VillaNumber> villas = await _villaNumberRepository.GetAllAsync();
            // return Ok(_mapper.Map<List<VillaNumberDto>>(villas));
            try
            {
                var villaNumbers = await _villaNumberRepository.GetAllAsync(Includes : new List<string>(){"Villa"});
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<List<VillaNumberDto>>(villaNumbers);
                _response.Success = true;
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>(){e.ToString()};
                return BadRequest(_response);
            }
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateVilla(VillaNumberDto villaNumberDto)
        {

            try
            {
                if (villaNumberDto is null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() { "Villa is null" };
                    return BadRequest(_response);
                }

                if (await _villaNumberRepository.GetAsync(v=>v.VillaNo == villaNumberDto.VillaNo) is not null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() { "The provided Villa Number Allready exists" };
                    return BadRequest(_response);
                }

                if (await _villaRepository.GetAsync(v=>v.ID == villaNumberDto.VillaID) is null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() { "No vill whith the provided ID" };
                    return BadRequest(_response);
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaNumberDto);
                villaNumber.CreatedTime = DateTime.Now;
                await _villaNumberRepository.AddAsync(villaNumber);
                _response.StatusCode = HttpStatusCode.Created;
                _response.Result = villaNumber;
                _response.Success = true;
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>(){e.ToString()};
                return BadRequest(_response);
            }
        }
        
        
        
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, VillaNumberDto villaNumberDto)
        {
            try
            {
                if (villaNumberDto is null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() { "Villa is null" };
                    return BadRequest(_response);
                }
                if (await _villaRepository.GetAsync(v=>v.ID == villaNumberDto.VillaID) is null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() { "No vill whith the provided ID" };
                    return BadRequest(_response);
                }
                VillaNumber villaNumber = await _villaNumberRepository.GetAsync(id);
                if (villaNumber is null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() { "No villa with the provided ID" };
                    return NotFound(_response);
                }
                
                villaNumber.SpecialDetails = villaNumberDto.SpecialDetails;
                villaNumber.VillaNo = villaNumberDto.VillaNo;
                villaNumber.VillaID = villaNumberDto.VillaID;
                
                await _villaNumberRepository.UpdateAsync(villaNumber);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<VillaNumberDto>(villaNumber);
                _response.Success = true;
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>(){e.ToString()};
                return BadRequest(_response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                VillaNumber villaNumber = await _villaNumberRepository.GetAsync(id);
                if (villaNumber is null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "No villa with the provided ID" };
                    return NotFound(_response);
                }

                await _villaNumberRepository.DeleteAsync(villaNumber);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<VillaNumberDto>(villaNumber);
                _response.Success = true;
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>(){e.ToString()};
                return BadRequest(_response);
            }
        }
    }
}
