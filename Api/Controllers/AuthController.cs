using Core.Interfaces;
using Domin.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        public AuthController(IAuthRepository authRepository, ITokenService tokenService)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var user = await _authRepository.RegisterAsync(model);
                    if (user is null)
                        return BadRequest(new
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = "User already exists",
                        });
                    return Ok(new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "User registered successfully",
                        Data = user
                    });
                }
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "ModelStateError",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });



            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                });
            }

        }



        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _authRepository.LoginAsync(model);
                    if (user is null)
                        return Unauthorized(new
                        {
                            StatusCode = StatusCodes.Status401Unauthorized,
                            Message = "Invalid username or password",
                        });

                    // generate token and store it
                    var token = _tokenService.GenerateJwtToken(user);
                    await _authRepository.StoreTokenAsync(user, token);

                    return Ok(new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "User logged in successfully",
                        Data = new
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            token = token,
                        }

                    });

                }
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "ModelStateError",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                });
            }

        }
    }
}
