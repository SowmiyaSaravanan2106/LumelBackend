using AppStore.Model;
using BackendAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {


        private readonly AppDBContext _context;

        public ProductController(AppDBContext context)
        {
            _context = context;
        }


        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopNProducts(
    [FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate,
    [FromQuery] int top = 10)
        {
            if (startDate > endDate) return BadRequest("Start date must be before end date.");
            if (top <= 0) return BadRequest("Top N must be greater than zero.");
            var result = await _context.OrderItems
        .Where(oi => oi.Order!.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
        .Include(oi => oi.Product)
        .GroupBy(oi => new { oi.ProductId, oi.Product!.ProductName })
        .Select(g => new TopProductDto
        {
            ProductId = g.Key.ProductId!.Value,
            ProductName = g.Key.ProductName,
            TotalQuantitySold = g.Sum(oi => oi.Quantity)
        })
        .OrderByDescending(p => p.TotalQuantitySold)
        .Take(top)
        .ToListAsync();
            return Ok(result);
        }



        [HttpGet("top-products-by-category")]
        public async Task<IActionResult> GetTopProductsByCategory(
    [FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate,
    [FromQuery] int top = 3)
        {
            if (startDate > endDate) return BadRequest("Start date must be before end date.");
            if (top <= 0) return BadRequest("Top N must be greater than zero.");

            var orderItems = await _context.OrderItems
                              .Include(oi => oi.Product)!.ThenInclude(p => p.Category)
                                .Where(oi => oi.Order!.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                                .ToListAsync();

            var groupedByCategory = orderItems
                .Where(oi => oi.Product != null && oi.Product.Category != null)
                .GroupBy(oi => new { oi.Product.CategoryId, oi.Product.Category.CategoryName, oi.ProductId, oi.Product.ProductName })
                .Select(g => new TopProductByCategoryDto
                {
                    CategoryId = g.Key.CategoryId!.Value,
                    CategoryName = g.Key.CategoryName,
                    ProductId = g.Key.ProductId!.Value,
                    ProductName = g.Key.ProductName,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity)
                })
                .ToList();

            var result = groupedByCategory
                .GroupBy(x => new { x.CategoryId, x.CategoryName })
                .SelectMany(g => g.OrderByDescending(x => x.TotalQuantitySold).Take(top))
                .OrderBy(x => x.CategoryId)
                .ToList();
            return Ok(result);
        }



        [HttpGet("top-products-by-region")]
        public async Task<IActionResult> GetTopProductsByRegion(
    [FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate,
    [FromQuery] int top = 3)
        {
            if (startDate > endDate) return BadRequest("Start date must be before end date.");
            if (top <= 0) return BadRequest("Top N must be greater than zero.");
            var orderItems = await _context.OrderItems
       .Include(oi => oi.Product)
       .Include(oi => oi.Order)!.ThenInclude(o => o.Customer)!.ThenInclude(c => c.Region)
       .Where(oi => oi.Order!.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
       .ToListAsync();

            var grouped = orderItems
                .Where(oi => oi.Order!.Customer != null && oi.Order.Customer.Region != null)
                .GroupBy(oi => new
                {
                    RegionId = oi.Order.Customer.RegionId!.Value,
                    RegionName = oi.Order.Customer.Region!.RegionName,
                    ProductId = oi.ProductId!.Value,
                    ProductName = oi.Product!.ProductName
                })
                .Select(g => new TopProductByRegionDto
                {
                    RegionId = g.Key.RegionId,
                    RegionName = g.Key.RegionName,
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity)
                })
                .ToList();

            var result = grouped
                .GroupBy(x => new { x.RegionId, x.RegionName })
                .SelectMany(g => g.OrderByDescending(x => x.TotalQuantitySold).Take(top))
                .OrderBy(x => x.RegionId)
                .ToList();

            return Ok(result);
        }

    }
}
