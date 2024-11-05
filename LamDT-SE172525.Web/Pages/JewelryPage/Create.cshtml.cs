using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using LamDT_SE172525.BOs;
using LamDT_SE172525.DAO;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace LamDT_SE172525.Web.Pages.JewelryPage
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public CreateModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> OnGet()
        {
            await GetCategories();
            return Page();
        }

        [BindProperty]
        public SilverJewelry SilverJewelry { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7077/");

                var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
                if (string.IsNullOrEmpty(token))
                    return NotFound();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var requestBody = new
                {
                    name = SilverJewelry.SilverJewelryName,
                    description = SilverJewelry.SilverJewelryDescription,
                    metalWeight = SilverJewelry.MetalWeight,
                    price = SilverJewelry.Price,
                    productionYear = SilverJewelry.ProductionYear,
                    categoryId = SilverJewelry.Category.CategoryId
                };
                var jsonContent = JsonSerializer.Serialize(requestBody);
                var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/jewelry", stringContent);
                if (response.IsSuccessStatusCode)
                    return RedirectToPage("./Index");
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create the jewelry. Please try again.");
                    await GetCategories();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        private async Task GetCategories()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7077/");

                var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
                if (string.IsNullOrEmpty(token))
                {
                    return;
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("/api/category");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    List<Category> cateList = JsonSerializer.Deserialize<List<Category>>(content, options) ?? new List<Category>();
                    ViewData["CategoryId"] = new SelectList(cateList, "CategoryId", "CategoryName");
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}
