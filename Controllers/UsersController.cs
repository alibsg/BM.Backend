using System;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using BM.BackEnd.DTOs;
using BM.BackEnd.Helpers;
using BM.BackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using BM.BackEnd.Models.Entities;

namespace BM.BackEnd.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController: ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(IUserService userService, IMapper mapper ,IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;

        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        IActionResult Authenticate([FromBody]UserDTO userDTO)
        {
            var user = ((UserService)_userService).AuthenticateByUserNameAndPassword(userDTO.UserName, userDTO.Password);
            if(user == null){
                return BadRequest(new { message = "Invalid useranme or password" });
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var userdto = _mapper.Map<UserDTO>(user);
            userdto.Token = tokenString;
            return Ok(userdto);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDTO userDTO)
        {
            var user = _mapper.Map<User>(userDTO);
            try{
                _userService.Create(user,userDTO.Password);
                return Ok();
            }
            catch(AppException ex)
            {
                return BadRequest(new { message = ex.Message});
            }
        }
        

    }  
}

