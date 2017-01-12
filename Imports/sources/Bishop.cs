using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;


namespace Imports.sources
{
    public class BishBeerType
    {
        public int id { get; set; }
        public string name { get; set; }
        public int count { get; set; }
    }

   public class current
    {
        public int id { get; set; }
        public BishBeerType[] beertypes { get; set; }
        public BishBeer[] beers { get; set; }
    }


    public class BishBeer
    {
        public int id { get; set; }
        public string title { get; set; }
        public string brewery { get; set; }
        public string breweryId { get; set; }
        public string country { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string labelImage { get; set; }
        public string thumbImage { get; set; }
        public double alcholPrecentage { get; set; }

        public string ibu { get; set; }
        public string hopType { get; set; }
        public int beerStyleId { get; set; }
        public int beerTypeId { get; set; }
        public string beerType { get; set; }
        public int prio { get; set; }
        public string pubs { get; set; }
               
    }
    public class Bishop
    {
       public int id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string postaö { get; set; }
        public string city { get; set; }
        public string email { get; set; }
        public string pubimage { get; set; }
        public string phone { get; set; }
        public current current { get; set; }
        public current future { get; set; }

        

    }
}
