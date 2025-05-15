using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HoloSimpID
{
    public class CartBot
    {
        private const string DiscordToken = "MTM3MjIyNjM1NTg0ODI4NjM1MA.Gf7JSo.nIc4SuhcoqG1bp0mwd-gSxSRGO-j1-n_wvJ_kU";
        private const long GuildId = 854176842847354920;
        //-+-+-+-+-+-+-+-+
        // Discord Component
        //-+-+-+-+-+-+-+-+
        static DiscordSocketClient Client; public static DiscordSocketClient client => Client;
        static CommandService Commands; public static CommandService commands => Commands;

        public static async Task Main()
        {
            Client = new DiscordSocketClient();
            Commands = new CommandService();

            //-+-+-+-+-+-+-+-+
            // Logging
            //-+-+-+-+-+-+-+-+
            client.Log += Log;
            client.Ready += Client_Ready;
            client.SlashCommandExecuted += SlashCommandHandler;

            //-+-+-+-+-+-+-+-+
            // Start
            //-+-+-+-+-+-+-+-+
            await Client.LoginAsync(TokenType.Bot, DiscordToken);
            await Client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        public static async Task Client_Ready()
        {
            var guild = client.GetGuild(GuildId);
            try
            {
                foreach(var command in CommandConsts.commands)
                    await guild.CreateApplicationCommandAsync(command.Build());
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                Console.WriteLine("Something Broke, idk... this is the first time I am making a bot");
                Console.WriteLine(json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something Broke, idk... this is the first time I am making a bot. Here's the message:\n{e.Message}\nGo Figure or Go Vibe.");
            }
        }
        private static async Task SlashCommandHandler(SocketSlashCommand command)
        {
            foreach (var respond in CommandConsts.responses)
            {
                if (command.Data.Name == respond.Key)
                {
                    respond.Value(command);
                    await Task.Run(() => respond.Value(command));
                }
            }
        }
        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}