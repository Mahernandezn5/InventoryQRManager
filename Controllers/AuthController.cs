using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InventoryQRManager.Models;
using InventoryQRManager.Models.DTOs;
using InventoryQRManager.Services;
using System.Security.Cryptography;
using System.Text;

namespace InventoryQRManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new ApiResponse<LoginResponseDto>
                    {
                        Success = false,
                        Message = "Usuario y contraseña son requeridos."
                    });
                }

                var success = _authService.Login(request.Username, request.Password);
                
                if (success && _authService.CurrentUser != null)
                {
                    var response = new LoginResponseDto
                    {
                        UserId = _authService.CurrentUser.Id,
                        Username = _authService.CurrentUser.Username,
                        FullName = _authService.CurrentUser.FullName,
                        Email = _authService.CurrentUser.Email,
                        Role = _authService.CurrentUser.Role.ToString(),
                        LastLoginDate = _authService.CurrentUser.LastLoginDate
                    };

                    return Ok(new ApiResponse<LoginResponseDto>
                    {
                        Success = true,
                        Data = response,
                        Message = "Login exitoso."
                    });
                }

                return Unauthorized(new ApiResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = "Credenciales inválidas."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login.");
                return StatusCode(500, new ApiResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = "Error interno del servidor."
                });
            }
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            try
            {
                _authService.Logout();
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Logout exitoso."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout.");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor."
                });
            }
        }

        [HttpGet("current-user")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> GetCurrentUser()
        {
            try
            {
                if (_authService.CurrentUser == null)
                {
                    return Unauthorized(new ApiResponse<LoginResponseDto>
                    {
                        Success = false,
                        Message = "No hay usuario autenticado."
                    });
                }

                var user = _authService.CurrentUser;
                var response = new LoginResponseDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    LastLoginDate = user.LastLoginDate
                };

                return Ok(new ApiResponse<LoginResponseDto>
                {
                    Success = true,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user.");
                return StatusCode(500, new ApiResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = "Error interno del servidor."
                });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<object>>> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || 
                    string.IsNullOrWhiteSpace(request.Email) || 
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Todos los campos son requeridos."
                    });
                }

                var success = _authService.Register(
                    request.Username,
                    request.Email,
                    request.Password,
                    request.FirstName ?? "",
                    request.LastName ?? "",
                    UserRole.Employee
                );

                if (success)
                {
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Usuario registrado exitosamente."
                    });
                }

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error al registrar usuario. Verifique que el usuario no exista."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration.");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor."
                });
            }
        }
    }
}
