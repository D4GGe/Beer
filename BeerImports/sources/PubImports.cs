using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Microsoft.SqlServer.Server;
namespace BeerImports.sources
{
    public static class PubImports
    {
        public static List<Pub> importedPubs = new List<Pub>();
        public static void UpdatePubs()
        {
            importedPubs.Clear();
            try
            {
                ImportBishop("6").Wait();
            }
            catch (Exception e)
            {
            }
            try
            {
                ImportBishop("7").Wait();
            }
            catch (Exception e)
            {
            }
            try
            {
                ImportBishop("8").Wait();
            }
            catch (Exception e)
            {
            }
            try
            {
                (new OlRepubliken()).Import();
            }
            catch (Exception e)
            {
            }
            try
            {
                BrewDog.ImportTaps();
            }
            catch (Exception e)
            {
            }
            try
            {
                BrewersBeerBar.ImportTaps();
            }
            catch (Exception e)
            {
            }
        }


        public static void updatePubDB(Pub pub)
        {
            List<SqlDataRecord> beers = new List<SqlDataRecord>();
            var metaData = new SqlMetaData[] {    new SqlMetaData("Name", SqlDbType.VarChar, -1), 
                                                  new SqlMetaData("Type", SqlDbType.VarChar,-1), 
                                                  new SqlMetaData("Price", SqlDbType.Int),
                                                  new SqlMetaData("Text", SqlDbType.Text),
                                                  new SqlMetaData("Brewery", SqlDbType.VarChar,-1),
                                                  new SqlMetaData("AlcholPrecentage", SqlDbType.Decimal,5,2),
                                                  new SqlMetaData("Country", SqlDbType.VarChar,-1) };

            

             foreach(Beer be in pub.Beers){
                 var record = new SqlDataRecord(metaData);
                // record.SetValues(new object[]{be.Name,be.Type,be.Price,be.Text,be.Brewery,Convert.ToSingle(be.AlcholPrecentage),be.Country});
                 record.SetString(0,be.Name);
                 record.SetString(1,be.Type??"");
                 record.SetInt32(2,be.Price);
                 record.SetString(3,be.Text ?? "");
                 record.SetString(4,be.Brewery ?? "");
                 record.SetDecimal(5,(decimal)be.AlcholPrecentage);
                 record.SetString(6,be.Country ?? "");
                 beers.Add(record);
             }   
             using (SqlConnection sql = NewSqlConnection())
            {
                using (SqlCommand cmd = sql.CreateCommand())
                {
                    SqlParameter beerParameter = new SqlParameter("@beerList", SqlDbType.Structured);
                    beerParameter.Value=beers;
      
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.UpdateSource";
                    
                    cmd.Parameters.Add(beerParameter);
                    cmd.Parameters.AddWithValue("@Adress",pub.Adress);
                    cmd.Parameters.AddWithValue("@sourceName",pub.Name);
                    sql.Open();
                    cmd.ExecuteNonQuery();
                }
            }

        }
        public static SqlConnection NewSqlConnection()
        {
            string SqlConnectionString = $"Data Source=.\\sqlexpress;Initial Catalog=beer;Integrated Security=True";
            SqlConnection sql = new SqlConnection(SqlConnectionString);
            return sql;
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
                        Name = "Bishops Arms " + posts.name,
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
