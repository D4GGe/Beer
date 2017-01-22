using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BeerImports;
using BeerImports.sources;

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

        [HttpGet]
        [Route("Update")]
        public string Update()
        {
            PubImports.UpdatePubs();
            return "true";
        }
    }
    

}
