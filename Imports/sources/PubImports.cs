using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Imports.sources
{
    public static class PubImports
    {
        public static List<Pub> importedPubs = new List<Pub>();
        public static void UpdatePubs()
        {
            importedPubs.Clear();
            ImportBishop("6").Wait();
            ImportBishop("7").Wait();
            ImportBishop("8").Wait();
            (new OlRepubliken()).Import();
            BrewDog.ImportTaps();
            BrewersBeerBar.ImportTaps();
        }
       public static async Task ImportBishop(string number)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://bishops-arms-ontap.azurewebsites.net/");
                    var response = await client.GetAsync($"/json/GetPub?pubId={number}&callback=angular.callbacks._1");
                    response.EnsureSuccessStatusCode(); // Throw in not success

                    var stringResponse = await response.Content.ReadAsStringAsync();
                    stringResponse = stringResponse.Substring(21);
                    stringResponse = stringResponse.Substring(0, stringResponse.Length - 2);

                    var posts = JsonConvert.DeserializeObject<Bishop>(stringResponse);
                    //Console.WriteLine($"Got {posts.Count()} posts");
                    // Console.WriteLine($"First post is {JsonConvert.SerializeObject(posts.First())}");
                    var beers = new List<Beer>();
                    foreach (var Bishbeer in posts.current.beers)
                    {
                        var tempBeer = new Beer
                        {
                            Name = Bishbeer.title,
                            Text = Bishbeer.description,
                            Type = Bishbeer.type,
                            AlcholPrecentage = Bishbeer.alcholPrecentage,
                            Brewery = Bishbeer.brewery
                        };
                        beers.Add(tempBeer);
                    }
                    importedPubs.Add(new Pub
                    {
                        Name = "Bishops Arms "+posts.name,
                        Adress = posts.address,
                        Beers = beers
                    });
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request exception: {e.Message}");
                }
            }
        }
    }
}
