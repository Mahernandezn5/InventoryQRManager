using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InventoryQRManager.Models.DTOs;
using InventoryQRManager.Services;

namespace InventoryQRManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ReportService _reportService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(ReportService reportService, ILogger<ReportsController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        /// <summary>
        
        /// </summary>
        [HttpGet("categories")]
        public async Task<ActionResult<ApiResponse<List<CategorySummaryDto>>>> GetCategorySummary()
        {
            try
            {
                var summaries = _reportService.GetCategorySummary();
                var summaryDtos = summaries.Select(s => new CategorySummaryDto
                {
                    Category = s.Category,
                    ItemsCount = s.ItemsCount,
                    TotalQuantity = s.TotalQuantity,
                    TotalValue = s.TotalValue,
                    AveragePrice = s.AveragePrice
                }).ToList();

                return Ok(new ApiResponse<List<CategorySummaryDto>>
                {
                    Success = true,
                    Message = "Resumen por categorías obtenido correctamente",
                    Data = summaryDtos
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo resumen por categorías");
                return StatusCode(500, new ApiResponse<List<CategorySummaryDto>>
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        
        /// </summary>
        [HttpGet("locations")]
        public async Task<ActionResult<ApiResponse<List<LocationSummaryDto>>>> GetLocationSummary()
        {
            try
            {
                var summaries = _reportService.GetLocationSummary();
                var summaryDtos = summaries.Select(s => new LocationSummaryDto
                {
                    Location = s.Location,
                    ItemsCount = s.ItemsCount,
                    TotalQuantity = s.TotalQuantity,
                    TotalValue = s.TotalValue
                }).ToList();

                return Ok(new ApiResponse<List<LocationSummaryDto>>
                {
                    Success = true,
                    Message = "Resumen por ubicaciones obtenido correctamente",
                    Data = summaryDtos
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo resumen por ubicaciones");
                return StatusCode(500, new ApiResponse<List<LocationSummaryDto>>
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        
        /// </summary>
        [HttpGet("low-stock")]
        public async Task<ActionResult<ApiResponse<List<LowStockItemDto>>>> GetLowStockItems([FromQuery] int threshold = 10)
        {
            try
            {
                var items = _reportService.GetLowStockItems(threshold);
                var itemDtos = items.Select(i => new LowStockItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    SKU = i.SKU,
                    Quantity = i.Quantity,
                    Category = i.Category,
                    Location = i.Location
                }).ToList();

                return Ok(new ApiResponse<List<LowStockItemDto>>
                {
                    Success = true,
                    Message = $"Se encontraron {itemDtos.Count} items con stock bajo",
                    Data = itemDtos
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo items con stock bajo");
                return StatusCode(500, new ApiResponse<List<LowStockItemDto>>
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        
        /// </summary>
        [HttpGet("created-period")]
        public async Task<ActionResult<ApiResponse<List<InventoryItemDto>>>> GetItemsCreatedInPeriod(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                var items = _reportService.GetItemsCreatedInPeriod(startDate, endDate);
                var itemDtos = items.Select(MapToDto).ToList();

                return Ok(new ApiResponse<List<InventoryItemDto>>
                {
                    Success = true,
                    Message = $"Se encontraron {itemDtos.Count} items creados en el período",
                    Data = itemDtos
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo items por período");
                return StatusCode(500, new ApiResponse<List<InventoryItemDto>>
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        private static InventoryItemDto MapToDto(Models.InventoryItem item)
        {
            return new InventoryItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                SKU = item.SKU,
                QRCode = item.QRCode,
                Quantity = item.Quantity,
                Price = item.Price,
                Category = item.Category,
                Location = item.Location,
                CreatedDate = item.CreatedDate,
                LastUpdated = item.LastUpdated,
                IsActive = item.IsActive
            };
        }
    }

    public class CategorySummaryDto
    {
        public string Category { get; set; } = string.Empty;
        public int ItemsCount { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public decimal AveragePrice { get; set; }
    }

    public class LocationSummaryDto
    {
        public string Location { get; set; } = string.Empty;
        public int ItemsCount { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class LowStockItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
