using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StatosphericBackend.Context;
using StatosphericBackend.Entities;
using StatosphericBackend.Options;

namespace StatosphericBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IOptions<ApiOptions> _options;
        private readonly string _key;
        private readonly HttpClient _httpClient;

        private static string BaseUrl = "https://api.darksky.net/forecast";

        public ValuesController(ApplicationContext context, IOptions<ApiOptions> options)
        {
            _context = context;
            _options = options;
            _key = _options.Value.ApiKey;
            _httpClient = new HttpClient();
        }
        
        [HttpGet("launches")]
//        [Authorize]
        public IEnumerable<Launch> Get()
        {
            return _context.Launches;
        }

        [HttpGet("weather")]
//        [Authorize]
        public async Task<string> GetWeatherAsync(double latitude, double longitude)
        {
            var url = $"{BaseUrl}/{_key}/{latitude},{longitude}";

            using (var result = await _httpClient.GetAsync(url))
            {
                return await result.Content.ReadAsStringAsync();
            }
        }
        
    }
}