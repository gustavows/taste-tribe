using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace tastetribe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantApiController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;


        public RestaurantApiController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClient = httpClientFactory.CreateClient();
            _config = config;
        }

        [HttpGet("by-location")]
        public async Task<IActionResult> GetByLocation([FromQuery] string? zip = null)
        {
            if (string.IsNullOrWhiteSpace(zip))
                return BadRequest("Zip code is required.");

            var baseUrl = _config["RapidApi:BaseUrl"];
            var url = baseUrl + System.Net.WebUtility.UrlEncode(zip) + "/0";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-rapidapi-key", _config["RapidApi:Key"]);
            request.Headers.Add("x-rapidapi-host", _config["RapidApi:Host"]);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}
