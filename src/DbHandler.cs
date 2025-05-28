using System;
using System.Text;
using Npgsql;

namespace HoloSimpID
{
    public static class DbHandler
    {
        private static NpgsqlDataSource dataSource;

        public static async Task InitializeDB()
        {
            string SqlConnection = Environment.GetEnvironmentVariable("SQL_CONNECTION");
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(SqlConnection);
            dataSourceBuilder.MapComposite<Item>("item_type");
            dataSource = dataSourceBuilder.Build();
        }

        public static async Task LoadDB()
        {
            Console.WriteLine("Loading Database");
            try
            {
                using (var sqlConnection = await dataSource.OpenConnectionAsync())
                {
                    Simp.DeserializeAll(sqlConnection);
                    Cart.DeserializeAll(sqlConnection);
                    Console.WriteLine("Database Loaded Succesfully, HUMU");
                    return;
                }
            }
            catch (PostgresException ex) when (ex.SqlState == "3D000")
            {
                Console.WriteLine("Database does not exist or is not ready, retrying in 3 seconds...");
                await Task.Delay(3000);
            }
            catch (Exception ex)
            {
                StringBuilder strErr = new();
                strErr.AppendLine($"Uncategorized Error when Loading Database:");
                strErr.AppendLine($"{ex.Message}");
                strErr.AppendLine($" {ex.StackTrace}");
                Console.WriteLine(strErr);
            }
        }

        public static async Task SaveAllDB()
        {
            Console.WriteLine("Saving Every Single Database, Before Marine steals it");
            List<NpgsqlCommand> sqlCommands = new();
            sqlCommands.AddRange(Simp.SerializeAll());
            sqlCommands.AddRange(Cart.SerializeAll());

            using (var sqlConnection = await dataSource.OpenConnectionAsync())
            {
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
                        await transaction.CommitAsync();
                        Console.WriteLine("Database Saved Successfully, HUMU");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        
                        StringBuilder strErr = new();
                        strErr.AppendLine($"Error saving Database: ");
                        strErr.AppendLine($"{ex.Message}");
                        strErr.AppendLine($" {ex.StackTrace}");
                        Console.WriteLine(strErr);
                    }

                }
            }

            Console.WriteLine("Database Saved");
        }

        public static async Task RunSqlCommand(NpgsqlCommand sqlCommand)
        {
            using (var sqlConnection = await dataSource.OpenConnectionAsync())
            {
                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        sqlCommand.Connection = sqlConnection;
                        sqlCommand.Transaction = transaction;
                        await sqlCommand.ExecuteNonQueryAsync();
                        await transaction.CommitAsync();
                        Console.WriteLine("SQL Command Executed Successfully, HUMU");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        StringBuilder strErr = new();
                        strErr.AppendLine($"Error running SqlCommand:");
                        strErr.AppendLine($"SQL Command: {sqlCommand.CommandText}");
                        strErr.AppendLine($"Error Details:");
                        strErr.AppendLine($"{ex.Message}");
                        strErr.AppendLine($" {ex.StackTrace}");
                        Console.WriteLine(strErr);
                    }
                }
            }
        }

        public static async Task RunSqlCommand(IEnumerable<NpgsqlCommand> sqlCommands)
        {
            using (var sqlConnection = await dataSource.OpenConnectionAsync())
            {
                using (var transaction = sqlConnection.BeginTransaction())
                {
                    string lastSqlCommand = string.Empty;
                    try
                    {
                        foreach (var cmd in sqlCommands)
                        {
                            lastSqlCommand = cmd.CommandText;
                            cmd.Connection = sqlConnection;
                            cmd.Transaction = transaction;
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await transaction.CommitAsync();
                        Console.WriteLine("All SQL Commands Executed Successfully, HUMU");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        StringBuilder strErr = new();
                        strErr.AppendLine($"Error running SqlCommand:");
                        strErr.AppendLine($"Last SQL Command: {lastSqlCommand}");
                        strErr.AppendLine($"Error Details:");
                        strErr.AppendLine($"{ex.Message}");
                        strErr.AppendLine($" {ex.StackTrace}");
                        Console.WriteLine(strErr);
                    }
                }
            }
        }

    }
}
