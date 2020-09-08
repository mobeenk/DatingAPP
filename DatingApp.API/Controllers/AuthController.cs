using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _config = config;
            _repo = repo;

        }
        //http://localhost:5000/api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto UserForRegisterDto)
        {
            //validate request

            UserForRegisterDto.Username = UserForRegisterDto.Username.ToLower();
            if (await _repo.UserExists(UserForRegisterDto.Username))
                return BadRequest("Username already exists!");

            var userToCreate = _mapper.Map<User>(UserForRegisterDto);
            // new User  { Username = userForRegister.Username };
            var createdUser = await _repo.Register(userToCreate, UserForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForDetailedDto>(createdUser);
            // from UserController GetUser API method
            // http://localhost:5000/api/  users/2 where users is the controller and 2 is parameter in method
            return CreatedAtRoute("GetUser",new {controller = "Users", id = createdUser.Id}, userToReturn);
        }
        //http://localhost:5000/api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            // throw new Exception("PC says no!");

            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[]
            {
                        new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                        new Claim(ClaimTypes.Name,userFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var user = _mapper.Map<UserForListDto>(userFromRepo);
            // send the user map along with token
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }
    }
}