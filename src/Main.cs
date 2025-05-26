using Azure;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;

namespace HoloSimpID
{
    public class CartBot
    {
        private static string DiscordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
        private static ulong GuildId = ulong.Parse(Environment.GetEnvironmentVariable("GUILD_ID"));
        private static ulong ThreadId = ulong.Parse(Environment.GetEnvironmentVariable("THREAD_ID"));
        private static string SqlConnection = Environment.GetEnvironmentVariable("SQL_CONNECTION");

        //-+-+-+-+-+-+-+-+
        // Discord Component
        //-+-+-+-+-+-+-+-+
        public static DiscordSocketClient client { get; private set; }
        public static SocketGuild guild { get; private set; }
        public static SocketThreadChannel threadTesting { get; private set; }
        public static CommandService commands { get; private set; }
        public static SqlConnection sqlConnection { get; private set; }
        public static CancellationTokenSource cancellationTokenSource { get; private set; } = new();
        public static CancellationToken cancellationToken => cancellationTokenSource.Token;

        public static async Task Main()
        {
            //connection = new SqlConnection(SqlConnection);
            client = new DiscordSocketClient();
            commands = new CommandService();

            //-+-+-+-+-+-+-+-+
            // Logging
            //-+-+-+-+-+-+-+-+
            client.Log += Log;
            client.Ready += ClientReady;
            client.SlashCommandExecuted += SlashCommandHandler;

            //-+-+-+-+-+-+-+-+
            // Load Database
            //-+-+-+-+-+-+-+-+
            //await LoadDB();

            //-+-+-+-+-+-+-+-+
            // Start
            //-+-+-+-+-+-+-+-+
            await client.LoginAsync(TokenType.Bot, DiscordToken);
            await client.StartAsync();

            // Block this task until the program is closed.
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true; // Prevent immediate process termination
                cancellationTokenSource.Cancel();
            };
            AppDomain.CurrentDomain.ProcessExit += async (s, e) =>
            {
                cancellationTokenSource.Cancel();
            };

            try
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Handle cancellation gracefully
                await threadTesting.SendMessageAsync("Hina, Nemui");

                await client.LogoutAsync();
                await client.StopAsync();
            }
            //await SaveDB();
        }

        public static async Task LoadDB()
        {
            using (sqlConnection)
            {
                await sqlConnection.OpenAsync();

                try
                {
                    Simp.DeserializeAll(sqlConnection);
                    Cart.DeserializeAll(sqlConnection);
                    Console.WriteLine("Database Loaded Succesfully, HUMU");
                }
                catch
                {
                    // DO NOTHING, this is the first time running the bot
                }
            }
        }

        public static async Task SaveDB()
        {
            List<SqlCommand> sqlCommands = new();
            sqlCommands.AddRange(Simp.SerializeAll());
            sqlCommands.AddRange(Cart.SerializeAll());

            using (sqlConnection)
            {
                await sqlConnection.OpenAsync();
                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        foreach (var cmd in sqlCommands)
                        {
                            cmd.Connection = sqlConnection;
                            cmd.Transaction = transaction;
                            await cmd.ExecuteNonQueryAsync();
                        }
                        transaction.Commit();
                        Console.WriteLine("Database Updated Successfully, HUMU");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error updating Database: " + ex.Message);
                    }
                }
            }
        }

        public static async Task ClientReady()
        {
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
            bool isValid = true;
            Console.WriteLine();
            Console.WriteLine($"Running Command Validation.");
            List<string> commandName = new();
            foreach (var command in CommandConsts.commands)
            {
                commandName.Add(command.Name);
                if(CommandConsts.responses.ContainsKey(command.Name))
                    continue;
                else
                {
                    isValid = false;
                    Console.WriteLine($"Command: {command.Name}, does not have a response.");
                }
            }
            foreach (var respond in CommandConsts.responses)
            {
                if (commandName.Contains(respond.Key))
                    continue;
                else
                {
                    isValid = false;
                    Console.WriteLine($"Response: {respond.Key}, does not have a command.");
                }
            }
            if (!isValid)
            {
                throw new Exception("Commands and Responses are not in sync Haiyahhh.");
                Console.WriteLine();
            }

            //-+-+-+-+-+-+-+-+
            // Clear Commands
            // ..if not, it will retain already deleted commands
            // ..or fail to update with new logic
            //-+-+-+-+-+-+-+-+
            Console.WriteLine("Clearing Previous Commands");
            await client.Rest.DeleteAllGlobalCommandsAsync();
            await guild.DeleteApplicationCommandsAsync();

            //-+-+-+-+-+-+-+-+
            foreach (var command in CommandConsts.commands)
            {
                try
                {
                    Console.WriteLine($"Registering {command.Name}");
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

            //-+-+-+-+-+-+-+-+-+
            // Finished Loading
            // ..Ready to respond
            //-+-+-+-+-+-+-+-+-+
            await threadTesting.SendMessageAsync("Hina caffeinated, ready to serve.");
        }

        private static async Task SlashCommandHandler(SocketSlashCommand command)
        {
            try
            {
                Console.WriteLine("Received slash command: " + command.Data.Name);
                await Task.Run(() => CommandConsts.responses[command.Data.Name].Invoke(command));
            }
            catch (Exception e)
            {
                StringBuilder strErr = new();
                strErr.AppendLine($"Error when performing command: ");
                strErr.AppendLine($" {e.Message}");
                strErr.AppendLine($"  {e.StackTrace}");
                Console.WriteLine(strErr);
            }
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}