using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace LamDT_SE172525.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;


        [BindProperty]
        public LoginViewModel Login { get; set; }
        public bool? IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public LoginModel(
           ILogger<LoginModel> logger,
           HttpClient httpClient,
           IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _httpClient.BaseAddress = new Uri("https://localhost:7077/");
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(Login),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("api/account", content);

                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    StoreJwtToken(token);
                    await GetRole(token);
                    IsSuccess = true;
                    return RedirectToPage("/Index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    IsSuccess = false;
                    ErrorMessage = "Invalid username or password";
                }
                else
                {
                    IsSuccess = false;
                    ErrorMessage = "An error occurred during login";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                IsSuccess = false;
                ErrorMessage = "An unexpected error occurred";
            }

            return Page();
        }

        private void StoreJwtToken(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("JWTToken", token, cookieOptions);
            HttpContext.Session.SetString("JWTToken", token);
        }

        private async Task<String> GetRole(string token)
        {
            var contentRole = new StringContent(
                    JsonSerializer.Serialize(token),
                    Encoding.UTF8,
                    "application/json"
                );
            var responseRole = await _httpClient.PostAsync("api/account/get-role", contentRole);
            if (responseRole.IsSuccessStatusCode)
            {
                var role = await responseRole.Content.ReadAsStringAsync();
                var roleModel = JsonSerializer.Deserialize<RoleModel>(role);
                StoreRole(roleModel.role);

                IsSuccess = true;
            }
            else if (responseRole.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                IsSuccess = false;
                ErrorMessage = "Invalid username or password";
            }
            else
            {
                IsSuccess = false;
                ErrorMessage = "An error occurred during login";
            }
            return await responseRole.Content.ReadAsStringAsync();
        }

        private void StoreRole(string role)
        {
            HttpContext.Session.SetString("Role", role);
        }

        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken")) ||
                   Request.Cookies.ContainsKey("JWTToken");
        }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
    }

    public class RoleModel
    {
        public string role { get; set; }
    }
}