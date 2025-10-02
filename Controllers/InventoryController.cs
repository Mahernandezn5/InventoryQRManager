using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InventoryQRManager.Models;
using InventoryQRManager.Models.DTOs;
using InventoryQRManager.Services;

namespace InventoryQRManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _inventoryService;
        private readonly ReportService _reportService;
        private readonly QRCodeService _qrCodeService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(
            InventoryService inventoryService, 
            ReportService reportService,
            QRCodeService qrCodeService,
            ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _reportService = reportService;
            _qrCodeService = qrCodeService;
            _logger = logger;
        }

        /// <summary>
       
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<InventoryItemDto>>>> GetAllItems()
        {
            try
            {
                var items = _inventoryService.GetAllItems();
                var itemDtos = items.Select(MapToDto).ToList();
                
                return Ok(new ApiResponse<List<InventoryItemDto>>
                {
                    Success = true,
                    Message = "Items obtenidos correctamente",
                    Data = itemDtos
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo todos los items");
                return StatusCode(500, new ApiResponse<List<InventoryItemDto>>
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<InventoryItemDto>>> GetItemById(int id)
        {
            try
            {
                var item = _inventoryService.GetItemById(id);
                if (item == null)
                {
                    return NotFound(new ApiResponse<InventoryItemDto>
                    {
                        Success = false,
                        Message = "Item no encontrado"
                    });
                }

                return Ok(new ApiResponse<InventoryItemDto>
                {
                    Success = true,
                    Message = "Item obtenido correctamente",
                    Data = MapToDto(item)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo item por ID: {Id}", id);
                return StatusCode(500, new ApiResponse<InventoryItemDto>
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        
        /// </summary>
        [HttpGet("sku/{sku}")]
        public async Task<ActionResult<ApiResponse<InventoryItemDto>>> GetItemBySKU(string sku)
        {
            try
            {
                var item = _inventoryService.GetItemBySKU(sku);
                if (item == null)
                {
                    return NotFound(new ApiResponse<InventoryItemDto>
                    {
                        Success = false,
                        Message = "Item no encontrado"
                    });
                }

                return Ok(new ApiResponse<InventoryItemDto>
                {
                    Success = true,
                    Message = "Item obtenido correctamente",
                    Data = MapToDto(item)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo item por SKU: {SKU}", sku);
                return StatusCode(500, new ApiResponse<InventoryItemDto>
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        
        /// </summary>
        [HttpGet("qr/{qrCode}")]
        public async Task<ActionResult<ApiResponse<InventoryItemDto>>> GetItemByQRCode(string qrCode)
        {
            try
            {
                var item = _inventoryService.GetItemByQRCode(qrCode);
                if (item == null)
                {
                    return NotFound(new ApiResponse<InventoryItemDto>
                    {
                        Success = false,
                        Message = "Item no encontrado"
                    });
                }

                return Ok(new ApiResponse<InventoryItemDto>
                {
                    Success = true,
                    Message = "Item obtenido correctamente",
                    Data = MapToDto(item)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo item por QR: {QRCode}", qrCode);
                return StatusCode(500, new ApiResponse<InventoryItemDto>
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<InventoryItemDto>>> CreateItem([FromBody] CreateInventoryItemDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<InventoryItemDto>
                    {
                        Success = false,
                        Message = "Datos de entrada inválidos",
                        Data = null
                    });
                }

                
                var existingItem = _inventoryService.GetItemBySKU(createDto.SKU);
                if (existingItem != null)
                {
                    return Conflict(new ApiResponse<InventoryItemDto>
                    {
                        Success = false,
                        Message = "Ya existe un item con este SKU"
                    });
                }

                var item = new InventoryItem
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    SKU = createDto.SKU,
                    Quantity = createDto.Quantity,
                    Price = createDto.Price,
                    Category = createDto.Category,
                    Location = createDto.Location,
                    CreatedDate = DateTime.Now,
                    QRCode = _qrCodeService.GenerateUniqueQRCode(createDto.SKU, createDto.Name)
                };

                var success = _inventoryService.AddItem(item);
                if (!success)
                {
                    return StatusCode(500, new ApiResponse<InventoryItemDto>
                    {
                        Success = false,
                        Message = "Error al crear el item"
                    });
                }

                return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, new ApiResponse<InventoryItemDto>
                {
                    Success = true,
                    Message = "Item creado correctamente",
                    Data = MapToDto(item)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando item");
                return StatusCode(500, new ApiResponse<InventoryItemDto>
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<InventoryItemDto>>> UpdateItem(int id, [FromBody] UpdateInventoryItemDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<InventoryItemDto>
                    {
                        Success = false,
                        Message = "Datos de entrada inválidos"
                    });
                }

                var existingItem = _inventoryService.GetItemById(id);
                if (existingItem == null)
                {
                    return NotFound(new ApiResponse<InventoryItemDto>
                    {
                        Success = false,
                        Message = "Item no encontrado"
                    });
                }

                
                existingItem.Name = updateDto.Name;
                existingItem.Description = updateDto.Description;
                existingItem.SKU = updateDto.SKU;
                existingItem.Quantity = updateDto.Quantity;
                existingItem.Price = updateDto.Price;
                existingItem.Category = updateDto.Category;
                existingItem.Location = updateDto.Location;
                existingItem.LastUpdated = DateTime.Now;

                var success = _inventoryService.UpdateItem(existingItem);
                if (!success)
                {
                    return StatusCode(500, new ApiResponse<InventoryItemDto>
                    {
                        Success = false,
                        Message = "Error al actualizar el item"
                    });
                }

                return Ok(new ApiResponse<InventoryItemDto>
                {
                    Success = true,
                    Message = "Item actualizado correctamente",
                    Data = MapToDto(existingItem)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando item: {Id}", id);
                return StatusCode(500, new ApiResponse<InventoryItemDto>
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteItem(int id)
        {
            try
            {
                var existingItem = _inventoryService.GetItemById(id);
                if (existingItem == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Item no encontrado"
                    });
                }

                var success = _inventoryService.DeleteItem(id);
                if (!success)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Error al eliminar el item"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Item eliminado correctamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando item: {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<InventorySummaryDto>>> GetInventorySummary()
        {
            try
            {
                var summary = _reportService.GetInventorySummary();
                var lowStockItems = _reportService.GetLowStockItems();
                
                var summaryDto = new InventorySummaryDto
                {
                    TotalItems = summary.TotalItems,
                    TotalQuantity = summary.TotalQuantity,
                    TotalValue = summary.TotalValue,
                    CategoriesCount = summary.CategoriesCount,
                    LocationsCount = summary.LocationsCount,
                    LowStockItems = lowStockItems.Count
                };

                return Ok(new ApiResponse<InventorySummaryDto>
                {
                    Success = true,
                    Message = "Resumen obtenido correctamente",
                    Data = summaryDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo resumen del inventario");
                return StatusCode(500, new ApiResponse<InventorySummaryDto>
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<List<InventoryItemDto>>>> SearchItems([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest(new ApiResponse<List<InventoryItemDto>>
                    {
                        Success = false,
                        Message = "Término de búsqueda requerido"
                    });
                }

                var items = _reportService.SearchItems(term);
                var itemDtos = items.Select(MapToDto).ToList();

                return Ok(new ApiResponse<List<InventoryItemDto>>
                {
                    Success = true,
                    Message = $"Se encontraron {itemDtos.Count} items",
                    Data = itemDtos
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buscando items: {Term}", term);
                return StatusCode(500, new ApiResponse<List<InventoryItemDto>>
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        private static InventoryItemDto MapToDto(InventoryItem item)
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
}
