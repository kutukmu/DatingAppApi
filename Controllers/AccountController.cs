using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _dbContext;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext dbContext, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _dbContext = dbContext;

        }

        [HttpPost("register")]

        public async Task<ActionResult<UserDto>> RegisterUser(RegisterDto registerDto)
        {

            if (await UserExist(registerDto.Username.ToLower())) return BadRequest("User Already Exist");

            using var hac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                UserPassword = hac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                UserHash = hac.Key
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };

        }

        private async Task<bool> UserExist(string username)
        {
            return await _dbContext.Users.AnyAsync(x => x.UserName == username);
        }



        [HttpPost("login")]

        public async Task<ActionResult<UserDto>> LoginUser(LoginDto loginDto)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user == null)
            {
                return Unauthorized("Invalid Username");
            }

            using var hmac = new HMACSHA512(user.UserHash);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.UserPassword[i])
                {
                    return Unauthorized("Password is wrong");
                }
            }

            return  new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };


        }


    }
}