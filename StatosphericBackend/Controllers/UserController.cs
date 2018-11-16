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
using Microsoft.EntityFrameworkCore;
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
            await _signInManager.PasswordSignInAsync(model.Email, model.Password, true,
                false);

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role),

                        // 
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                    };

                    var jwtSecurityToken = new JwtSecurityToken(
                        issuer: AuthOptions.Issuer,
                        audience: AuthOptions.Audience,
                        claims: claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(AuthOptions.Lifetime)),
                        signingCredentials: new SigningCredentials(
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthOptions.Key)),
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

        [HttpPost("logout")]
        public async Task LogOff()
        {
            await _signInManager.SignOutAsync();
        }

        [HttpPost("uploadPhoto")]
        [Authorize]
        public async Task UploadPhotoAsync(IFormFile file)
        {
            var email = User.Identity.Name;
            var user = _context
                .Users
                .Include(usr => usr.Photos)
                .FirstOrDefault(u => u.Email == email);
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                var result = stream.ToArray();
                var photo = new Photo {Content = result};
                user.Photos.Add(photo);
                await _context.SaveChangesAsync();
            }
        }

        [HttpGet("getPhoto")] 
        [Authorize]
        public async Task<IActionResult> GetPhotoAsync(string id)
        {
            var image = await _context.Photo.FirstOrDefaultAsync(ph => ph.Id == id);
            return File(image.Content, "image/jpeg");
        }

        [HttpGet("getUser")]
        [Authorize]
        public async Task<User> GetUserAsync(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(usr => usr.Id == id);
        }

        [HttpGet("getLaunch")]
        [Authorize]
        public async Task<Launch> GetLaunchAsync(string id)
        {
            return await _context.Launches.FirstOrDefaultAsync(usr => usr.Id == id);
        }

        [HttpGet("getPhotos")]
        [Authorize]
        public List<string> GetPhotos()
        {
            var email = User.Identity.Name;
            var user = _context
                .Users
                .Include(usr => usr.Photos)
                .FirstOrDefault(u => u.Email == email);
            return user.Photos.Select(ph => ph.Id).ToList();
        }

        [HttpPost("addLaunch")]
        [Authorize(Roles = "admin")]
        public async Task AddLaunchAsync([FromBody] LaunchAddRequestModel model)
        {
            var launch = new Launch
            {
                Name = model.Name,
                Time = model.Time,
                Latitude = model.Latitude,
                Longitude = model.Longitude
            };
            _context.Launches.Add(launch);
            await _context.SaveChangesAsync();
        }


        [HttpDelete("removeLaunch")]
        [Authorize(Roles = "admin")]
        public async Task RemoveLaunchAsync(string id)
        {
            var launch = _context.Launches.First(l => l.Id == id);
            _context.Launches.Remove(launch);
            await _context.SaveChangesAsync();
        }

        [HttpDelete("removePhoto")]
        [Authorize]
        public async Task RemovePhotoAsync(string id)
        {
            var photo = _context.Photo.First(ph => ph.Id == id);
            _context.Photo.Remove(photo);
            await _context.SaveChangesAsync();
        }


        [HttpGet("launchesRadius")]
        [Authorize]
        public async Task<IEnumerable<Launch>> GetLaunchesInRadiusAsync(double longitude, double latitude)
        {
            
            return await _context.Launches
                .Where(l => GetDistance(l.Longitude, longitude, l.Latitude, latitude) < 0.2)
                .ToListAsync();
        }

        private double GetDistance(double x1, double x2, double y1, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }
    }
}