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
        private static string DiscordToken => TokenHolder.DiscordToken;
        private static ulong GuildId => TokenHolder.GuildId;
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

            //-+-+-+-+-+-+-+-+
            // Clear Commands
            // .. if not, it will retain already deleted commands
            // .. or fail to update with new logic
            //-+-+-+-+-+-+-+-+
            await guild.DeleteApplicationCommandsAsync();

            //-+-+-+-+-+-+-+-+
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
                    await Task.Run(() => respond.Value(command));
            }
        }
        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}