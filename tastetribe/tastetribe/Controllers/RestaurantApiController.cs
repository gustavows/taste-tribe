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
        private const string RapidApiKey = "583c0bba26mshca00ed2820d21e1p1c5526jsnf09664a080f7";
        private const string RapidApiHost = "restaurants-near-me-usa.p.rapidapi.com";
        private const string ZipEndpoint = "https://restaurants-near-me-usa.p.rapidapi.com/restaurants/location/zipcode/";

        public RestaurantApiController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("by-location")]
        public async Task<IActionResult> GetByLocation([FromQuery] string zip = null)
        {
            if (string.IsNullOrWhiteSpace(zip))
                return BadRequest("Zip code is required.");

            var url = ZipEndpoint + System.Net.WebUtility.UrlEncode(zip) + "/0";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-rapidapi-key", RapidApiKey);
            request.Headers.Add("x-rapidapi-host", RapidApiHost);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}
