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
        private static DiscordSocketClient Client; public static DiscordSocketClient client => Client;
        private static CommandService Commands; public static CommandService commands => Commands;
        private static SqlConnection Connection; public static SqlConnection connection => Connection;

        public static async Task Main()
        {
            //Connection = new SqlConnection(SqlConnection);
            Client = new DiscordSocketClient();
            Commands = new CommandService();

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
            await Client.LoginAsync(TokenType.Bot, DiscordToken);
            await Client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
            //await SaveDB();
        }

        public static async Task LoadDB()
        {
            using (var connection = new SqlConnection(SqlConnection))
            {
                await connection.OpenAsync();

                try
                {
                    Simp.DeserializeAll(connection);
                    Cart.DeserializeAll(connection);
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

            using (var connection = new SqlConnection(SqlConnection))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var cmd in sqlCommands)
                        {
                            cmd.Connection = connection;
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
            // Clear Commands
            // .. if not, it will retain already deleted commands
            // .. or fail to update with new logic
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