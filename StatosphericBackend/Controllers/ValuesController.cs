using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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

        [HttpGet]
        public IEnumerable<Launch> Get()
        {
            return _context.Launches;
        }
        
        [HttpGet("getWeather")]
        public async Task<string> Get(double latitude, double longitude)
        {
            var httpClient = new HttpClient();
            
            var url = $"{BaseUrl}/{_key}/{latitude},{longitude}";
            
            using (var result = await httpClient.GetAsync(url))
            {
                return await result.Content.ReadAsStringAsync();
            }
        }
        
        
        
       
        /*// GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/
    }
}