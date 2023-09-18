using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto request)
        {
            //Map DTO to Domain Model
            var category = new Category
            {
                Name = request.Name,
                UrlHandle = request.UrlHandle
            };
            await categoryRepository.CreateAsync(category);
            // Domain Model to DTO 

            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };
            return Ok(response);
        }

        //GET: https://localhost:7163/api/Categories
        [HttpGet]
        
        public async Task<IActionResult> GetAllCategories()
        {
           var categories= await categoryRepository.GetAllAsync();

            //Map Domain Model to DTO
            var response=new List<CategoryDto>();
            foreach (var category in categories)
            {
                response.Add(new CategoryDto 
                { Id = category.Id, 
                    Name = category.Name , 
                    UrlHandle=category.UrlHandle
                });
            }
            return Ok(response);

        }

        //GET: https://localhost:7163/api/Categories/{id}
        // https://localhost:7163/api/Categories/b9a67d8b-9537-4d16-20f1-08dbafba7826
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
        {
           var existingCategory= await categoryRepository.GetByIdAsync(id);
            if (existingCategory is null)
            {
                return NotFound();
            }
            var response = new CategoryDto
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                UrlHandle = existingCategory.UrlHandle
            };
            return Ok(response);
        }

        //PUT : https://localhost:7163/api/Categories/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] UpdateCategoryRequestDto request)
        {
            //convert DTO to Domain model
            var category = new Category
            {
                Id = id,
                Name = request.Name,
                UrlHandle = request.UrlHandle
            };
            category= await categoryRepository.UpdateAsync(category);
            if (category ==null)
                return NotFound();
            //convert Domain model to DTO
            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };
            return Ok(response);
        }

        //DELETE: https://localhost:7163/api/Categories/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            var category = await categoryRepository.DeleteAsync(id);
            if (category is null)
                return NotFound();
            //convert Domain Model to DTO
            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };
            return Ok(response);

        }
        [HttpPost]
        [Route("DeleteCategories")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteCategories([FromBody] List<Guid> ids)
        {
            var category = await categoryRepository.DeleteBulkAsync(ids);
            if (category is null)
                return NotFound();
            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };
            return Ok(response);
        }

    }
}
