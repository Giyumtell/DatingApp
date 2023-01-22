using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDTO registerDto)
        {
            if (await UserExists(registerDto.Username))
            {
                return BadRequest("User already exists");
            }
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new UserDto { Username = user.UserName, Token = _tokenService.CreateToken(user)});
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(usr => usr.UserName == loginDTO.Username);
            if (user == null) return Unauthorized("user does not exist");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var newHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            return CompareBytes (newHash, user.PasswordHash) ? Ok(new UserDto { Username = user.UserName, Token = _tokenService.CreateToken(user) }) : Unauthorized("invalid password");

        } 
        private bool CompareBytes(byte[] bytes1, byte[] bytes2)
        {
            for (int i = 0; i < bytes1.Length; i++)
            {
                if (bytes1[i] != bytes2[i])
                {
                    return false;
                }
            }
            return true;
        }
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
