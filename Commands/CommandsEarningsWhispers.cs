using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using OpenQA.Selenium.Chrome;

namespace SKTestBot.Commands
{
    class CommandsEarningsWhispers : CommandBaseSK
    {
        [Command("er")]
        [Description("Retrieves the next earnings date for the provided ticker. Ex: !er <ticker>.")]
        public async Task NextEarnings(CommandContext ctx, 
                                        string ticker)
        {
            ticker = ticker.ToUpper();
            string uri = $"https://www.earningswhispers.com/stocks/{ticker}";

            using (WebClient client = new WebClient())
            {
                try
                {
                    // need to add this user agent so we aren't blocked
                    client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                    string result = client.DownloadString(uri);
                    CompanyEpsData ced = new CompanyEpsData(ticker, result);
                    await ctx.Channel.SendMessageAsync(ced.FormattedResult).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    var msg = e.Message;
                    throw;
                }
                
            }

        }

        [Command("sent")]
        [Description("Offers the current analyst sentiments for the provided ticker. Ex: !sent <ticker>.")]
        public async Task Sentiment(CommandContext ctx,
                                     string ticker)
        {
            ticker = ticker.ToUpper();
            string uri = $"https://www.earningswhispers.com/stats-coanrec?symbol={ticker}";

            await ctx.Channel.SendFileAsync(ProcessImageAsString(uri), ticker + " Analyst Sentiment").ConfigureAwait(false);
        }

    }

    public class CompanyEpsData
    {
        public string Ticker { get; set; }
        public string ExpectedEps { get; set; }
        public string ConsensusEps { get; set; }
        public string ConsensusRevenue { get; set; }
        public string ReportDayDate { get; set; }
        private string _sourceText { get; set; }
        
        public readonly string FormattedResult;
        public CompanyEpsData(string ticker, string sourceText)
        {
            Ticker = ticker;
            _sourceText = sourceText;

            ParseExpectedEps();
            ParseConsensusEps();
            ParseConsensusRevenue();
            ParseReportDayDate();

            FormattedResult = Ticker + " will report on " + ReportDayDate +
                " with an expected eps of " + ExpectedEps + 
                ", consensus eps of "  + ConsensusEps +
                ", and revenue of " + ConsensusRevenue;
        }

        private void ParseExpectedEps()
        {
            string matchString = @"""mainitem\"">.{0,15}\d";
            ExpectedEps = System.Text.RegularExpressions.Regex.Match(_sourceText, matchString).Value;
            ExpectedEps = ExpectedEps.Replace(@"""mainitem"">", "");
        }

        private void ParseConsensusEps()
        {

            string matchString = @"Consensus:&nbsp;.{0,15}\d";
            ConsensusEps = System.Text.RegularExpressions.Regex.Match(_sourceText, matchString).Value;
            ConsensusEps = ConsensusEps.Replace("Consensus:&nbsp;", "");
            ConsensusEps = ConsensusEps.Replace("(", "");
            ConsensusEps = ConsensusEps.Replace(")", "");
        }

        private void ParseConsensusRevenue()
        {

            string matchString = @"(?<=Revenue.*)\$\d+(?:\.\d+)? \w*(?=<)";
            ConsensusRevenue = System.Text.RegularExpressions.Regex.Match(_sourceText, matchString).Value;
        }


        private void ParseReportDayDate()
        {
            string matchWeekday = System.Text.RegularExpressions.Regex.Match(_sourceText, @"(?<=""boxhead\\"">)\w*(?=<)").Value;
            string matchDate = System.Text.RegularExpressions.Regex.Match(_sourceText, @"\w{1,4} \d\d(?=<)").Value;
            string matchTime = System.Text.RegularExpressions.Regex.Match(_sourceText, @"\d+:\d+ \w\w \w\w(?=<)").Value;
            ReportDayDate = matchWeekday + ", " + matchDate + " at " + matchTime;
        }

    
           
        

    }
}
