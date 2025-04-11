using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppStore.DTO;
using AppStore.Model;
using System;
using BackendAPI.DTO;

namespace AppStore.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RevenueController : ControllerBase
    {

        private readonly AppDBContext _context;

        public RevenueController(AppDBContext context)
        {
            _context = context;
        }
        [HttpGet("total-revenue")]
        public async Task<IActionResult> GetTotalRevenue([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate) return BadRequest("Start date must be before end date.");

            var total = await _context.OrderItems
                  .Where(oi => oi.Order!.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                  .SumAsync(oi =>
                      (oi.UnitPrice * oi.Quantity)
                      - (oi.Order!.Discount / _context.OrderItems.Count(o => o.OrderId == oi.OrderId)) 
                      + (oi.Order.ShippingCost / _context.OrderItems.Count(o => o.OrderId == oi.OrderId)) 
                  );

            total= Math.Round(total, 2);
            return Ok(new { startDate, endDate, totalRevenue = total });
        }


        [HttpGet("total-revenue-by-product")]
        public async Task<IActionResult> GetTotalRevenueByProduct([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate) return BadRequest("Start date must be before end date.");
            var orderItems = await _context.OrderItems
      .Where(oi => oi.Order!.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
      .Include(oi => oi.Product)
      .Include(oi => oi.Order)
      .ToListAsync();

            var results = orderItems
                .GroupBy(oi => new { oi.ProductId, oi.Product!.ProductName })
                .Select(g => new ProductRevenueDto
                {
                    ProductId = g.Key.ProductId!.Value,
                    ProductName = g.Key.ProductName,
                    TotalRevenue = Math.Round(
                        g.Sum(oi =>
                            (oi.UnitPrice * oi.Quantity)
                            - (oi.Order!.Discount / _context.OrderItems.Count(o => o.OrderId == oi.OrderId))
                            + (oi.Order.ShippingCost / _context.OrderItems.Count(o => o.OrderId == oi.OrderId))
                        ), 2)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();
            return Ok(results);
        }



        [HttpGet("total-revenue-by-category")]
        public async Task<IActionResult> GetTotalRevenueByCategory([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate) return BadRequest("Start date must be before end date.");

            var orderItems = await _context.OrderItems
        .Where(oi => oi.Order!.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
        .Include(oi => oi.Product)!.ThenInclude(p => p.Category)
        .Include(oi => oi.Order)
        .ToListAsync();

            var results = orderItems
                .Where(oi => oi.Product!.Category != null)
                .GroupBy(oi => new { oi.Product!.CategoryId, oi.Product.Category!.CategoryName })
                .Select(g => new CategoryRevenueDto
                {
                    CategoryId = g.Key.CategoryId!.Value,
                    CategoryName = g.Key.CategoryName,
                    TotalRevenue = Math.Round(
                        g.Sum(oi =>
                            (oi.UnitPrice * oi.Quantity)
                            - (oi.Order!.Discount / _context.OrderItems.Count(o => o.OrderId == oi.OrderId))
                            + (oi.Order.ShippingCost / _context.OrderItems.Count(o => o.OrderId == oi.OrderId))
                        ), 2)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

      
            return Ok(results);
        }


        [HttpGet("total-revenue-by-region")]
        public async Task<IActionResult> GetTotalRevenueByRegion([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate) return BadRequest("Start date must be before end date.");

            var orderItems = await _context.OrderItems
         .Where(oi => oi.Order!.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
         .Include(oi => oi.Order)!.ThenInclude(o => o.Customer)!.ThenInclude(c => c.Region)
         .ToListAsync();

            var results = orderItems
                .Where(oi => oi.Order!.Customer != null && oi.Order.Customer.Region != null)
                .GroupBy(oi => new { oi.Order.Customer.RegionId, oi.Order.Customer.Region.RegionName })
                .Select(g => new RegionRevenueDto
                {
                    RegionId = g.Key.RegionId!.Value,
                    RegionName = g.Key.RegionName,
                    TotalRevenue = Math.Round(
                        g.Sum(oi =>
                            (oi.UnitPrice * oi.Quantity)
                            - (oi.Order!.Discount / _context.OrderItems.Count(o => o.OrderId == oi.OrderId))
                            + (oi.Order.ShippingCost / _context.OrderItems.Count(o => o.OrderId == oi.OrderId))
                        ), 2)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();
            return Ok(results);
        }
    }
}
