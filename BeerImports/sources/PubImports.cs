using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace BeerImports.sources
{
    public static class PubImports
    {
        public static List<Pub> importedPubs = new List<Pub>();
        public static void UpdatePubs()
        {
            importedPubs.Clear();
            try{
            ImportBishop("6").Wait();
            }catch(Exception e ){
            }
            try{
            ImportBishop("7").Wait();
            }catch(Exception e ){
            }
            try{
            ImportBishop("8").Wait();
            }catch(Exception e ){
            }
            try{
            (new OlRepubliken()).Import();
            }catch(Exception e ){
            }
            try{
            BrewDog.ImportTaps();
            }catch(Exception e ){
            }
            try{
            BrewersBeerBar.ImportTaps();
            }catch(Exception e ){
            }
        }

        public static void insertPubDB(Pub pub){
            SqlConnection con = new SqlConnection("");
                        con.Open();
                        for(int a= 0;a<1000;a++ ){
                        using (SqlCommand command = new SqlCommand())
    {
        command.Connection = con;            // <== lacking
        command.CommandText = "INSERT into Sources (name,adress) VALUES (@name, @address)";
        command.Parameters.AddWithValue("@name", pub.Name);
        command.Parameters.AddWithValue("@address", pub.Adress);

  
            
            int recordsAffected = command.ExecuteNonQuery();
            Console.WriteLine("rowsE: "+recordsAffected);
            

    }
                        }
    con.Close();
            

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
