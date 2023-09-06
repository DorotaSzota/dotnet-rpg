using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    
    [ApiController, Route("[controller]")]
    public class AuthentificationController : ControllerBase
    {
        private readonly IAuthentificationRepository _authentificationRepository;

        public AuthentificationController(IAuthentificationRepository authentificationRepository)
        {
            _authentificationRepository = authentificationRepository;
        }

    [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request)
        {
            var response = await _authentificationRepository.Register(new User {
                Username = request.Username}, request.Password
            );
            if(!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);    

        }

         [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<int>>> Login(UserLoginDto request)
        {
            var response = await _authentificationRepository.Login(request.Username, request.Password);
            if(!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);    

        }
    }
}