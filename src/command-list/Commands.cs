using Discord;

namespace HoloSimpID
{
    /// <summary>
    /// <br/> - <strong>COMMANDS WA KOKO DESUWA</strong>
    /// <br/> - Even Gio's Biology Brain should be able to understand this.
    /// </summary>
    public static partial class CommandConsts
    {
        public static readonly SlashCommandBuilder[] commands =
        {
            // Each new command, write this line:
            new SlashCommandBuilder()
                // This is the name, which will be used to call the command
                .WithName("create-cart")
                // This is the tooltip for hints, can leave blank with "", or leave a Dad Joke Ina would be proud of
                .WithDescription("Create a new cart")
                // This becomes the parameters, staring with name, type and then hint
                // ..can add multiple parameters by repeating the line below
                .AddOption("cart-name", ApplicationCommandOptionType.String, "Where's my Hina Gio", isRequired: true)
                , // Move the comma here to make it easier to add new Parameters
            /// Once you are done adding it here, go to <see cref="responses"/>

            new SlashCommandBuilder()
                .WithName("get-cart-by-name")
                .WithDescription("Create a new cart")
                .AddOption("cart-name", ApplicationCommandOptionType.String, "Where's my Hina Gio", isRequired: true)
                ,

            new SlashCommandBuilder()
                .WithName("get-cart-by-id")
                .WithDescription("Create a new cart")
                .AddOption("cart-id", ApplicationCommandOptionType.Integer, "Where's my Hina Gio", isRequired: true)
                ,

            new SlashCommandBuilder()
                .WithName("close-cart-by-name")
                .WithDescription("Closes a Cart")
                .AddOption("cart-name", ApplicationCommandOptionType.String, "Where's my Hina Gio", isRequired: true)
                ,
            new SlashCommandBuilder()
                .WithName("close-cart-by-id")
                .WithDescription("Closes a Cart")
                .AddOption("cart-id", ApplicationCommandOptionType.Integer, "Where's my Hina Gio", isRequired: true)
                ,

            new SlashCommandBuilder()
                .WithName("add-item-by-id")
                .WithDescription("Adds Item")
                .AddOption("cart-id", ApplicationCommandOptionType.Integer, "Where's my Hina Gio", isRequired: true)
                .AddOption("item-name", ApplicationCommandOptionType.String, "Where's my Hina Gio", isRequired: true)
                .AddOption("item-link", ApplicationCommandOptionType.String, "Where's my Hina Gio", isRequired: true)
                .AddOption("quantity", ApplicationCommandOptionType.Integer, "Where's my Hina Gio", isRequired: true)
                ,
            new SlashCommandBuilder()
                .WithName("add-item-by-name")
                .WithDescription("Adds Item")
                .AddOption("cart-name", ApplicationCommandOptionType.String, "Where's my Hina Gio", isRequired: true)
                .AddOption("item-name", ApplicationCommandOptionType.String, "Where's my Hina Gio", isRequired: true)
                .AddOption("item-link", ApplicationCommandOptionType.String, "Where's my Hina Gio", isRequired: true)
                .AddOption("quantity", ApplicationCommandOptionType.Integer, "Where's my Hina Gio", isRequired: true)
                ,
        };
    }
}