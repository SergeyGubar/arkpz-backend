using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StatosphericBackend.Context;
using StatosphericBackend.Entities;
using StatosphericBackend.Options;

namespace StatosphericBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserController(ApplicationContext context, UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task RegisterAsync([FromBody] UserRegisterRequestModel userRegisterRequestModel)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Email = userRegisterRequestModel.Email,
                    UserName = userRegisterRequestModel.Email,
                    Role = userRegisterRequestModel.Role
                };
                var result = await _userManager.CreateAsync(user, userRegisterRequestModel.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);

                    await Login(new AuthModel
                    {
                        Email = userRegisterRequestModel.Email,
                        Password = userRegisterRequestModel.Password
                    });
                }
                else
                {
                    await Response.WriteAsync("Result validation failed!");
                }
            }
        }

        [HttpPost("login")]
        public async Task Login([FromBody] AuthModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
                    };

                    var jwtSecurityToken = new JwtSecurityToken(
                        issuer: AuthOptions.Issuer,
                        audience: AuthOptions.Audience,
                        claims: claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(AuthOptions.Lifetime)),
                        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthOptions.Key)),
                            SecurityAlgorithms.HmacSha256));
                    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

                    var response = new
                    {
                        access_token = encodedJwt,
                        username = user.UserName
                    };

                    Response.ContentType = "application/json";
                    await Response.WriteAsync(JsonConvert.SerializeObject(response,
                        new JsonSerializerSettings {Formatting = Formatting.Indented}));
                    return;
                }

                await Response.WriteAsync("Wrong credentials!");
            }
        }
        
        [HttpGet("getUser")]
        public async Task GetCurrentUserAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await Response.WriteAsync(userId);
        }

        [HttpPost("logout")]
        public async Task LogOff()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
        }
        
        [HttpPost("uploadPhoto")]
        [Authorize]
        public async Task UploadPhotoAsync(IFormFile photo)
        {
            var user = await GetCurrentUser();
            var userId = user.Id;
            /*using (var stream = new MemoryStream())
            {
                await photo.CopyToAsync(stream);
                var result = stream.ToArray();
            }*/
        }

        private Task<User> GetCurrentUser() => _userManager.GetUserAsync(HttpContext.User);
    }
}