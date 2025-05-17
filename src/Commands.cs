using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoloSimpID
{
    /// <summary>
    /// <br/> - <strong>COMMANDS WA KOKO DESUWA</strong>
    /// <br/> - Even Gio's Biology Brain should be able to understand this.
    /// </summary>
    public static class CommandConsts
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

        public static readonly Dictionary<string, Action<SocketSlashCommand>> responses = new()
        {
            // Add the command name like this, followed by comma:
            { "create-cart",
                // Write this exact line: (Though honestly the "command" part can be anything, but let's not fry our brains here)
                command => {
                    // Everything inside will be what the command does
                    List<object> parameters = command.Data.Options.Select(x => x.Value).ToList();

                    string userName = command.User.Username;
                    string cartName = parameters[0] as string;

                    Simp owner = Simp.GetSimp(userName);
                    owner ??= new Simp(userName);

                    Cart cart = new(cartName, owner);
                    
                    // The Bot will reply with whatever you write here
                    command.RespondAsync($"Created Cart:\n{cart.getDetails()}");
                }
            },
            { "get-cart-by-name",
                command => {
                    List<object> parameters = command.Data.Options.Select(x => x.Value).ToList();

                    string userName = command.User.Username;
                    string cartName = parameters[0] as string;

                    Cart cart = Cart.GetCart(cartName);
                    if (cart == null)
                    {
                        command.RespondAsync($"No Cart with name: {cartName} was found.");
                        return;
                    }

                    command.RespondAsync(cart.getDetails());
                }
            },
            { "get-cart-by-id",
                command => {
                    List<object> parameters = command.Data.Options.Select(x => x.Value).ToList();

                    string userName = command.User.Username;
                    uint cartId = Convert.ToUInt32(parameters[0]);

                    Cart cart = Cart.GetCart(cartId);
                    if (cart == null)
                    {
                        command.RespondAsync($"No Cart with id: {cartId} was found.");
                        return;
                    }

                    command.RespondAsync(cart.getDetails());
                }
            },
            { "add-item-by-id",
                command => {
                    List<object> parameters = command.Data.Options.Select(x => x.Value).ToList();

                    string userName = command.User.Username;
                    uint cartId = Convert.ToUInt32(parameters[0]);
                    string itemName = parameters[1] as string;
                    string itemLink = parameters[2] as string;
                    uint quantity = Convert.ToUInt32(parameters[3] ?? 1);

                    Cart cart = Cart.GetCart(cartId);
                    if (cart == null)
                    {
                        command.RespondAsync($"No Cart with id: {cartId} was found.");
                        return;
                    }
                    Simp simp = Simp.GetSimp(userName);
                    simp ??= new Simp(userName);

                    if(!cart.addItem(simp, new(itemName, itemLink), quantity))
                    {
                        command.RespondAsync($"{cart} is already closed.");
                        return;
                    }

                    command.RespondAsync($"{userName} added {itemName} to {cart}");
                }
            },
            { "add-item-by-name",
                command => {
                    List<object> parameters = command.Data.Options.Select(x => x.Value).ToList();

                    string userName = command.User.Username;
                    string cartName = parameters[0] as string;
                    string itemName = parameters[1] as string;
                    string itemLink = parameters[2] as string;
                    uint quantity = Convert.ToUInt32(parameters[3] ?? 1);

                    Cart cart = Cart.GetCart(cartName);
                    if (cart == null)
                    {
                        command.RespondAsync($"No Cart with name: {cartName} was found.");
                        return;
                    }

                    Simp simp = Simp.GetSimp(userName);
                    simp ??= new Simp(userName);

                    if(!cart.addItem(simp, new(itemName, itemLink), quantity))
                    {
                        command.RespondAsync($"{cart} is already closed.");
                        return;
                    }

                    command.RespondAsync($"{userName} added {itemName} to {cart}");
                }
            },
            { "close-cart-by-name",
                command => {
                    List<object> parameters = command.Data.Options.Select(x => x.Value).ToList();

                    string userName = command.User.Username;
                    string cartName = parameters[0] as string;

                    Cart cart = Cart.GetCart(cartName);

                    if (cart == null)
                    {
                        command.RespondAsync($"No Cart with name: {cartName} was found.");
                        return;
                    }

                    cart.closeCart();

                    command.RespondAsync($"{userName} has closed: \n {cart}");
                }
            },
            { "close-cart-by-id",
                command => {
                    List<object> parameters = command.Data.Options.Select(x => x.Value).ToList();

                    string userName = command.User.Username;
                    uint cartId = Convert.ToUInt32(parameters[0]);

                    Cart cart = Cart.GetCart(cartId);
                    if (cart == null)
                    {
                        command.RespondAsync($"No Cart with id: {cartId} was found.");
                        return;
                    }

                    cart.closeCart();

                    command.RespondAsync($"{userName} has closed: \n {cart}");
                }
            },
        };
    }
}
