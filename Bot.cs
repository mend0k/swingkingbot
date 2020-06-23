using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using System.IO;
using Newtonsoft.Json;
using SKTestBot.Commands;


namespace SKTestBot
{
    class Bot
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public async Task RunAsync()
        {
            ConfigJson configJson = DeserializeJsonConfig();

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = false,
                EnableMentionPrefix = true,
                DmHelp = true
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<CommandsFun>();
            Commands.RegisterCommands<CommandsBarChart>(); // deprecated
            Commands.RegisterCommands<CommandsFinviz>();
            Commands.RegisterCommands<CommandsEodData>();
            Commands.RegisterCommands<CommandsEarningsWhispers>();

            CommandsEarningsWhispers ced = new CommandsEarningsWhispers();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        private static ConfigJson DeserializeJsonConfig()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            return JsonConvert.DeserializeObject<ConfigJson>(json);
        }
    }
}
