using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HuynhDuyKhanh.Data;
using HuynhDuyKhanh.Model;
using Microsoft.AspNetCore.Authorization;


namespace HuynhDuyKhanh.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderDetailController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderDetail
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var details = await _context.OrderDetails.ToListAsync();
        //    return Ok(details);
        //}
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var details = await _context.OrderDetails
                .Select(od => new
                {
                    orderDetailId = od.OrderDetailId,
                    orderId = od.OrderId,
                    productId = od.ProductId
                })
                .ToListAsync();

            return Ok(details);
        }


        // GET: api/OrderDetail/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var detail = await _context.OrderDetails.FindAsync(id);
        //    if (detail == null)
        //        return NotFound();

        //    return Ok(detail);
        //}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var detail = await _context.OrderDetails
                .Where(od => od.OrderDetailId == id)
                .Select(od => new
                {
                    orderDetailId = od.OrderDetailId,
                    orderId = od.OrderId,
                    productId = od.ProductId
                })
                .FirstOrDefaultAsync();

            if (detail == null)
                return NotFound();

            return Ok(detail);
        }


        // POST: api/OrderDetail
        [HttpPost]
        public async Task<IActionResult> Create(OrderDetail orderDetail)
        {
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = orderDetail.OrderDetailId }, orderDetail);
        }

        // PUT: api/OrderDetail/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailId)
                return BadRequest();

            _context.Entry(orderDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.OrderDetails.Any(od => od.OrderDetailId == id))
                    return NotFound();

                throw;
            }
            return NoContent();
        }

        // DELETE: api/OrderDetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var detail = await _context.OrderDetails.FindAsync(id);
            if (detail == null)
                return NotFound();

            _context.OrderDetails.Remove(detail);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}