using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SKTestBot.Commands
{
    class CommandsFinviz : CommandBaseSK
    {

        [Command("c3")]
        [Description("Returns 3 min chart for provided ticker. Ex: '!c3 spy'")]
        public async Task C3(CommandContext ctx, [Description("Company symbol")] string ticker)
        {
            string uri = $"https://charts.finviz.com/chart.ashx?t={ticker}&ty=c&ta=0&p=i3&s=l";
            
            await ctx.Channel.SendFileAsync(ProcessImageAsString(uri)).ConfigureAwait(false);
        }

        [Command("cc")]
        [Description("Returns 5 min chart for provided ticker. Ex: '!cc spy'")]
        public async Task C5(CommandContext ctx, [Description("Company symbol")] string ticker)
        {
            string uri = $"https://charts.finviz.com/chart.ashx?t={ticker}&ty=c&ta=0&p=i5&s=l";

            await ctx.Channel.SendFileAsync(ProcessImageAsString(uri)).ConfigureAwait(false);
        }

        [Command("c15")]
        [Description("Returns 15 min chart for provided ticker. Ex: '!c15 spy'")]
        public async Task C15(CommandContext ctx, [Description("Company symbol")] string ticker)
        {
            string uri = $"https://charts.finviz.com/chart.ashx?t={ticker}&ty=c&ta=0&p=i15&s=l";

            await ctx.Channel.SendFileAsync(ProcessImageAsString(uri)).ConfigureAwait(false);
        }

        [Command("cd")]
        [Description("Returns daily chart for provided ticker. Ex: '!cd spy'" +
            "Light orange line = 50 SMA." +
            "Dark orange line = 200 SMA." +
            "Yellow line = 20 SMA.")]
        public async Task Cd(CommandContext ctx, [Description("Company symbol")] string ticker)
        {
            string uri = $"https://charts.finviz.com/chart.ashx?t={ticker}&ty=c&ta=st_c,sch_200,sma_50,sma2_200,sma_20,rsi_b_14&p=d&s=l";

            await ctx.Channel.SendFileAsync(ProcessImageAsString(uri)).ConfigureAwait(false);
        }

        [Command("cw")]
        [Description("Returns weekly chart for provided ticker. Ex: '!cw spy'")]
        public async Task Cw(CommandContext ctx, [Description("Company symbol")] string ticker)
        {
            string uri = $"https://charts.finviz.com/chart.ashx?t={ticker}&ty=c&ta=0&p=w&s=l";

            await ctx.Channel.SendFileAsync(ProcessImageAsString(uri)).ConfigureAwait(false);
        }

        [Command("cm")]
        [Description("Returns monthly chart for provided ticker. Ex: '!cm spy'")]
        public async Task Cm(CommandContext ctx, [Description("Company symbol")] string ticker)
        {
            string uri = $"https://charts.finviz.com/chart.ashx?t={ticker}&ty=c&ta=0&p=m&s=l";

            await ctx.Channel.SendFileAsync(ProcessImageAsString(uri)).ConfigureAwait(false);
        }

        [Command("es")]
        [Description("Returns monthly chart for provided ticker. Ex: '!cm spy'")]
        public async Task Es(CommandContext ctx, string chart = "")
        {
            string  uri = "";

            if (chart == "")
            {
                uri = $"https://elite.finviz.com/futures.ashx";
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                    var content = client.DownloadString(uri).Split("S&P 500");

                    string change = "";
                    string currentValue = "";

                    foreach (string line in content)
                    {
                        // currently the 3rd (last) line of content returns the correct value so we'll just leave it as is
                        change = Regex.Match(line, @"(?<=\""change\"":).\d{0,2}.\d{0,2}").Value;
                        currentValue = Regex.Match(line, @"(?<=\""ES\"",\""last\"":)\d{4}.\d{0,2}").Value;
                    }

                    await ctx.Channel.SendMessageAsync($"SPX price : {currentValue} || change : {change}").ConfigureAwait(false);
                }
            }
            else
            {
                uri = $"https://elite.finviz.com/fut_chart.ashx?t=ES&cot=138741,13874A&p=m5&rev=637218079143208285";
                await ctx.Channel.SendFileAsync(ProcessImageAsString(uri)).ConfigureAwait(false);
            }
        }
    }

    public class Company
    {
        public string Ticker { get; set; }
        public string CompanyName { get; set; }
        public string Sector { get; set; }
        public string Industry { get; set; }
        public string NextEarnings { get; set; }
        public string Price { get; set; }
        public string Float { get; set; }
        public string ShortFloat { get; set; }
        public string MarketCap { get; set; }

        public readonly string Result;

        public Company(string ticker, string content)
        {
            Ticker = ticker;
            CompanyName = ParseCompanyName(content, ticker);
            //Industry = ParseIndustry(content);
            //Sector = ParseSector(content);
            //NextEarnings = ParseNextEarnings(content);
            //Price = ParsePrice(content);
            Float = ParseFloat(content);
            ShortFloat = ParseShortFloat(content);
            MarketCap = ParseMarketCap(content);

            StringBuilder sb = new StringBuilder();

            sb.Append("```");
            sb.Append($"Data for {CompanyName}");
            //sb.AppendLine($"Industry: {Industry}");
            //sb.AppendLine($"Sector: {Sector}");
            //sb.AppendLine($"NextEarnings: {NextEarnings}");
            sb.AppendLine($"Price: {Price}");
            sb.AppendLine($"Float: {Float}");
            sb.AppendLine($"ShortFloat: {ShortFloat}");
            sb.AppendLine($"MarketCap: {MarketCap}");
            sb.Append("```");

            Result = sb.ToString(); 
        }

        private string ParseCompanyName(string content, string ticker)
        {
            var pattern = "(?<=" + ticker + " ).*(?= Stock Quote)";
            return Regex.Match(@content, pattern).Value;
        }

        //private string ParseIndustry(string content)
        //{
        //    var pattern = "ind_.* ";
        //    var match = Regex.Match(content, pattern).Value;
        //    var result = match.Substring(0, match.IndexOf("\"")).Replace("ind_", "");

        //    return Strings.StrConv(result, VbStrConv.ProperCase);
        //}

        //private string ParseSector(string content)
        //{
        //    var pattern = "sec_.* ";
        //    var match = Regex.Match(content, pattern).Value;
        //    var result = match.Substring(0, match.IndexOf("\"")).Replace("sec_", "");

        //    return Strings.StrConv(result, VbStrConv.ProperCase);
        //}

        //private string ParseNextEarnings(string content)
        //{
        //    var pattern = "After Market Close] .*</b>";
        //    var match = Regex.Match(content, pattern).Value;
        //    var start = match.LastIndexOf("<b>") + 3;
        //    var length = match.Length - start - 4;

        //    return match.Substring(start, length);
        //}

        private string ParsePrice(string content)
        {
            var pattern = @"Current stock price] .*Analysts' mean";
            string match = Regex.Match(@content, pattern).Value;
            match = Regex.Match(@match, "<b>.*</b>").Value;
            match = match.Replace(@match, "<b>");
            match = match.Replace(@match, "</b>");
            return match;
        }

        private string ParseFloat(string content)
        {
            var pattern = "Shares float";
            var match = Regex.Match(@content, pattern).Value;
            pattern = @"\d{0,5}.\d{0,3}\w</b>";
            match = Regex.Match(@match, pattern).Value;
            match = match.Replace("</b>", "");
            return match;
        }

        private string ParseShortFloat(string content)
        {
            var pattern = @"Short interest share] .*(?<=Perf Quarter)";
            var match = Regex.Match(@content, pattern).Value;
            match = Regex.Match(@match, "<b>.*</b>").Value;
            match = match.Replace("<b>", "");
            match = match.Replace("</b>", "");
            return match;
        }

        private string ParseMarketCap(string content)
        {
            var pattern = @"Market capitalization] .*\[Forward";
            var match = Regex.Match(@content, pattern).Value;
            match = Regex.Match(@match, "<b>.*</b>").Value;
            match = match.Replace("<b>", "");
            match = match.Replace("</b>", "");
            return match;
        }

    }

}
