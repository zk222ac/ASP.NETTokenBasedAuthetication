using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NETTokenBasedAuthetication.Controllers
{
    [ApiController]
    [Route("[controller]")]   
    [Authorize]
    public class HomeController : ControllerBase
    {
        public HomeController()
        {

        }

        [HttpGet]
        public IActionResult Get() 
        {
            return Ok("welcome to home controller");
        }

    }
}
