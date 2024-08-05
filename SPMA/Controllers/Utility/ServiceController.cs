using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SPMA.Data;
using SPMA.Dtos.Core;
using SPMA.Models.Core;
using SPMA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPMA.Controllers.Utility
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : Controller
    {
        private readonly SubOrderRWCompletionCheckService _subOrderRWCompletionCheckService;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ServiceController(SubOrderRWCompletionCheckService subOrderRWCompletionCheckService, ApplicationDbContext context, IMapper mapper)
        {
            _subOrderRWCompletionCheckService = subOrderRWCompletionCheckService;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Enables/disables service for aquiring suborders rw completion 
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        [HttpPost("SubOrderRWCompletionCheck")]
        public IActionResult SubOrderRWStateToggle(bool serviceStatusChange)
        {
            // Get http request headers
            //bool serviceStatus = Convert.ToBoolean(HttpContext.Request.Headers["serviceStatusChange"]);
            Task task;
            if (serviceStatusChange)
            {
               task = _subOrderRWCompletionCheckService.StartAsync(new CancellationToken());
            }
            else
            {
              task = _subOrderRWCompletionCheckService.StopAsync(new CancellationToken());
            }
            return Ok(task.Status);
        }

        [HttpGet("SubOrderRWCompletionCheck/currentjob")]
        public ActionResult CheckSubOrderRWStateServiceJob()
        {
            return Content(_subOrderRWCompletionCheckService.Job, "text/plain");
        }
        /// <summary>
        /// Checks if service is running
        /// </summary>
        /// <returns></returns>
        [HttpGet("SubOrderRWCompletionCheck/status")]
        public IActionResult CheckSubOrderRWStateServiceStatus()
        {
            return Ok(_subOrderRWCompletionCheckService.IsRunning);
        }

        [HttpGet]
        public IActionResult GetServices()
        {
            var servicesDb = _context.Services.ToList();
            List<ServiceDto> services = _mapper.Map<List<Service>, List<ServiceDto>>(servicesDb);
            return Ok(services);
        }
    }
}
