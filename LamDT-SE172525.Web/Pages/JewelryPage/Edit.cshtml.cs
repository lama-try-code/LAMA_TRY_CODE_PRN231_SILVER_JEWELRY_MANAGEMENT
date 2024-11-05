using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LamDT_SE172525.BOs;
using LamDT_SE172525.DAO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace LamDT_SE172525.Web.Pages.JewelryPage
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EditModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public SilverJewelry SilverJewelry { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7077/");

                var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
                if (string.IsNullOrEmpty(token))
                {
                    return NotFound();
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("/api/jewelry/" + id);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    SilverJewelry = JsonSerializer.Deserialize<SilverJewelry>(content, options) ?? new SilverJewelry();
                    await GetCategories();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7077/");

                var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
                if (string.IsNullOrEmpty(token))
                {
                    return NotFound();
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var requestBody = new
                {
                    id = id, 
                    name = SilverJewelry.SilverJewelryName,
                    description = SilverJewelry.SilverJewelryDescription,
                    metalWeight = SilverJewelry.MetalWeight,
                    price = SilverJewelry.Price,
                    productionYear = SilverJewelry.ProductionYear,
                    categoryId = SilverJewelry.Category.CategoryId
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PutAsync("api/jewelry", stringContent);
                if (response.IsSuccessStatusCode)
                    return RedirectToPage("./Index");
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update the jewelry. Please try again.");
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
