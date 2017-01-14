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
    public class Beers : Controller
    {

        [HttpGet]
        public IEnumerable<Pub> Get()
        {
            return PubImports.importedPubs;
        }

        
    }
    public class UpdateBeers : Controller
    {
        [HttpGet]
        public string Get()
        {
            PubImports.UpdatePubs();
            return "true";
        }
    }

}
