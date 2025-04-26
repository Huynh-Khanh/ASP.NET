using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HuynhDuyKhanh.Data;
using HuynhDuyKhanh.DTO;
using HuynhDuyKhanh.Model;

namespace HuynhDuyKhanh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);  // Trả về danh sách sản phẩm
        }

        // GET: api/product/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();  // Trả về 404 nếu không tìm thấy sản phẩm
            }
            return Ok(product);  // Trả về sản phẩm theo id
        }

        // POST: api/product
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO productdto)
        {
            if (productdto == null)
            {
                return BadRequest();  // Trả về lỗi nếu không có dữ liệu
            }

            var product = _mapper.Map<Product>(productdto);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.Product_Id }, product);
        }

        // PUT: api/product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO productdto)
        {
            if (id != productdto.Product_Id)
            {
                return BadRequest();  // Trả về lỗi nếu id không khớp
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound();  // Trả về lỗi nếu sản phẩm không tồn tại
            }

            existingProduct.Product_Name = productdto.Product_Name;
            existingProduct.Price = productdto.Price;
            existingProduct.Image = productdto.Image;
            existingProduct.Update_at = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();  // Trả về 204 khi cập nhật thành công
        }

        // DELETE: api/product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();  // Trả về lỗi nếu không tìm thấy sản phẩm
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();  // Trả về 204 khi xóa thành công
        }
    }
}
