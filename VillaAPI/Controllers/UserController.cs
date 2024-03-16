using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VillaAPI.Models;
using VillaAPI.Models.DTO;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILocalUserRepository _localUserRepository;

        public UserController(ILocalUserRepository localUserRepository)
        {
            _localUserRepository = localUserRepository;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDto)
        {
            try
            {
                LoginResponseDTO loginResponseDto = await _localUserRepository.Login(loginRequestDto);
                APIResponse response = new APIResponse();
                if (loginResponseDto.User is null)
                {
                    response.ErrorMessages = new List<string>() { "UserName or Password are incorrect" };
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(response);
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Success = true;
                response.Result = loginResponseDto;
                return Ok(response);
            }
            catch (Exception e)
            {
                APIResponse response = new APIResponse()
                {
                    ErrorMessages = { e.ToString() },
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
                return BadRequest(response);
            }
        }
        
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequestDTO registerRequestDto)
        {
            APIResponse response = new APIResponse();
            LocalUserDTO user = await _localUserRepository.Register(registerRequestDto);
            if (User is null)
            {
                response.ErrorMessages = new List<string>() { "User Name is Aleady Token" };
                response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            if (user.UserName == "Error")
            {
                response.ErrorMessages = new List<string>() { user.Name };
                response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            response.StatusCode = HttpStatusCode.OK;
            response.Success = true;
            response.Result = user;
            return Ok(response);
        }
        
        
        [HttpPost("Refresh")]
        public async Task<IActionResult> RefreshToken(TokenDTO  tokenDto)
        {
            APIResponse response = new APIResponse();
            if (ModelState.IsValid)
            {
                TokenDTO Token = await _localUserRepository.RefreshAccessToken(tokenDto);
                if (Token is null || string.IsNullOrEmpty(Token.AccessToken))
                {
                    response.ErrorMessages = new(){"Invalid Token"};
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(response);
                }
                response.StatusCode = HttpStatusCode.OK;
                response.Result = Token;
                return Ok(response);

            }
            else
            {
                response.ErrorMessages = new(){"Invalid Token"};
                response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }
        }
    }
}
