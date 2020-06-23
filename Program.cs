namespace SKTestBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
        }
    }

    public static class FilePaths
    {
        // NOTE: Not sure where to place these yet...
        public const string PATH_PLS_JKOBS = @"C:\Users\mend0k\Desktop\source\repos\SKTestBot\Images\PlsJkobs\{0}";
        public const string PATH_IMG_HOME = @"C:\Users\mend0k\Desktop\source\repos\SKTestBot\Images\{0}";
        public const string PATH_IMG_CHARTS = @"C:\Users\mend0k\Desktop\source\repos\SKTestBot\Images\charts\{0}";
    }
}

