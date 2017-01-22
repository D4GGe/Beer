using System;
using System.Collections.Generic;
using System.Text;

namespace BeerImports
{
    public enum BeerType {Stout,ImperialStout, Lager, IPA , PA,WheatBeer,SouerAle,RedAle,Belgian}
    public class Beer
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Price { get; set; }
        public string Text { get; set; }
        public string Brewery { get; set; }
        public double AlcholPrecentage { get; set; }
        public string Country { get; set; }
    }
}