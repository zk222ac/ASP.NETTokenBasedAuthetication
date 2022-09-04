using ASP.NETTokenBasedAuthetication.Data;
using ASP.NETTokenBasedAuthetication.Data.Models;
using ASP.NETTokenBasedAuthetication.Data.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASP.NETTokenBasedAuthetication.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        // UserManager which provides API for managing users in a persistance store ( SQL Serever Database)
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthenticationController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext dbContext, 
            IConfiguration configuration,
            TokenValidationParameters tokenValidationParameters)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] RegisterVm registerVm) 
        {
            if (!ModelState.IsValid)
                return BadRequest("Please provide all the request");

            var existUser  = await _userManager.FindByEmailAsync(registerVm.Email);
            if (existUser != null)
            {
                return BadRequest($"User {registerVm.Email} already Exists");
            }

            // here we register new User
            ApplicationUser newUser = new ApplicationUser
            {
                FirstName = registerVm.FirstName,
                LastName = registerVm.LastName,
                Email = registerVm.Email,
                UserName = registerVm.UserName,
                SecurityStamp = Guid.NewGuid().ToString()                
            };

            var result = await _userManager.CreateAsync(newUser , registerVm.Password);
            if (result.Succeeded) return Ok("User is Created succesfully");
            return BadRequest("User has not been Created");
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromBody] LoginVm loginVm)
        {
            if (!ModelState.IsValid)
                return BadRequest("Please provide all the request");

            var userExists = await _userManager.FindByEmailAsync(loginVm.Email);
            if (userExists != null && await _userManager.CheckPasswordAsync(userExists, loginVm.Password))
            {
                // When User successfully SIGN-IN , we generate token 
                var tokenValue = await GenerateJWTTokenAsync(userExists);
                return Ok(tokenValue);
            }
            return Unauthorized();
        }

        private async Task<AuthResultVm> GenerateJWTTokenAsync(ApplicationUser user)
        {
            // Claims provide here 
            var authClaims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(ClaimTypes.NameIdentifier, user.Id),
               new Claim(JwtRegisteredClaimNames.Email,user.Email),
               new Claim(JwtRegisteredClaimNames.Sub,user.Email),
               new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var authSignInKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));
            
            var securityToken = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(1),
                claims:authClaims,
                signingCredentials: new SigningCredentials(authSignInKey,SecurityAlgorithms.HmacSha256)                
                );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(securityToken);

            // Refresh Token 
            var refreshToken = new RefreshToken
            {
                JwtId = securityToken.Id,
                IsRevoked = false,
                UserId = user.Id,
                DateAdded = DateTime.UtcNow,
                DateExpire = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString(),

            };
            // Add and save refreshtoken in database
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            // Create response 
            var response = new AuthResultVm
            { 
              Token = jwtToken,
              RefreshToken = refreshToken.Token,
              ExpireAt = securityToken.ValidTo
            };
            return  response;
        }
    }   
}
