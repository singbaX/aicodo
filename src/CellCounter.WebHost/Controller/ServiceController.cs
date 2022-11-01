using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CellCounter.WebHost.Controller
{
    [ApiController]
    public class ServiceController : ControllerBase
    {
        [HttpGet]
        [Route("/Service/Names")]
        public IActionResult Index()
        {
            var content = ServiceMethods.GetMethods();
            return Ok(content);
        }

        [HttpGet]
        [Route("/Service/{methodName}")]
        public IActionResult Get(string methodName)
        {
            if (!ServiceMethods.HasMethod(methodName))
            {
                return NotFound();
            }

            var args = new Dictionary<string, string>();
            foreach (var r in Request.Query)
            {
                args.Add(r.Key, r.Value);
            }

            var content = ServiceMethods.Request(methodName, args, "");
            return Ok(content);
        }

        [HttpPost]
        [Route("/Service/{methodName}")]
        public IActionResult Post(string methodName)
        {
            if (!ServiceMethods.HasMethod(methodName))
            {
                return NotFound();
            }

            var args = new Dictionary<string, string>();
            foreach (var r in Request.Query)
            {
                args.Add(r.Key, r.Value);
            }

            var data = Request.Body.ReadToEnd();
            var content = ServiceMethods.Request(methodName, args, "");
            return Ok(content);
        }
    }
}
