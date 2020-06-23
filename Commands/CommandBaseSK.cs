using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace SKTestBot.Commands
{
    class CommandBaseSK : BaseCommandModule
    {
        protected readonly ConfigJson ConfigJson;
        public CommandBaseSK()
        {
            ConfigJson = DeserializeJsonConfig();
        }

        private static ConfigJson DeserializeJsonConfig()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            return JsonConvert.DeserializeObject<ConfigJson>(json);
        }


        protected string ProcessImageAsString(string uri)
        {
            int randomNumber = new Random().Next(0, 20);

            string filePath = string.Format(FilePaths.PATH_IMG_CHARTS, $"image{randomNumber}.png");

            using (WebClient client = new WebClient())
            {
                // need to add this user agent so we aren't blocked
                client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                client.DownloadFile(uri, filePath);
            }

            return filePath;
        }

    }
}
