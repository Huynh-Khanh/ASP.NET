using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HuynhDuyKhanh_2122110004.Data;
using HuynhDuyKhanh_2122110004.Model;

namespace HuynhDuyKhanh_2122110004.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Product - Lấy tất cả sản phẩm
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        // GET api/Product/5 - Lấy sản phẩm theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { Message = $"Product with ID {id} not found" });
            }
            return Ok(product);
        }

        // POST api/Product - Thêm sản phẩm mới
        [HttpPost]
        public async Task<ActionResult<Product>> Post([FromBody] Product product)
        {
            if (product == null || string.IsNullOrEmpty(product.Name) || product.Price < 0)
            {
                return BadRequest(new { Message = "Invalid product data. Name is required and Price must be non-negative." });
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }

        // PUT api/Product/5 - Cập nhật sản phẩm
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Product updatedProduct)
        {
            if (updatedProduct == null || string.IsNullOrEmpty(updatedProduct.Name) || updatedProduct.Price < 0)
            {
                return BadRequest(new { Message = "Invalid product data. Name is required and Price must be non-negative." });
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { Message = $"Product with ID {id} not found" });
            }

            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Image = updatedProduct.Image;

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/Product/5 - Xóa sản phẩm
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { Message = $"Product with ID {id} not found" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Product with ID {id} has been deleted" });
        }
    }
}