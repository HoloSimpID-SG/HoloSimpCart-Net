using System.Diagnostics;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace HoloSimpID
{
    public class CartBot
    {
        private static readonly string DiscordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
        private static readonly ulong GuildId = ulong.Parse(Environment.GetEnvironmentVariable("GUILD_ID"));
        private static readonly ulong ThreadId = ulong.Parse(Environment.GetEnvironmentVariable("THREAD_ID"));
        private static readonly CancellationTokenSource cancellationTokenSource = new();

        //-+-+-+-+-+-+-+-+
        // Discord Component
        //-+-+-+-+-+-+-+-+
        public static DiscordSocketClient client { get; private set; }
        public static SocketGuild guild { get; private set; }
        public static SocketThreadChannel threadTesting { get; private set; }
        public static CommandService commands { get; private set; }
        public static CancellationToken cancellationToken => cancellationTokenSource.Token;

        public static async Task Main()
        {
            //-+-+-+-+-+-+-+-+
            // Load Database
            //-+-+-+-+-+-+-+-+
            try
            {
                Console.WriteLine("Checking DB EF Migration");
                await AppDbContext.EnsureMigrated();
                Console.WriteLine("DB EF Migration Complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during EF Migration:{ex.ToStringDemystified()}");
                throw;
            }
            //-+-+-+-+-+-+-+-+
            // Starting up the Discord Bot
            // ..these are event listeners
            //-+-+-+-+-+-+-+-+
            client = new DiscordSocketClient();
            commands = new CommandService();
            // .Log sends message to console
            // ..typically for errors.
            client.Log += Log;
            // .Ready tells when the bot is up and running,
            // ..so it contains the primary setup logic.
            client.Ready += ClientReady;
            // .SlashCommandExecuted gets fired whenever a command is received.
            client.SlashCommandExecuted += SlashCommandHandler;
            //-+-+-+-+-+-+-+-+

            //await DbHandler.InitializeDB();
            //await DbHandler.LoadDB();
            //-+-+-+-+-+-+-+-+

            //-+-+-+-+-+-+-+-+
            // Start
            //-+-+-+-+-+-+-+-+
            // Tells the bot to login
            await client.LoginAsync(TokenType.Bot, DiscordToken);
            await client.StartAsync();
            //-+-+-+-+-+-+-+-+

            //-+-+-+-+-+-+-+-+
            // Optional, but allows for more graceful shutdown
            //-+-+-+-+-+-+-+-+
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true; // Prevent immediate process termination
                cancellationTokenSource.Cancel();
            };
            AppDomain.CurrentDomain.ProcessExit += async (s, e) => { cancellationTokenSource.Cancel(); };
            //-+-+-+-+-+-+-+-+

            try
            {
                // Hold the application open until cancellation is requested
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Handle cancellation gracefully
                await threadTesting.SendMessageAsync("Hina, Nemui");

                //await DbHandler.SaveAllDB();
                await client.LogoutAsync();
                await client.StopAsync();
            }
        }

        private static async Task ClientReady()
        {
            //-+-+-+-+-+-+-+-+
            // Load the Guild and Thread/Channel
            //-+-+-+-+-+-+-+-+
            guild = client.GetGuild(GuildId);
            threadTesting = guild.GetThreadChannel(ThreadId);

            //-+-+-+-+-+-+-+-+-+
            // Starts Loading
            //-+-+-+-+-+-+-+-+-+
            await threadTesting.SendMessageAsync("Hina, Waking Up.\nPlease wait while I drink my coffee.");

            //-+-+-+-+-+-+-+-+
            // Commands Validation
            // ..so that you buffons don't forget :ayamewheeze:
            // Checks if each command have a response and vice-versa
            //-+-+-+-+-+-+-+-+
            #region Commands Validation
            var isValid = true;
            Console.WriteLine();
            Console.WriteLine("Running Command Validation.");
            List<string> commandName = new();
            foreach (SlashCommandBuilder command in CommandConsts.commands)
            {
                commandName.Add(command.Name);
                if (CommandConsts.responses.ContainsKey(command.Name))
                {
                    continue;
                }
                isValid = false;
                Console.WriteLine($"Command: {command.Name}, does not have a response.");
            }
            foreach (KeyValuePair<string, Func<SocketSlashCommand, Task>> respond in CommandConsts.responses)
            {
                if (commandName.Contains(respond.Key))
                {
                    continue;
                }
                isValid = false;
                Console.WriteLine($"Response: {respond.Key}, does not have a command.");
            }
            if (!isValid)
            {
                throw new Exception("Commands and Responses are not in sync Haiyahhh.");
            }
            #endregion
            //-+-+-+-+-+-+-+-+
            // Clear Commands
            // ..if not, it will retain already deleted commands
            // ..or fail to update with new logic
            //-+-+-+-+-+-+-+-+
            Console.WriteLine("Clearing Previous Commands");
            await client.Rest.DeleteAllGlobalCommandsAsync();
            await guild.DeleteApplicationCommandsAsync();

            //-+-+-+-+-+-+-+-+
            foreach (SlashCommandBuilder command in CommandConsts.commands)
            {
                try
                {
                    Console.WriteLine($"Registering {command.Name}");
                    await guild.CreateApplicationCommandAsync(command.Build());
                    Console.WriteLine($"Finished Registering {command.Name}");
                }
                catch (HttpException exception)
                {
                    string json = JsonConvert.SerializeObject(exception.ToStringDemystified(), Formatting.Indented);

                    Console.WriteLine("Something Broke, idk... this is the first time I am making a bot");
                    Console.WriteLine(json);
                }
                catch (Exception e)
                {
                    Console.WriteLine(
                        $"Something Broke, idk... this is the first time I am making a bot. Here's the message:\n{e.ToStringDemystified()}\nGo Figure or Go Vibe.");
                }
            }

            //-+-+-+-+-+-+-+-+-+
            // Finished Loading
            // ..Ready to respond
            //-+-+-+-+-+-+-+-+-+
            Console.WriteLine("App Finished Loading.");
            await threadTesting.SendMessageAsync("Hina caffeinated, ready to serve.");
        }

        private static async Task SlashCommandHandler(SocketSlashCommand command)
        {
            try
            {
                await CommandConsts.responses[command.Data.Name].Invoke(command);
            }
            catch (Exception e)
            {
                StringBuilder strErr = new();
                strErr.AppendLine("Error when performing command:");
                strErr.AppendLine($"{e.ToStringDemystified()}");
                Console.WriteLine(strErr.ToString());
            }
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}