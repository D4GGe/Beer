using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Imports.sources;
class Program
{
    static void Main(string[] args)
    {
        


     
        Console.WriteLine(PubImports.importedPubs.ToArray().Length);
        Console.ReadLine();
    }

   
}