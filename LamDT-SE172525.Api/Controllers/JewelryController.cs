using LamDT_SE172525.BOs;
using LamDT_SE172525.DAO;
using LamDT_SE172525.Repository.Cate;
using LamDT_SE172525.Repository.Jewelry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace LamDT_SE172525.Api.Controllers
{
    [Route("api/jewelry")]
    [ApiController]
    public class JewelryController : ControllerBase
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly ICategoryRepository _categoryRepository;
        public JewelryController(IJewelryRepository jewelryRepository, ICategoryRepository categoryRepository)
        {
            _jewelryRepository = jewelryRepository;
            _categoryRepository = categoryRepository;
        }

        [Authorize(Roles = "1, 2")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _jewelryRepository.GetAll();
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [Authorize(Roles = "1, 2")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJewelryById([FromRoute] String id)
        {
            var response = await _jewelryRepository.GetSilverJewelryById(id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [Authorize(Roles = "1, 2")]
        [HttpGet("search")]
        public async Task<IActionResult> Search(string? name, decimal? weight)
        {
            var response = await _jewelryRepository.SearchJewelryAsync(name, weight);
            if(response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        public async Task<IActionResult> AddJewelry([FromBody] JewelryCreateDTO jewelry)
        {
            if(jewelry.ProductionYear < 1900)
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Production year must not be earlier than 1900",
                    Success = false
                });
            if(jewelry.Price < 0)
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Price must not be less than 0",
                    Success = false
                });
            if(!IsValidSilverJewelryName(jewelry.Name))
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Jewelry name is not valid",
                    Success = false
                });

            SilverJewelry data = new SilverJewelry();
            Util helper = new Util();
            data.SilverJewelryId = helper.GenerateRandomString();
            data.SilverJewelryName = jewelry.Name;
            data.SilverJewelryDescription = jewelry.Description;
            data.MetalWeight = jewelry.MetalWeight;
            data.Price = jewelry.Price;
            data.ProductionYear = jewelry.ProductionYear;
            data.CreatedDate = DateTime.Now;
            data.CategoryId = jewelry.CategoryId;
            var check = await _jewelryRepository.AddJewelry(data);
            if (!check) return NotFound();
            return Ok(check);
        }

        [Authorize(Roles = "1")]
        [HttpPut]
        public async Task<IActionResult> UpdateJewelry([FromBody] JewelryUpdateDTO jewelry)
        {
            SilverJewelry data = await _jewelryRepository.GetSilverJewelryById(jewelry.Id);
            if (data == null) return NotFound();

            data.SilverJewelryName = jewelry.Name;
            data.SilverJewelryDescription = jewelry.Description;
            data.MetalWeight = jewelry.MetalWeight;
            data.Price = jewelry.Price;
            data.ProductionYear = jewelry.ProductionYear;
            data.CategoryId = jewelry.CategoryId;

            var check = await _jewelryRepository.UpdateJewelry(data);
            if (!check) return NotFound();
            return Ok(check);
        }

        [Authorize(Roles = "1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveJewelry([FromRoute] string id)
        {
            var check = await _jewelryRepository.RemoveJewelry(id);
            if (!check) return NotFound();
            return Ok(check);
        }

        private bool IsValidSilverJewelryName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            string[] words = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                if (word.Length == 0 || !char.IsUpper(word[0]))
                    return false;

                if (!word.All(c => char.IsLetterOrDigit(c)))
                    return false;
            }

            return true;
        }
    }

    public class JewelryCreateDTO()
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal MetalWeight { get; set; }
        public required decimal Price { get; set; }
        public required int ProductionYear { get; set; }
        public required string CategoryId { get; set; }
    }

    public class JewelryUpdateDTO()
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal MetalWeight { get; set; }
        public required decimal Price { get; set; }
        public required int ProductionYear { get; set; }
        public required string CategoryId { get; set; }
    }
}
