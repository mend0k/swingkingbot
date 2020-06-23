using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using SKTestBot.Api;
using Microsoft.Xrm.Sdk.Messages;
using Newtonsoft.Json.Linq;
using System.Linq;


namespace SKTestBot.Commands
{
    class CommandsEodData : CommandBaseSK
    {

        [Command("oi")]
        public async Task OpenInterest(CommandContext ctx,
            [Description("Company symbol")] string ticker)
        {

            ApiHandler.InitializeClient();
            string token = ConfigJson.EodDataToken;
            string uri = $"https://eodhistoricaldata.com/api/options/{ticker}?api_token={token}";
            //string uri = "https://api-v2.intrinio.com/companies/AAPL?api_key=OmExMmM2M2MwOGQwZDJjY2RlYjc2NWI4NjFjZGRkYjI4";
            try
            {
                using (HttpResponseMessage response = await ApiHandler.ApiClient.GetAsync(uri))
                {
                    var chk = response.IsSuccessStatusCode;
                    // [^}]* = stop at first instance of "}"
                    var options = System.Text.RegularExpressions.Regex.Matches(await response.Content.ReadAsStringAsync(), "(?={\"contractName\")[^}]*") ; 

                    // deserialize JSON results
                    var settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
                    List<Option> aggResults = new List<Option>();

                    foreach (System.Text.RegularExpressions.Match item in options)
                    {
                        Option option = JsonConvert.DeserializeObject<Option>(item.Value.ToString() + '}', settings);

                        aggResults.Add(option);
                    }

                    // group & organize results
                    List<OutputOpenInterest> oiResults = new List<OutputOpenInterest>();

                    foreach (dynamic group in aggResults.GroupBy(x => x.ExpirationDate))
                    {
                        List<Option> putOptions = new List<Option>();
                        List<Option> callOptions = new List<Option>();

                        foreach (dynamic item in group)
                        {
                            if (item.Type == "CALL")
                            {
                                callOptions.Add(item);
                            }
                            else
                            {
                                putOptions.Add(item);
                            }
                        }

                        // calculate final results
                        decimal sumOiCalls = callOptions.Sum(y => y.OpenInterest);
                        decimal sumOiPuts = putOptions.Sum(y => y.OpenInterest);
                        decimal oiTotal = sumOiCalls + sumOiPuts;

                        string percentCalls = "0";
                        string percentPuts = "0";

                        if (oiTotal > 0)
                        {
                            percentCalls = decimal.Round(((sumOiCalls / oiTotal) * 100)).ToString() + "%";
                            percentPuts = decimal.Round(((sumOiPuts / oiTotal) * 100)).ToString() + "%";
                        }

                        oiResults.Add(new OutputOpenInterest() { Date = group.Key, OI = (int)oiTotal, Call = percentCalls, Put = percentPuts });
                    }

                    // format the output
                    StringBuilder sb = new StringBuilder();
                    sb.Append("```");
                    sb.AppendLine("|    Date   |     OI    | call % | put % |");
                    sb.AppendLine("+-----------+-----------+--------+-------+");
                    foreach (OutputOpenInterest oi in oiResults)
                    {
                        sb.AppendLine($"|{string.Format("{0,10}", oi.Date.ToString("dd-MMM-yy"))} " +
                            $"|{string.Format("{0,10}", string.Format("{0:n0}", oi.OI))} " +
                            $"|{string.Format("{0,7}", oi.Call)} " +
                            $"|{string.Format("{0,6}", oi.Put)} |");
                    }
                    sb.Append("+-----------+-----------+--------+-------+");
                    sb.Append("```");


                    // send it to channel
                   await ctx.Channel.SendMessageAsync(sb.ToString()).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                var cgk = e.InnerException;
                throw;
            }
        }
    }

    public class OutputOpenInterest
    {
        public DateTime Date { get; set; }
        public int OI { get; set; }
        public string Call { get; set; }
        public string Put { get; set; }
    }
    

    public class Option
    {
        public string Type { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal Strike { get; set; }
        public int Volume { get; set; }
        public decimal OpenInterest { get; set; }
        public decimal ImpliedVolatility { get; set; }
        public decimal Theta { get; set; }

    }

}
