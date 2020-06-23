using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using SKTestBot.Utilities.StringUtils;
using DSharpPlus.Entities;

namespace SKTestBot.Commands
{
    class CommandsFun : CommandBaseSK
    {
        [Command("draw")]
        [Description("Write something. Ex: '!draw hello world!'")]
        public async Task Draw(CommandContext ctx, params string[] textToDraw)
        {
            string fileName = string.Format(FilePaths.PATH_IMG_HOME,"drawnText.png");
            string output = string.Empty;

            try
            {
                FontFamily fontFamily = new FontFamily("Arial");
                Font font = new Font(
                   fontFamily,
                   24,
                   FontStyle.Bold,
                   GraphicsUnit.Pixel);

                foreach (string txt in textToDraw)
                {
                    output += txt + " ";
                }

                Image img = output.ToImage(font, Color.Green, Color.White);

                img.Save(fileName);
                await ctx.Message
                    .RespondWithFileAsync(string.Format(FilePaths.PATH_IMG_HOME, "drawnText.png"))
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                var chk = e.InnerException;
                throw;
            }
        }

        [Command("pls")]
        [Description("Don't do it")]
        public async Task PlsJkobs(CommandContext ctx, string plsCommand)
        {
            List<string> pics = new List<string>();
            string picsPath = FilePaths.PATH_PLS_JKOBS;

            pics.Add(string.Format(picsPath, "jkob1.png"));
            pics.Add(string.Format(picsPath, "jkob2.jpeg"));
            pics.Add(string.Format(picsPath, "jkob3.jpg"));
            pics.Add(string.Format(picsPath, "jkob4.jpg"));
            pics.Add(string.Format(picsPath, "jkob5.jpg"));
            pics.Add(string.Format(picsPath, "jkob6.jpg"));
            pics.Add(string.Format(picsPath, "jkob7.jpg"));
            pics.Add(string.Format(picsPath, "jkob8.jpg"));
            pics.Add(string.Format(picsPath, "jkob9.jpg"));
            pics.Add(string.Format(picsPath, "jkob10.jpg"));
            pics.Add(string.Format(picsPath, "jkob11.jpg"));
            pics.Add(string.Format(picsPath, "jkob12.jpg"));
            pics.Add(string.Format(picsPath, "jkob13.jpg"));
            pics.Add(string.Format(picsPath, "jkob14.jpg"));
            pics.Add(string.Format(picsPath, "jkob15.png"));
            pics.Add(string.Format(picsPath, "jkob16.png"));
            pics.Add(string.Format(picsPath, "jkob17.png"));
            pics.Add(string.Format(picsPath, "jkob18.png"));
            pics.Add(string.Format(picsPath, "jkob19.png"));
            pics.Add(string.Format(picsPath, "jkob20.png"));

            int randomNumber = new Random().Next(0,pics.Count-1);

            if (plsCommand == "jkobs")
                await ctx.Channel.SendFileAsync(pics[randomNumber]).ConfigureAwait(false);
            else if (plsCommand == "funnyJkob")
                // this is how you send gifs/images as embedded images
                if (randomNumber <= 6)
                {
                    await ctx.Message.RespondAsync(embed: new DiscordEmbedBuilder
                    {
                        ImageUrl = "removed gif link"
                    });

                    await ctx.Message.RespondAsync(embed: new DiscordEmbedBuilder
                    {
                        ImageUrl = "removed gif link"
                    });
                }
                else
                {
                    await ctx.Channel
                        .SendFileAsync(string.Format(FilePaths.PATH_IMG_HOME, "stopIt.png"))
                        .ConfigureAwait(false);
                }

        }
    }
}
