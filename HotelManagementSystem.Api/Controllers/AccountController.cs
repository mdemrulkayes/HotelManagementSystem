using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Business.Core;
using Business.DataModels;
using Business.Models;
using Common;
using DataAccess.Data;
using HotelManagementSystem.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace HotelManagementSystem.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AccountController(IOptions<AppSettings> options, IUserRepository userRepository, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
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
                    return BadRequest(new ErrorModel()
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            ErrorMessage = "Username and password does not match"
                        }
                    );
                }

                if (!await _userManager.IsEmailConfirmedAsync(await _userManager.FindByIdAsync(user.Id)))
                {
                    return BadRequest(
                        new ErrorModel()
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            ErrorMessage = "User Email is not confirmed. Please check your email to confirm"
                        }
                    );
                }

                var userWithToken =await GenerateJwtToken(user);
                userWithToken.RefreshToken = GenerateRefreshToken();

                var updateRefreshTokenResult =  await _userRepository.UpdateUser(user.Id, userWithToken);
                if (!updateRefreshTokenResult.Succeeded)
                {
                    return BadRequest(
                        new ErrorModel()
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            ErrorMessage = "Something went very bad. Please contact with administrator."
                        }
                    );
                }

                return Ok(userWithToken);
            }
            else
            {
                return BadRequest(
                    new ErrorModel()
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorMessage = "Username and password does not match"
                    }
                );
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Signup([FromBody] UserRequestDTO model)
        {
            var result = await _userRepository.CreateUser(model);
            if (result.Succeeded)
            {
                var applicationUser = await _userManager.FindByEmailAsync(model.Email);
                var addUserToRole = await _userManager.AddToRoleAsync(applicationUser, model.UserRole);

                //Send confirmation email here
                string code =HttpUtility.UrlEncode(await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser));
                var callBackUrl = $"{_configuration["WasmUrl"]}/confirm-email?userId={applicationUser.Id}&code={code}";

                var mailBody =
                    $"Hello {applicationUser.Name} <br /> To Confirm your account please click on the below link <a href='{callBackUrl}'>{callBackUrl}</a>";

                try
                {
                    var emailSendResult = await EmailSender.SendEmailAsync(applicationUser.Email, "Confirm Account", mailBody);
                }
                catch (Exception e)
                {
                    //Log error into the DB
                    Log.Error(e, "Error on Reset password send email");
                }

                if (addUserToRole.Succeeded)
                {
                    return Ok(new SuccessModel()
                    {
                        StatusCode = StatusCodes.Status200OK,
                        SuccessMessage = "User created successfully"
                    });
                }
                return Ok(new SuccessModel()
                {
                    StatusCode = StatusCodes.Status200OK,
                    SuccessMessage = "User created successfully but can not assign to a role. Please contact with administrator."
                });
            }
            else
            {
                return BadRequest(GenerateErrorFromIdentityResult(result));
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return BadRequest(new ErrorModel()
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorMessage = "Invalid Information"
                    });
                }

                var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, model.Code);
                if (confirmEmailResult.Succeeded)
                {
                    return Ok(new SuccessModel()
                    {
                        StatusCode = StatusCodes.Status200OK,
                        SuccessMessage =
                            "Email Confirmed successfully."
                    });
                }
                else
                {
                    return BadRequest(new ErrorModel()
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorMessage = "Invalid Operation. Can not confirm your account."
                    });
                }
            }

            return BadRequest(ModelState);
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
                    return BadRequest(
                        new ErrorModel()
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            ErrorMessage = "Invalid User details"
                        }
                    );
                }

                var savedRefreshToken = user.RefreshToken;
                if (savedRefreshToken != tokenModel.RefreshToken)
                {
                    return BadRequest(
                        new ErrorModel()
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            ErrorMessage = "Invalid refresh token"
                        }
                    );
                }

                var userWithToken = await GenerateJwtToken(user);
                userWithToken.RefreshToken = GenerateRefreshToken();

                var updateRefreshTokenResult = await _userRepository.UpdateUser(user.Id, userWithToken);
                if (!updateRefreshTokenResult.Succeeded)
                {
                    return BadRequest(
                        new ErrorModel()
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            ErrorMessage = "Something went very bad. Please contact with administrator."
                        }
                    );
                }

                return Ok(new SuccessModel()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = user
                });
            }
            catch (SecurityTokenException e)
            {
                return BadRequest(
                    new ErrorModel()
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorMessage = "Invalid auth token"
                    }
                );
            }
            catch (Exception e)
            {
                return BadRequest(
                    new ErrorModel()
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorMessage = e.Message
                    }
                );
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendResetPasswordLink([FromBody] ForgotPasswordDTO passwordDto)
        {
            if (ModelState.IsValid)
            {
                var applicationUser = await _userManager.FindByEmailAsync(passwordDto.Email);
                if (applicationUser == null)
                {
                    return Ok(new SuccessModel()
                    {
                        StatusCode = StatusCodes.Status200OK,
                        SuccessMessage =
                            "Reset password link sent to your email. Please check email and reset your password."
                    });
                }

                var token = HttpUtility.UrlEncode(await _userManager.GeneratePasswordResetTokenAsync(applicationUser));
                var callBack = $"{_configuration["WasmUrl"]}/ResetPassword?userid={applicationUser.Id}&token={token}";

                //Send email code will be here
                var mailBody =
                    $"Hello {applicationUser.Name}. <br /> To reset your pass word please click the below link. <a href='{callBack}'>{callBack}</a>";

                try
                {
                    var result = await EmailSender.SendEmailAsync(applicationUser.Email, "Reset password", mailBody);
                }
                catch (Exception e)
                {
                    //Log error into the DB
                    Log.Error(e,"Error on Reset password send email");
                }
                
                return Ok(new SuccessModel()
                {
                    StatusCode = StatusCodes.Status200OK,
                    SuccessMessage =
                        "Reset password link sent to your email. Please check email and reset your password."
                });
            }

            return BadRequest(new ErrorModel()
            {
                ErrorMessage = "Please fill all the fields."
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(PasswordResetDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return BadRequest(new ErrorModel()
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorMessage = "Invalid Information"
                    });
                }

                var resetPasResult = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (resetPasResult.Succeeded)
                {
                    return Ok(new SuccessModel()
                    {
                        StatusCode = StatusCodes.Status200OK,
                        SuccessMessage =
                            "Password reset successfully."
                    });
                }
                else
                {
                    return BadRequest(GenerateErrorFromIdentityResult(resetPasResult));
                }
            }

            return BadRequest(new ErrorModel()
            {
                ErrorMessage = "Please fill all the fields."
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var userDetails = await _userRepository.GetUserAsync(User.FindFirst("Id").Value);
            if (userDetails == null)
            {
                return BadRequest(
                    new ErrorModel()
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorMessage = "User not found."
                    }
                );
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

        private ErrorModel GenerateErrorFromIdentityResult(IdentityResult result)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var errors in result.Errors)
            {
                sb.Append($"{errors.Description}");
                sb.Append("</br />");
            }

            return new ErrorModel()
            {
                ErrorMessage = sb.ToString(),
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        private async Task<UserDTO> GenerateJwtToken(UserDTO user)
        {
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByEmailAsync(user.Email));
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.Add(new Claim("Id", user.Id));
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }

            user.Role = roles.Count > 0 ? roles[0] : "User";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(30);

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
