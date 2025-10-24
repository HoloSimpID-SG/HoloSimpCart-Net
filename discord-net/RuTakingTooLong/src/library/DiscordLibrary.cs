using Discord.WebSocket;

namespace HoloSimpID {
  public static partial class MoLibrary {
    /// <summary>
    ///     <br /> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///     <br /> - Converts Discord.NET's Command parameters to a more easily accessed
    ///     <see cref="Dictionary{TKey, TValue}" />.
    ///     <br /> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// </summary>
    public static void ReadCommandParameter(
        Dictionary<string, object> parameters, SocketSlashCommand command) {
      IEnumerable<SocketSlashCommandDataOption> dataOptions = command.Data.Options;
      foreach (SocketSlashCommandDataOption dataOption in dataOptions) {
        parameters.Add(dataOption.Name, dataOption.Value);
      }
    }

    /// <summary>
    ///     <inheritdoc cref="ReadCommandParameter(Dictionary{string, object}, SocketSlashCommand)"
    ///     />
    public static Dictionary<string, object> ReadCommandParameter(SocketSlashCommand command) {
      Dictionary<string, object> parameters = new();
      ReadCommandParameter(parameters, command);
      return parameters;
    }
  }
}
