using System.Text;
using System.Security.Cryptography;
using Discord;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MMOR.NET.Utilities;
namespace HoloSimpID;

public class CommandVCS
{
  [Key]
  public string command_name { get; set; }
  public string version_hash { get; set; }
  [Column(TypeName = "timestamptz")]
  public DateTime last_update { get; set; }

  public CommandVCS() { }

  public CommandVCS(SlashCommandBuilder command)
  {
    command_name = command.Name;
    version_hash = GetCommandHash(command);
    last_update = DateTime.UtcNow;
  }

  public static string GetCommandHash(SlashCommandBuilder command)
  {
    var command_data = new StringBuilder();
    command_data.Append(command.Name);
    command_data.Append(command.Description);

    if (!command.Options.IsNullOrEmpty())
    {
      foreach (SlashCommandOptionBuilder option in command.Options)
      {
        command_data.Append(option.Name);
        command_data.Append(option.Description);
        command_data.Append(option.Type);
        command_data.Append(option.IsRequired);
      }
    }

    using (var sha256 = SHA256.Create())
    {
      byte[] bytes = UTF8Encoding.UTF8.GetBytes(command_data.ToString());
      byte[] hash = sha256.ComputeHash(bytes);
      return Convert.ToBase64String(hash);
    }
  }
  public static string GetCommandHash(SlashCommandProperties command)
  {
    var command_data = new StringBuilder();
    command_data.Append(command.Name);
    command_data.Append(command.Description);

    if (command.Options.IsSpecified)
    {
      foreach (ApplicationCommandOptionProperties option in command.Options.Value)
      {
        command_data.Append(option.Name);
        command_data.Append(option.Description);
        command_data.Append(option.Type);
        command_data.Append(option.IsRequired);
      }
    }

    using (var sha256 = SHA256.Create())
    {
      byte[] bytes = UTF8Encoding.UTF8.GetBytes(command_data.ToString());
      byte[] hash = sha256.ComputeHash(bytes);
      return Convert.ToBase64String(hash);
    }

  }
}
