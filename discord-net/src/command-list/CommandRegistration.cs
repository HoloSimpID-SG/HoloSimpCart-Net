using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Discord;
using Discord.WebSocket;
using MMOR.NET.RichString;

namespace HoloSimpID;

public static class CommandRegisration
{
  public static async Task Start(SocketGuild guild)
  {
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
      if (CommandConsts.Responses.ContainsKey(command.Name))
      {
        continue;
      }
      isValid = false;
      Console.WriteLine($"Command: {command.Name}, does not have a response.");
    }
    foreach (
        KeyValuePair<string, Func<SocketSlashCommand, Task>> respond in CommandConsts.Responses
        )
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

    //-+-+-+-+-+-+-+-+-+
    // Upsert Commands
    //-+-+-+-+-+-+-+-+-+
    #region Upset Commands
    using var db = new AppDbContext();
    IReadOnlyCollection<SocketApplicationCommand> current_version =
      await guild.GetApplicationCommandsAsync();

    // Track which commands remains
    Dictionary<string, bool> registered_commands = db.CommandVCS.ToDictionary(x => x.command_name, _ => false);

    foreach (SlashCommandBuilder command in CommandConsts.commands)
    {
      try
      {
        var command_vcs = new CommandVCS(command);

        // Check if something of same name exists
        var db_ver = await db.CommandVCS.SingleOrDefaultAsync(x => x.command_name == command.Name);
        if (db_ver != null)
        {
          Console.WriteLine($"{command.Name} already exists in the db, comparing hashes");
          if (db_ver.version_hash != command_vcs.version_hash)
          {
            Console.WriteLine();
            Console.WriteLine($"{command.Name} is out of date. Reregistering."
                .SetColor(new(255, 128, 80)).Format(RichStringFormatter.kTerminal));
            Console.WriteLine();

            db_ver.version_hash = command_vcs.version_hash;
            db_ver.last_update = DateTime.UtcNow;
            await db.SaveChangesAsync();

            var old_ver = current_version.FirstOrDefault(old => old.Name == command.Name);
            await old_ver.DeleteAsync();
            await guild.CreateApplicationCommandAsync(command.Build());
            Console.WriteLine($"Finished Registering {command.Name}");
          }
          else
          {
            Console.WriteLine();
            Console.WriteLine($"{command.Name} is up to date, nothing to do."
                .SetColor(new(0, 128, 255)).Format(RichStringFormatter.kTerminal));
            Console.WriteLine();
          }
          // Mark Command
          registered_commands[command.Name] = true;
        }
        else
        {
          Console.WriteLine($"New command, reregistering {command.Name}.");
          await guild.CreateApplicationCommandAsync(command.Build());

          // Update Database
          await db.CommandVCS.AddAsync(command_vcs);
          await db.SaveChangesAsync();
          Console.WriteLine();
          Console.WriteLine($"Finished Registering {command.Name}"
              .SetColor(new(0, 255, 128)).Format(RichStringFormatter.kTerminal));
          Console.WriteLine();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error when upserting command {command.Name}");
        Console.WriteLine(ex.ToStringDemystified());
      }
    }

    // Clean Old Commands
    foreach ((string command_name, bool registered) in registered_commands) {
      if (registered) continue;

      // Delete from guild
      var command_to_delete = current_version.FirstOrDefault(cmd => cmd.Name == command_name);
      await command_to_delete.DeleteAsync();

      // Delete from DB
      db.CommandVCS.Where(x => x.command_name == command_name).ExecuteDeleteAsync();

      Console.WriteLine();
      Console.WriteLine($"Deleteing unbound command: {command_name}"
          .SetColor(new(255, 128, 255)).Format(RichStringFormatter.kTerminal));
      Console.WriteLine();
    }
    #endregion
  }
}
