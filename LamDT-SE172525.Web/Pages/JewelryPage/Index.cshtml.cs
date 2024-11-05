using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LamDT_SE172525.BOs;
using LamDT_SE172525.DAO;
using LamDT_SE172525.Repository.Jewelry;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LamDT_SE172525.Web.Pages.JewelryPage
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IndexModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public IList<SilverJewelry> SilverJewelry { get; set; } = new List<SilverJewelry>();
        public string? ErrorMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? SearchWeight { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7077/");

                // Get token from session
                var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
                if (string.IsNullOrEmpty(token))
                {
                    ErrorMessage = "No authentication token found. Please login again.";
                    return;
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var url = "api/jewelry";
                if (!string.IsNullOrEmpty(SearchName) || SearchWeight.HasValue)
                {
                    url += "/search?";
                    var parameters = new List<string>();

                    if (!string.IsNullOrEmpty(SearchName))
                    {
                        parameters.Add($"name={Uri.EscapeDataString(SearchName)}");
                    }

                    if (SearchWeight.HasValue)
                    {
                        parameters.Add($"weight={SearchWeight}");
                    }

                    url += string.Join("&", parameters);
                }

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    SilverJewelry = JsonSerializer.Deserialize<List<SilverJewelry>>(content, options) ?? new List<SilverJewelry>();
                }
                else
                {
                    ErrorMessage = $"Failed to fetch jewelry data. Status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
        }
    }
}
