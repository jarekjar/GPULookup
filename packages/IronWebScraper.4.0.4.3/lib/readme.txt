Iron Web Scraper
===========================================
Iron Web Scraper is web scraping library for C#.   It allows developers to simulate and automate human browsing behavior in order to extract content, files and image content from web applications as native .Net objects.  Developers have the power to write and debug their own synchronous code. Iron Web Scraper will manage politeness and multithreading in the background, leaving your application easy to understand and maintain.
 Iron Web Scraper can be used to migrate content from existing websites as well as build search indexes and monitor website structure & content changes.

Functionality of Iron WebScraper:
===========================================
- Read and extract structured content from web pages using  html DOM, Javascript, Xpath and jQuery Style  CSS Selectors.
- Fast multi threading allows hundreds of simultaneous requests.
- Politely avoid over staring remote servers using IP and domain level trolling and optionally  respecting robots.txt
- Manage multiple identities, DNS, proxies, user agents, request methods, custom headers, cookies and logins.
- Data exported from websites becomes native C# objects which can be stored or used immediately.
- Exceptions managed in all but the developers own code. Errors and captchas auto retried on failure
- Save, pause, resume, autosave scrape jobs.
- Built in web cache allows for action replay, crash recovery, and querying existing web scrape data. Zipped to save disk space. - Change scrape logic on the fly and then action-replay your job without any internet traffic


Getting Installed
===========================================
- Requires: .Net Framework 4.5.2 or above
- Online Docs: http://ironsoftware.com/csharp/webscraper/help/  
- Download Dll and manual: http://localhost/Iron/ironsoftware.com/csharp/webscraper/packages/IronWebScraper.zip



Your first Web Scraper
===========================================
To build your first web scraper, we will extend the IronWebScraper.WebScraper class and add 2 methods:  
- Init() - sets up the scraper
- Parse(IronWebScraper.Response) - Tells the scraper how to process webpages.  You can have multiple of these Parse methods for different types of html page.  


using IronWebScraper


class BlogScraper : WebScraper
{
    public override void Init()
    {  	this.LoggingLevel = WebScraper.LogLevel.All;
        this.Request("https://blog.scrapinghub.com", Parse);
    }

    public override void Parse(Response response)
    {
        foreach (var title_link in response.Css("h2.entry-title a"))
        {
            string strTitle = title_link.TextContentClean;
            Scrape(new ScrapedData() { { "Title", strTitle } });
          }

        if (response.CssExists("div.prev-post > a[href]"))
        {
            var next_page = response.Css("div.prev-post > a[href]")[0].Attributes["href"];
            this.Request(next_page, Parse);
        }
    }
}

 To start it in a console application:
     

    static void Main(string[] args)
    {
        var ScrapeJob = new BlogScraper();
        ScrapeJob.Start();
    }  


Understanding the code:
===========================================
- The webscraper runs Init first.  This lets you set any properties elegantly, and also add our staring page using:   this.Request(url, Parse); 
- The Webscraper manages parallelism, http and threads… keeping all your code easy to debug and synchronous.
- The Parse method process the reponses.  This logic is entirely up to the developer in this case:
        - It finds a blog-post’s name using JQUery like CSS selectors (can also use JS DOM or Xpath)
        - It turns that data into a ScrapedData class instance - basically an easy to use genetic class.
        - You can also use custom classes to save data - which is better practice.
        - If saves the ScrapedData to a file.  The default working Directory is bin/Scrape/ 
        - It looks for “previous post links” in the blog page - and follows them again using Parse to process the result. This causes a chain reaction

Support
===========================================
- Requires .NET 4.5.2 or above
- Licensing & Support available for commercial deployments. 
- For code examples, documentation & more visit http://ironsoftware.com/cshapr/webscraper
= For support please email us at developers@ironsoftware.com
