using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SPMA.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        public SystemController()
        {

        }

        //GET /api/system/
        [HttpGet]
        public IActionResult GetAssemblyInfo()
        {
            var version = GetType().Assembly.GetName().Version.ToString();

            return Ok(version);
        }
    }
}