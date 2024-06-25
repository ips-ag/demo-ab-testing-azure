using API.DTOs;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ApplicationIdentityDbContext dbContext,
        IMapper mapper,
        ITokenGenerationService tokenService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = mapper.Map<ApplicationUser>(model);

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Registration successful" });
            }

            return BadRequest(new { Message = "Registration failed", result.Errors });
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                // Generate a JWT token with user claims
                var tokenClaims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user!.Id),
                    new(ClaimTypes.Name, user!.DisplayName),
                    new(ClaimTypes.Email, user!.Email!),
                    // Add more claims as needed
                };

                // Generate the JWT token
                var token = tokenService.GenerateToken(tokenClaims);

                return new UserDto
                {
                    Email = user.Email!,
                    Token = token,
                    DisplayName = user.DisplayName
                };
            }

            if (result.RequiresTwoFactor)
            {
                // Handle two-factor authentication if enabled
                // You can implement this based on your setup
                return BadRequest(new { Message = "Two-factor authentication required" });
            }

            return BadRequest(new { Message = "Login failed" });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> LoadUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId!);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            // Generate a JWT token with user claims
            var tokenClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.DisplayName),
                new(ClaimTypes.Email, user.Email!),
                // Add more claims as needed
            };

            // Generate the JWT token
            var token = tokenService.GenerateToken(tokenClaims);

            var userDto = new UserDto
            {
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Token = token // Set the Token property with the generated token
            };

            return Ok(userDto);
        }




        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok(new { Message = "Logout successful" });
        }

        [Authorize]
        [HttpGet("user-address")]
        public IActionResult GetUserAddress()
        {
            var userId = userManager.GetUserId(User);
            var user = dbContext.Users.Find(userId);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var address = mapper.Map<AddressDto>(user.Address);
            // Return the user's address as needed

            return Ok(new { Address = address });
        }

        [HttpGet("check-email-exists")]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { Message = "Email is required" });
            }

            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                return Ok(new { EmailExists = true });
            }

            return Ok(new { EmailExists = false });
        }

        [Authorize]
        [HttpPost("update-user-address")]
        public async Task<IActionResult> UpdateUserAddress(AddressDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = userManager.GetUserId(User);
            var user = dbContext.Users.Find(userId);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var address = mapper.Map<Address>(model);
            user.Address = address;

            await dbContext.SaveChangesAsync();

            return Ok(new { Message = "User address updated successfully" });
        }
    }
}
