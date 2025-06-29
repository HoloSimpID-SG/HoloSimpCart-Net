using Discord;

namespace HoloSimpID
{
    /// <summary>
    ///     <br /> - <strong>COMMANDS WA KOKO DESUWA</strong>
    ///     <br /> - Even Gio's Biology Brain should be able to understand this.
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
                .WithDescription("Where's my Hina, Gio")
                // This becomes the parameters, staring with name, type and then hint
                // ..can add multiple parameters by repeating the line below
                .AddOption("cart-name", ApplicationCommandOptionType.String, "Where's my Hina, Gio",
                    true), // Move the comma here to make it easier to add new Parameters
            /// Once you are done adding it here, go to <see cref="responses"/>

            //-+-+-+-+-+-+-+-+
            // Get Cart Commands
            //-+-+-+-+-+-+-+-+
            new SlashCommandBuilder()
                .WithName("get-cart-by-name")
                .WithDescription("Where's my Hina, Gio")
                .AddOption("cart-name", ApplicationCommandOptionType.String, "Where's my Hina, Gio", true),
            new SlashCommandBuilder()
                .WithName("get-cart-by-id")
                .WithDescription("Where's my Hina, Gio")
                .AddOption("cart-id", ApplicationCommandOptionType.Integer, "Where's my Hina, Gio", true),

            //-+-+-+-+-+-+-+-+
            // Close Cart Commands
            //-+-+-+-+-+-+-+-+
            new SlashCommandBuilder()
                .WithName("close-cart-by-name")
                .WithDescription("Where's my Hina, Gio")
                .AddOption("cart-name", ApplicationCommandOptionType.String, "Where's my Hina, Gio", true),
            new SlashCommandBuilder()
                .WithName("close-cart-by-id")
                .WithDescription("Where's my Hina, Gio")
                .AddOption("cart-id", ApplicationCommandOptionType.Integer, "Where's my Hina, Gio", true),

            //-+-+-+-+-+-+-+-+
            // List All Carts
            //-+-+-+-+-+-+-+-+
            new SlashCommandBuilder()
                .WithName("list-all-carts")
                .WithDescription("Where's my Hina, Gio")
                .AddOption("only-open-carts", ApplicationCommandOptionType.Boolean, "Where's my Hina, Gio", true),

            new SlashCommandBuilder()
                .WithName("get-cart-stats-by-id")
                .WithDescription("Where's my Hina, Gio")
                .AddOption("cart-id", ApplicationCommandOptionType.Integer, "Where's my Hina, Gio", true),

            new SlashCommandBuilder()
                .WithName("register-me")
                .WithDescription("Where's my Hina, Gio")
                .AddOption("nickname", ApplicationCommandOptionType.String, "Where's my Hina, Gio", true),

            new SlashCommandBuilder()
                .WithName("bau-bau")
                .WithDescription("Where's my Hina, Gio")
                .AddOption("how-many", ApplicationCommandOptionType.Integer, "Where's my Hina, Gio", true),

            //-+-+-+-+-+-+-+-+
            // Add Item to Cart Commands
            //-+-+-+-+-+-+-+-+
            new SlashCommandBuilder()
                .WithName("add-item-by-id")
                .WithDescription("Where's my Hina, Gio")
                .AddOption("cart-id", ApplicationCommandOptionType.Integer, "Where's my Hina, Gio", true)
                .AddOption("item-name", ApplicationCommandOptionType.String, "Where's my Hina, Gio", true)
                .AddOption("item-link", ApplicationCommandOptionType.String, "Where's my Hina, Gio", false)
                .AddOption("item-price", ApplicationCommandOptionType.Number, "Where's my Hina, Gio", false)
                .AddOption("quantity", ApplicationCommandOptionType.Integer, "Where's my Hina, Gio", false),
            new SlashCommandBuilder()
                .WithName("add-item-by-name")
                .WithDescription("Where's my Hina, Gio")
                .AddOption("cart-name", ApplicationCommandOptionType.String, "Where's my Hina, Gio", true)
                .AddOption("item-name", ApplicationCommandOptionType.String, "Where's my Hina, Gio", true)
                .AddOption("item-link", ApplicationCommandOptionType.String, "Where's my Hina, Gio", false)
                .AddOption("item-price", ApplicationCommandOptionType.Number, "Where's my Hina, Gio", false)
                .AddOption("quantity", ApplicationCommandOptionType.Integer, "Where's my Hina, Gio", false)
        };
    }
}