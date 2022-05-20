// PuppeteerSharp nuget //
using HtmlAgilityPack;
using PuppeteerSharp;
using ScrapySharp.Extensions;
using System.Diagnostics;
using System.Net;

string url = "https://listado.mercadolibre.com.mx/mezcladora#D[A:mezcladora,L:undefined]";
string chrome = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

try
{
    await GetPuppeteerSharpData();

    await GetWebRequetsData();

    GetScrapySharpData();
}
catch (Exception ex)
{

    throw;
}


async Task GetPuppeteerSharpData()
{
    Stopwatch stopwatch = Stopwatch.StartNew();
    stopwatch.Start();

    using var browserFetcher = new BrowserFetcher();
    await browserFetcher.DownloadAsync();

    await using var browser = await Puppeteer.LaunchAsync(
        new LaunchOptions
        {
            Headless = true,
            ExecutablePath = chrome
        }
    );

    await using var page = await browser.NewPageAsync();

    await page.GoToAsync(url);

    List<string> titles = new List<string>();

    var result = await page.EvaluateFunctionAsync(
       @"() => { 
            const a = document.querySelectorAll('.ui-search-item__title');
            const res = [];
            for (let i = 0; i < a.length; i++)
                res.push(a[i].innerHTML);
            return res;
        }");


    foreach (var item in result)
    {
        titles.Add(item.ToString());
    }

    foreach (var item in titles)
    {
        Console.WriteLine(item);
    }

    stopwatch.Stop();
    Console.WriteLine(Environment.NewLine + "Tiempo Toltal: " + stopwatch.Elapsed.ToString());

};

async Task GetWebRequetsData()
{
    Stopwatch stopwatch = Stopwatch.StartNew();
    stopwatch.Start();

    // Create a request for the URL.
    WebRequest request = WebRequest.Create(url);
    // If required by the server, set the credentials.
    request.Credentials = CredentialCache.DefaultCredentials;

    // Get the response.
    WebResponse response = request.GetResponse();
    // Display the status.
    Console.WriteLine(((HttpWebResponse)response).StatusDescription);

    // Get the stream containing content returned by the server.
    // The using block ensures the stream is automatically closed.
    using (Stream dataStream = response.GetResponseStream())
    {
        // Open the stream using a StreamReader for easy access.
        StreamReader reader = new StreamReader(dataStream);
        // Read the content.
        string responseFromServer = reader.ReadToEnd();
        int startIndex = responseFromServer.IndexOf("<div style=\"display:contents\"></div><div style=\"display:contents\"></div>");
        int endIndex = responseFromServer.IndexOf("<div style=\"display:contents\">");
        string data = responseFromServer.Substring(startIndex,endIndex);

        // Display the content.
        Console.WriteLine(data);
    }
    // Close the response.
    response.Close();

    stopwatch.Stop();
    Console.WriteLine(Environment.NewLine + "Tiempo Toltal: " + stopwatch.Elapsed.ToString());
}

void GetScrapySharpData()
{
    Stopwatch stopwatch = Stopwatch.StartNew();
    stopwatch.Start();

    HtmlWeb web = new HtmlWeb();
    HtmlDocument doc = web.Load(url);
    List<string> list = new List<string>();

    foreach (var item in doc.DocumentNode.CssSelect(".ui-search-item__title"))
    {        
        list.Add(item.InnerHtml);
    }

    list.ForEach(x => Console.WriteLine(x));

    stopwatch.Stop();
    Console.WriteLine(Environment.NewLine + "Tiempo Toltal: " + stopwatch.Elapsed.ToString());

}