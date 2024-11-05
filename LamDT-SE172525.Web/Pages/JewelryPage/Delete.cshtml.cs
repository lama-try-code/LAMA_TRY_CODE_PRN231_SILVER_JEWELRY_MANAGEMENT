using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LamDT_SE172525.BOs;
using LamDT_SE172525.DAO;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LamDT_SE172525.Web.Pages.JewelryPage
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
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

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7077/");

                var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
                if (string.IsNullOrEmpty(token))
                    return NotFound();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.DeleteAsync("api/jewelry/" + id);
                if (response.IsSuccessStatusCode)
                    return RedirectToPage("./Index");
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to delete the jewelry. Please try again.");
                    return Page();
                }
            }
            catch(Exception ex)
            {
                return NotFound();
            }
        }
    }
}
