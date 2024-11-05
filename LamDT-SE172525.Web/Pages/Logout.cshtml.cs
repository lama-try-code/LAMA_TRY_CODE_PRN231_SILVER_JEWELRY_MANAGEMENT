using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LamDT_SE172525.Web.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogoutModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult OnGet()
        {
            _httpContextAccessor.HttpContext?.Session.Remove("JWTToken");

            _httpContextAccessor.HttpContext?.Session.Clear();
            return RedirectToPage("/Index");
        }
    }
}
