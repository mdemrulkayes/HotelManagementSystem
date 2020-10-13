using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Business.Core;
using Business.DataModels;
using Business.Models;
using DataAccess.Data;
using HotelManagementSystem.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HotelManagementSystem.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserRepository _userRepository;

        public AccountController(IOptions<AppSettings> options, IUserRepository userRepository, SignInManager<ApplicationUser> signInManager)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
            _appSettings = options.Value;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Signin([FromBody]AuthenticationDTO model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
            if (result.Succeeded)
            {
                var user = await _userRepository.GetUserByNameAsync(model.UserName);
                if (user == null)
                {
                    return BadRequest(new List<ErrorModel>()
                    {
                        new ErrorModel()
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            ErrorMessage = "Username and password does not match"
                        }
                    });
                }

                var userWithToken = GenerateJwtToken(user);
                userWithToken.RefreshToken = GenerateRefreshToken();

                var updateRefreshTokenResult =  await _userRepository.UpdateUser(user.Id, userWithToken);
                if (!updateRefreshTokenResult.Succeeded)
                {
                    return BadRequest(new List<ErrorModel>()
                    {
                        new ErrorModel()
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            ErrorMessage = "Something went very bad. Please contact with administrator."
                        }
                    });
                }

                return Ok(new SuccessModel()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = userWithToken
                });
            }
            else
            {
                return BadRequest(new List<ErrorModel>()
                {
                    new ErrorModel()
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorMessage = "Username and password does not match"
                    }
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Signup([FromBody] UserRequestDTO model)
        {
            var result = await _userRepository.CreateUser(model);
            if (result.Succeeded)
            {
                return Ok(new SuccessModel()
                {
                    StatusCode = StatusCodes.Status200OK,
                    SuccessMessage = "User created successfully"
                });
            }
            else
            {
                return BadRequest(GenerateErrorFromIdentityResult(result));
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public  async Task<IActionResult> Refresh([FromBody] RefreshTokenModel tokenModel)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(tokenModel.ExpiredToken);
                var user = await _userRepository.GetUserAsync(principal.FindFirst("Id").Value);
                if (user == null)
                {
                    return BadRequest(new List<ErrorModel>()
                    {
                        new ErrorModel()
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            ErrorMessage = "Invalid User details"
                        }
                    });
                }

                var savedRefreshToken = user.RefreshToken;
                if (savedRefreshToken != tokenModel.RefreshToken)
                {
                    return BadRequest(new List<ErrorModel>()
                    {
                        new ErrorModel()
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            ErrorMessage = "Invalid refresh token"
                        }
                    });
                }

                var userWithToken = GenerateJwtToken(user);
                userWithToken.RefreshToken = GenerateRefreshToken();

                var updateRefreshTokenResult = await _userRepository.UpdateUser(user.Id, userWithToken);
                if (!updateRefreshTokenResult.Succeeded)
                {
                    return BadRequest(new List<ErrorModel>()
                    {
                        new ErrorModel()
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            ErrorMessage = "Something went very bad. Please contact with administrator."
                        }
                    });
                }

                return Ok(new SuccessModel()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = user
                });
            }
            catch (SecurityTokenException e)
            {
                return BadRequest(new List<ErrorModel>()
                {
                    new ErrorModel()
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorMessage = "Invalid auth token"
                    }
                });
            }
            catch (Exception e)
            {
                return BadRequest(new List<ErrorModel>()
                {
                    new ErrorModel()
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorMessage = e.Message
                    }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var userDetails = await _userRepository.GetUserAsync(User.FindFirst("Id").Value);
            if (userDetails == null)
            {
                return BadRequest(new List<ErrorModel>()
                {
                    new ErrorModel()
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorMessage = "User not found."
                    }
                });
            }

            var result = await _userRepository.ChangePasswordAsync(userDetails.Id, model);
            if (!result.Succeeded)
            {
                return BadRequest(GenerateErrorFromIdentityResult(result));
            }

            return Ok(new SuccessModel()
            {
                StatusCode = StatusCodes.Status200OK,
                SuccessMessage = "Password has been changed successfully"
            });
        }

        private List<ErrorModel> GenerateErrorFromIdentityResult(IdentityResult result)
        {
            var errorList = new List<ErrorModel>();
            foreach (var errors in result.Errors)
            {
                errorList.Add(new ErrorModel()
                {
                    Title = errors.Code,
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorMessage = errors.Description
                });
            }

            return errorList;
        }

        private UserDTO GenerateJwtToken(UserDTO user)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.Add(new Claim("Id", user.Id));
            

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(1);

            var token = new JwtSecurityToken(
                issuer: _appSettings.ValidIssuer,
                audience: _appSettings.ValidAudience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            user.Token = new JwtSecurityTokenHandler().WriteToken(token);

            return user;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string expiredToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true, 
                ValidateIssuer = true,
                ValidIssuer = _appSettings.ValidIssuer,
                ValidAudience = _appSettings.ValidAudience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.SecretKey)),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(expiredToken, tokenValidationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
