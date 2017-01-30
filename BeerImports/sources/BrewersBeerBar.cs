using System;
using System.Net.Http;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
namespace BeerImports.sources
{
    class BrewersBeerBar
    {
        public static string html;

        public static async Task DownloadTaps(string url, string path)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(url);
                    var response = await client.GetAsync(path);
                    response.EnsureSuccessStatusCode();

                    html = await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request exception: {e.Message}");
                }
            }
        }

        public static void ImportTaps()
        {
            DownloadTaps("https://brewersbeerbar.se/", "").Wait();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNodeCollection tapHTML = doc.GetElementbyId("menu-collapse").SelectSingleNode("ul").SelectNodes("li");

            string path = "";
            foreach (var li in tapHTML){
                if (li.InnerHtml.Contains("BEERS ON TAP")){
                    path = li.SelectSingleNode("a").Attributes["href"].Value;
                }
            }

            if (path != "") {
                DownloadTaps("https://brewersbeerbar.se/", path).Wait();
                doc.LoadHtml(html);

                tapHTML = doc.GetElementbyId("alacarte-order-table").SelectSingleNode("div").SelectNodes("div");

                var taplist = new List<Beer>();

                foreach (var dom in tapHTML)
                {
                    HtmlNode nameHTML = dom.SelectSingleNode("div/div/div/h4");
                    HtmlNode priceHTML = dom.SelectSingleNode("div/div/div/p");

                    if (nameHTML != null)
                    {
                        string[] strings = nameHTML.InnerText.Replace('\t',' ').Split('\n');

                        if (strings.Length == 3)
                        {
                            string name = strings[0].Trim();
                            string brewery = strings[1].Trim();
                            string typeabv = strings[2].Trim();
                            string type = typeabv.Substring(0, typeabv.LastIndexOf(' '));
                            string sabv = typeabv.Substring(typeabv.LastIndexOf('(')+1, typeabv.LastIndexOf(')')-typeabv.LastIndexOf('(')-2);
                            double abv = double.Parse(sabv.Replace(',', '.'),new CultureInfo("en-US"));
                            string sprice = priceHTML.InnerText;
                            int price = int.Parse(sprice.Substring(sprice.LastIndexOf(" ")+1));

                            var temptap = new Beer
                            {
                                Name = name,
                                Brewery = brewery,
                                Type = type,
                                Price = price,
                                AlcholPrecentage = abv
                            };
                            taplist.Add(temptap);
                        }
                    }
                }

                PubImports.importedPubs.Add(new Pub
                {
                    Name = "Brewers Beer Bar",
                    Adress = "Tredje Långgatan 8, Göteborg 41303",
                    Beers = taplist

                });
            }
        }
    }
}
