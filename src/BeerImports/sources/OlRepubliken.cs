using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Xfinium.Pdf;
using Xfinium.Pdf.Content;

namespace BeerImports.sources
{
    class OlRepubliken
    {
        public void Import()
        {
            string Result = "";
            var beerList =new List<Beer>();
            using (var client = new HttpClient())
            {
                var bytes = client.GetByteArrayAsync(@"http://media.olrepubliken.se/fatolAktiv.pdf").Result;
                var pdfInputStream = new MemoryStream(bytes, true);
                PdfFixedDocument document = new PdfFixedDocument(pdfInputStream);
                foreach (var page in document.Pages)
                {
                    PdfContentExtractor ce = new PdfContentExtractor(page);
                    Result = Result + ce.ExtractText();
                }
            }

            using (StringReader reader = new StringReader(Result))
            {
                reader.ReadLine();
                reader.ReadLine();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string line2 = reader.ReadLine();
                    
                    List<String> parsed = new List<string>();
                    char[] lineChars = line.ToCharArray();
                    int arrAyPointer = lineChars.Length-1;
                    while (parsed.Count < 3)
                    {
                        string Number = "";
                        while (!Char.IsNumber(lineChars[arrAyPointer]))
                        {
                            arrAyPointer--;
                        }
                        while(Char.IsNumber(lineChars[arrAyPointer]) || lineChars[arrAyPointer] == ',' | lineChars[arrAyPointer] == '.')
                        {
                            Number = Number + lineChars[arrAyPointer];
                            arrAyPointer--;
                        }
                        Number = Number.Replace(',', '.');
                        var reversedNumber = Number.ToCharArray();
                        Array.Reverse(reversedNumber);
                        parsed.Add(new string(reversedNumber));
                    }
                    string name = line.Substring(0, arrAyPointer);
                    string brew = "";
                    if (name.LastIndexOf('-') > 0)
                    {
                        brew = name.Substring(name.LastIndexOf('-')+1).Trim();
                        name = name.Substring(0, name.LastIndexOf('-')).Trim();
                        
                    }
                    beerList.Add(new Beer
                    {
                        Price = int.Parse(parsed[0]),
                        AlcholPrecentage = double.Parse(parsed[2]),
                        Name = name,
                        Text = line2,
                        Brewery = brew
                    });
                }
            }
            PubImports.importedPubs.Add(new Pub
            {
                Name = "Ölrepubliken",
                Adress= "Kronhusgatan 2B 41113 Göteborg",
                Beers = beerList
            });
        }
    }
}
