//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using WebAPIDB.Models;

//namespace WebAPIDB.Controllers
//{
//    [Route("api/product")]
//    [ApiController]
//    public class ProductController : ControllerBase
//    {
//        private readonly ApplicationDbContext _dbContext;

//        public ProductController(ApplicationDbContext dbContext)
//        {
//            _dbContext = dbContext;
//        }
//        [HttpGet]
//        public IActionResult GetProducts()
//        {
//            var products = _dbContext.Products.ToList();

//            if(products == null || products.Count == 0)
//            {
//                return NotFound(new { message = "No products found." });
//            }

//            var baseUrl = $"{Request.Scheme}//{Request.Host}";
//            //foreach

//            return Ok(products);
//        }

//        [HttpPost]
//        public async Task<IActionResult> AddProduct([FromForm] PostProductDao productDao)
//        {
//            var file = productDao.File;
//            if(file == null || file.Length == 0)
//            {
//                return BadRequest(new { message = "File is required" });

//            }
//            var allowedExtensions = new[] { "image/jpg", "image/jpeg", "image/png", "image/gif" };

//            if (!allowedExtensions.Contains(file.ContentType))
//            {
//                return BadRequest(new { message = "Invalid failed type. Only JPED, JPG, PNG, GIF are allowed" });
//            }

//            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
//            if(!Directory.Exists(uploadPath))
//            {
//                Directory.CreateDirectory(uploadPath);
//            }

//            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
//            var filePath = Path.Combine(uploadPath, fileName);

//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await file.CopyToAsync(stream);
//            }

//            var product = new Product
//            {
//                Name = productDao.Name,
//                Price = productDao.Price,
//                Qty = productDao.Qty,
//                ImageUrl = "/images/" +  fileName
//            };

//            _dbContext.Products.Add(product);
//            await _dbContext.SaveChangesAsync();


//            //string baseUrl = $"{Request.Scheme}//"

//            return CreatedAtAction(nameof(GetProducts), new {newProduct = product});
//        }



//    }
//}









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

        // ======================
        // GET: api/product
        // ======================
        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _dbContext.Products.ToList();

            if (products == null || products.Count == 0)
            {
                return NotFound(new { message = "No products found." });
            }

            // Correct Base URL
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            // Return full image URL
            var result = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.Qty,
                ImageUrl = baseUrl + p.ImageUrl
            });

            return Ok(result);
        }

        // ======================
        // POST: api/product
        // ======================
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] PostProductDao productDao)
        {
            var file = productDao.File;

            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Image file is required" });
            }

            // Validate file type
            var allowedTypes = new[]
            {
                "image/jpg",
                "image/jpeg",
                "image/png",
                "image/gif"
            };

            if (!allowedTypes.Contains(file.ContentType))
            {
                return BadRequest(new { message = "Only JPG, JPEG, PNG, GIF are allowed" });
            }

            // Upload path: wwwroot/images
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Generate unique file name
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save to database
            var product = new Product
            {
                Name = productDao.Name,
                Price = productDao.Price,
                Qty = productDao.Qty,
                ImageUrl = "/images/" + fileName
            };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            // Return full URL
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var response = new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Qty,
                ImageUrl = baseUrl + product.ImageUrl
            };

            return Created("", response);
        }


        [HttpPut]
        public async Task<ActionResult> UpdateProduct([FromForm] PutProductDAO productDAO)
        {
            var existingProduct = await _dbContext.Products.FindAsync(productDAO.Id);
            if(existingProduct == null)
            {
                return NotFound(new { message = $"Product id={productDAO.Id} not found" });

            }

            existingProduct.Name = productDAO.Name;
            existingProduct.Price = productDAO.Price;
            existingProduct.Qty = productDAO.Qty;

            var file = productDAO.File;
            if (file != null && file.Length > 0)
            {
                var allowedTypes = new[]
                {
                    "image/jpg",
                    "image/jpeg",
                    "image/png",
                    "image/gif"
                };
                if (!allowedTypes.Contains(file.ContentType))
                {
                    return BadRequest(new { message = "Only JPG, JPEG, PNG, GIF are allowed" });
                }
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                // Delete old image file if exists
                if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingProduct.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                existingProduct.ImageUrl = "/images/" + fileName;
            }



            _dbContext.Products.Update(existingProduct);
            await _dbContext.SaveChangesAsync();

            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            existingProduct.ImageUrl = baseUrl + existingProduct.ImageUrl;


            return Accepted(existingProduct);

        }

        [HttpGet("{id}"),HttpGet("id/{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _dbContext.Products.Find(id);
            if(product == null)
            {
                return NotFound(new { message = $"Product id={id} not found" });
            }
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            product.ImageUrl = baseUrl + product.ImageUrl;
            return Ok(product);
        }

        [HttpDelete("{id}"), HttpDelete("id/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if(product == null)
            {
                return NotFound(new { message = $"Product id={id} not found" });
            }
            // Delete image file if exists
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            return Accepted(new { message = $"Product id={id} deleted successfully" });

        }
        }

}