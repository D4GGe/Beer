using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeerImports.sources;


namespace BeerImports
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PubImports.UpdatePubs();
            Systembolaget.ImportReleases();
            Console.Write("hej");
        }
    }
}
