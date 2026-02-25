using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPIDB.Models;

namespace WebAPIDB.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _dbContext.Products.ToList();

            if(products == null || products.Count == 0)
            {
                return NotFound(new { message = "No products found." });
            }

            return Ok(products);
        }
    }
}
