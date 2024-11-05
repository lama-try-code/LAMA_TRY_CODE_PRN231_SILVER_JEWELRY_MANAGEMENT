using LamDT_SE172525.Repository.Cate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LamDT_SE172525.Api.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _categoryRepository.GetAllCategories();
            if(response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute]String id)
        {
            var response = await _categoryRepository.GetCategory(id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }
    }
}
