using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;

namespace HoloSimpID
{
    public static partial class MoLibrary
    {
        public static void ReadCommandParameter(Dictionary<string, object> parameters, SocketSlashCommand command)
        {
            IEnumerable<SocketSlashCommandDataOption> dataOptions = command.Data.Options;
            foreach (var dataOption in dataOptions)
                parameters.Add(dataOption.Name, dataOption.Value);
        }
        public static Dictionary<string, object> ReadCommandParameter(SocketSlashCommand command)
        {
            Dictionary<string, object> parameters = new();
            ReadCommandParameter(parameters, command);
            return parameters;
        }
    }
}