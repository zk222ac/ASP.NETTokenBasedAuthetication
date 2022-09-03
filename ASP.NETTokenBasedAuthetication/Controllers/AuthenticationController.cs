using ASP.NETTokenBasedAuthetication.Data;
using ASP.NETTokenBasedAuthetication.Data.Models;
using ASP.NETTokenBasedAuthetication.Data.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions;

namespace ASP.NETTokenBasedAuthetication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        // UserManager which provides API for managing users in a persistance store ( SQL Serever Database)
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext dbContext,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _configuration = configuration;
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
                return Ok("User Signed-IN Succesfully ");
            }
            return Unauthorized();
        }
    }
}
