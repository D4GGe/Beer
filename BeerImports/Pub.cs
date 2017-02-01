using System;
using System.Collections.Generic;
using System.Text;

namespace BeerImports
{
    public class Pub
    {
        public string Name { get; set; }
        public string Adress { get; set; }
        public List<Beer> Beers { get; set; } 
        public int id { get; set; }
    }
}
