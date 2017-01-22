using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Imports;
using Imports.sources;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class Releases : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<SystembolagetRelease> Get()
        {
            return Systembolaget.releases;
        }
    }

    public class UpdateReleases : Controller
    {
        [HttpGet]
        public string Get()
        {
            Systembolaget.ImportReleases();
            return "true";
        }
    }
}
