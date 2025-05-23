using Azure;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace HoloSimpID
{
    public class CartBot
    {
        private static string DiscordToken => TokenHolder.DiscordToken;
        private static ulong GuildId => TokenHolder.GuildId;
        private static string SqlConnection => TokenHolder.SqlConnection;

        //-+-+-+-+-+-+-+-+
        // Discord Component
        //-+-+-+-+-+-+-+-+
        public static DiscordSocketClient client { get; private set; }
        public static CommandService commands { get; private set; }
        public static SqlConnection sqlConnection { get; private set; }

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
            await Task.Delay(-1);
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
            var guild = client.GetGuild(GuildId);

            //-+-+-+-+-+-+-+-+
            // Commands Validation
            // ..so that you buffons don't forget :ayamewheeze:
            //-+-+-+-+-+-+-+-+
            bool isValid = true;
            Console.WriteLine();
            Console.WriteLine($"Running Command Validation.");
            foreach (var command in CommandConsts.commands)
            {
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
                if (CommandConsts.commands.Any(x => x.Name == respond.Key))
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
            //await client.Rest.DeleteAllGlobalCommandsAsync();
            //await guild.DeleteApplicationCommandsAsync();

            //-+-+-+-+-+-+-+-+
            foreach (var command in CommandConsts.commands)
            {
                try
                {
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
        }

        private static async Task SlashCommandHandler(SocketSlashCommand command)
        {
            foreach (var respond in CommandConsts.responses)
            {
                if (command.Data.Name == respond.Key)
                    try
                    {
                        await Task.Run(() => respond.Value(command));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: ");
                        Console.WriteLine($" {e.Message}");
                        Console.WriteLine($"  {e.StackTrace}");
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