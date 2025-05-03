using Azure.Core;
using Core.Interfaces;
using Domin.DTOs;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly IUintOfWork _uintOfWork;
        public AuthController(IAuthRepository authRepository, ITokenService tokenService, IUintOfWork uintOfWork)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
            _uintOfWork = uintOfWork;
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

        [Authorize(Policy = "AllPolicy")]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {

                // get jti
                var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrWhiteSpace(jti))
                {
                    return Unauthorized(new
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        message =  "Invalid token : missing JTI"
                    });
                }
                // set revoce => add jti to revocedtokens table
                await _uintOfWork.revokedTokens.RevokeRokenAsync(jti);
                await _uintOfWork.SaveChangesAsync();

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    message = "Logged out successfully!"
                });


            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    message = "An error occurred while logging out!",
                    Details = ex.Message
                });
            }
        }

        [Authorize(Policy = "AllPolicy")]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenRequest request)
        {
            var response = await _tokenService.RefreshToken(request.Token,GetIpAddress());
            if (response == null)
                return Unauthorized(new
                {
                    SatatusCode = StatusCodes.Status401Unauthorized,
                    message = "Invalid Token Or Expired"
                });
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "Refresh Token Generated Successfully!",
                Details = response
            });
        }

        [Authorize(Policy = "AllPolicy")]
        [HttpPost("revoce-Token")]
        public async Task<IActionResult> RevoceToken(TokenRequest request)
        {
            var result = await _tokenService.RevoceToken(request.Token, GetIpAddress());
            if (result)
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    message = "Token has been revoced successfully!"

                });
            return NotFound(new
            {
                StatusCode = StatusCodes.Status404NotFound,
                message= "Token Not Found Or Already Revoced!"
            });
          }      






        private string GetIpAddress()
        {
            return Request.Headers.ContainsKey("X-Forwarded-For") ?
                Request.Headers["X-Forwarded-For"] :
                HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

    }
}
