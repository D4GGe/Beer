using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.Generic;
using OfficeOpenXml;
using System.IO;
using System.Globalization;

namespace BeerImports.sources
{
    public class SystembolagetRelease
    {
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public List<Beer> Beers { get; set; }
    }

    public class Systembolaget
    {
        public static string html;
        public static List<SystembolagetRelease> releases = new List<SystembolagetRelease>();

        public static async Task DownloadHtml()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("https://www.systembolaget.se/");
                    var response = await client.GetAsync("/fakta-och-nyheter/nyheter-i-sortimentet/lanseringar/");
                    response.EnsureSuccessStatusCode();

                    html = await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request exception: {e.Message}");
                }
            }
        }

        public static void ImportReleases()
        {
            DownloadHtml().Wait();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode node = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'col-lg-8')]");
            HtmlNodeCollection nodes = node.ChildNodes;

            var releasepaths = new List<String>();
            foreach (HtmlNode cnode in node.ChildNodes)
            {
                if (cnode.InnerText.Contains("Excel: s") || cnode.InnerText.Contains("Excel: w"))
                {
                    foreach(HtmlNode a in cnode.SelectNodes("./a"))
                    {
                        if (a.OuterHtml.Contains(".xlsx"))
                        {
                            releasepaths.Add(a.Attributes["href"].Value);
                        }
                    }
                }
            }

            
            foreach (string releasepath in releasepaths)
            {
                using (var client = new HttpClient())
                {
                    string uri = "http://www.systembolaget.se" + releasepath;
                    var bytes = client.GetByteArrayAsync(uri).Result;
                    var xlsxstream = new MemoryStream(bytes, true);
                    ExcelPackage pck = new ExcelPackage(xlsxstream);
                    ExcelWorksheet ws = pck.Workbook.Worksheets[1];

                    var beerlist = new List<Beer>();
                    for (int row = 3; row <= ws.Dimension.End.Row; row++)
                    {
                        if (ws.Cells[row, 5].Text == "ÖL MM")
                        {
                            var tempbeer = new Beer
                            {
                                Name = ws.Cells[row, 2].Text,
                                Brewery = ws.Cells[row, 4].Text,
                                Country = ws.Cells[row, 6].Text,
                                Price = (int)Math.Round(double.Parse(ws.Cells[row, 7].Text))
                            };
                            beerlist.Add(tempbeer);
                        }
                    }

                    string name = ws.Name;
                    string type = "Små partier";
                    string sdate = name.Substring(12);

                    if (name.Contains("Webblansering"))
                    {
                        type = "Webblansering";
                        sdate = name.Substring(14);
                    }

                    DateTime date = Convert.ToDateTime(sdate, new CultureInfo("sv-SE").DateTimeFormat).AddHours(10.0);

                    if (beerlist.Count > 0)
                    {
                        var temprelease = new SystembolagetRelease
                        {
                            Type = type,
                            Date = date,
                            Beers = beerlist
                        };
                        releases.Add(temprelease);
                    }
                }
            }
        }
    }
}
