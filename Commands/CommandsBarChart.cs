using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Text;
using SKTestBot.Api;
using SKTestBot.Utilities.StringUtils;
using System.Drawing;

namespace SKTestBot.Commands
{
    // ********************DEPRECATED AFTER GETTING EoD DATA API*********************
    class CommandsBarChart : CommandBaseSK
    {
        private int maxQueries = 3;
        private int currentQueries = 0;

        // TODO:
        // save to memory user who queried. 
        // check if user who queried is still running prev query
        // if so return exception

        [Command("oibc")]
        [Description("To use: !oi <ticker> <results>. Ex: '!oi MSFT 2'")]
        public async Task OpenInterest(CommandContext ctx, 
            [Description("Company symbol")]string ticker, 
            [Description("The amount of dates to return. Default is 1, max is 10.")]int resultsLimit = 1)
        {
            if (currentQueries <= maxQueries -1)
            {
                // max out results at 10
                if (resultsLimit > 10)
                    resultsLimit = 10;

                currentQueries++;

                try
                {
                    if (currentQueries == 1)
                    {
                        System.Threading.Thread.Sleep(500);
                    }
                    else if(currentQueries == 2)
                    {
                        System.Threading.Thread.Sleep(1200);
                    }
                    else if(currentQueries == 3)
                    {
                        System.Threading.Thread.Sleep(2500);
                    }


                    await ScalpOpenInterestData(ctx, ticker.ToUpper(), resultsLimit);
                }
                catch (Exception e)
                {
                    await ctx.Channel.SendMessageAsync($"Error: {e.Message}.").ConfigureAwait(false);
                    currentQueries--;
                    throw;
                }
                
            }
            else
            {
                await ctx.Channel.SendMessageAsync("Queries maxed out. Please try again in a momnent.").ConfigureAwait(false);
            }
        }
 
        private async Task ScalpOpenInterestData(CommandContext ctx, string ticker, int resultsLimit)
        {
            await ctx.Channel.SendMessageAsync($"Initializing... please wait").ConfigureAwait(false);

            // load web page and configure chromeDriver.
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("start-maximized");
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--disable-extensions");

            ChromeDriver chromeDriver = new ChromeDriver(chromeOptions);
            string uri = $"https://www.barchart.com/stocks/quotes/{ticker}/options";

            try
            {
                chromeDriver.Navigate().GoToUrl(uri);
            }
            catch (Exception e)
            {
                throw e;
            }
            
            // find all expiration dates 
            var listItems = chromeDriver.FindElements(By.XPath("//div[@class='clearfix left bc-dropdown filter expiration-name']//*//*[contains(@data-ng-repeat,'')]"));
                                    
            int maxItems = listItems.Count;
            int currentCount = 0;

            // make sure we don't try to return more than the total amount
            if (resultsLimit <= listItems.Count)
                maxItems = resultsLimit;

            await ctx.Channel.SendMessageAsync($"Loading {maxItems} results... please wait").ConfigureAwait(false);

            // select first 3 dates
            List<string> dates = new List<string>();
            foreach (IWebElement item in listItems)
            {
                if (currentCount < maxItems)
                {
                    dates.Add(item.Text);
                    currentCount++;
                }
            }

            //get oi of selected dates
            Dictionary<string, string> oi = new Dictionary<string, string>();
         
            foreach (string date in dates)
            {
                // need this here for loading stability 
                System.Threading.Thread.Sleep(1000);

                if (maxItems > 2)
                {
                    System.Threading.Thread.Sleep(2000);
                }
                else if (currentQueries > 5)
                {
                    System.Threading.Thread.Sleep(4000);
                }

                chromeDriver.Url = uri + $"?expiration={ date }";

                oi.Add(date,chromeDriver.FindElementByXPath("//div[@class='column small-12 medium-6'][2]").Text);
            }

            string oiPuts = "(?<=\\r\\n)(.*)(?=\\r\\nC)";
            string oiCalls = "(?<=Call Open Interest Total\\r\\n)(.*)(?=\\r\\nP)";
            StringBuilder sb = new StringBuilder();

            string columnFormat = "|         {0}         |     {1}      |    {2}    |    {3}    |";

            sb.AppendLine($"                           Open Interest for [{ticker}]");
            sb.AppendLine(String.Format(columnFormat, "      Date     ", "    oi   ", "call%", "put%"));
            
            foreach (KeyValuePair<string, string> str in oi)
            {
                try
                {
                    string date = str.Key.Replace("(w)", "").Trim();
                    date = str.Key.Replace("(m)", "").Trim();
                    string oiTypeAmount = str.Value;

                    string oiPut = System.Text.RegularExpressions.Regex.Match(oiTypeAmount, oiPuts).Value.Replace(",", "");
                    string oiCall = System.Text.RegularExpressions.Regex.Match(oiTypeAmount, oiCalls).Value.Replace(",", "");

                    decimal oiPutValue, oiCallValue;

                    //convert string values to integer
                    decimal.TryParse(oiPut, out oiPutValue);
                    decimal.TryParse(oiCall, out oiCallValue);

                    decimal totalOi = oiPutValue + oiCallValue;
                    decimal percentPut = oiPutValue / totalOi;
                    decimal percentCall = oiCallValue / totalOi;

                    var outputPuts = string.Format("{0}% ", Decimal.ToInt32(percentPut * 100));
                    var outputCalls = string.Format("{0}% ", Decimal.ToInt32(percentCall * 100));

                    sb.AppendLine(String.Format(columnFormat, date, totalOi, outputCalls, outputPuts));
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    chromeDriver.Quit();
                    currentQueries--;
                }
            }


            // convert to string result to image
            string fileName = string.Format(FilePaths.PATH_IMG_HOME, "imgOi.png");

            FontFamily fontFamily = new FontFamily("Arial");
            Font font = new Font(
               fontFamily,
               16,
               FontStyle.Regular,
               GraphicsUnit.Pixel);


            Image img = sb.ToString().ToImage(font, Color.Gold, Color.Black); 

            img.Save(fileName);

            await ctx.Message.RespondWithFileAsync(fileName).ConfigureAwait(false);
        }
   }

    

}
