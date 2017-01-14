using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using HtmlAgilityPack;
using System.Threading.Tasks;

namespace Imports.sources
{
    class BrewDog
    {
        public static string html;

        public static async Task DownloadTaps()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("https://www.brewdog.com/");
                    var response = await client.GetAsync($"/bars/worldwide/goteborg");
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
            DownloadTaps().Wait();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            string tapstring = doc.GetElementbyId("tab1").InnerHtml;
            string[] listsplit = tapstring.Split(new string[] { "--- Guest Beer ---" }, StringSplitOptions.None);
            List<string> brewdogtaps = new List<string>(listsplit[0].Split(new string[] { "<br>" }, StringSplitOptions.None));
            List<string> guesttaps = new List<string>(listsplit[1].Split(new string[] { "<br>" }, StringSplitOptions.None));

            brewdogtaps.RemoveAt(0);
            brewdogtaps.RemoveAt(brewdogtaps.Count-1);
            guesttaps.RemoveAt(0);
            guesttaps.RemoveAt(guesttaps.Count-1);

            var taplist = new List<Beer>();

            foreach (var tap in brewdogtaps)
            {
                guesttaps.Add("BrewDog - " + tap);
            }

            foreach(var tap in guesttaps)
            {
                int splitpos = tap.LastIndexOf(" ");
                string sbreweryandname = tap.Substring(0, splitpos);

                double tapabv = 0;
                if (tap.Contains("%"))
                {
                    tapabv = double.Parse(tap.Substring(splitpos + 1, tap.Length - 2 - splitpos).Replace(",", "."));
                }

                string[] breweryandname = sbreweryandname.Split(new string[] { " - " }, StringSplitOptions.None);
                string brewery = breweryandname[0];
                string tapname = "";

                if (breweryandname.Length > 1)
                {
                    tapname = breweryandname[1];
                }

                var temptap = new Beer
                {
                    Name = tapname,
                    Brewery = brewery,
                    alcholPrecentage = tapabv
                };
                taplist.Add(temptap);
            }

            PubImports.importedPubs.Add(new Pub
            {
                Name = "BrewDog Bar Göteborg",
                Adress = "Kungsgatan 10B, Göteborg 41119",
                Beers = taplist

            });
        }
    }
}
