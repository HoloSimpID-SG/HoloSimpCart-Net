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
                .AddOption("cart-name", ApplicationCommandOptionType.String, "The name of the cart", isRequired: true),

            /// Once you are done adding it here, go to <see cref="responses"/>

            new SlashCommandBuilder()
                .WithName("get-cart")
                .WithDescription("Create a new cart")
                .AddOption("cart-name", ApplicationCommandOptionType.String, "The name of the cart", isRequired: true),

            new SlashCommandBuilder()
                .WithName("close-cart")
                .WithDescription("Closes a Cart")
                .AddOption("cart-name", ApplicationCommandOptionType.String, "The name of the cart", isRequired: true),

            new SlashCommandBuilder()
                .WithName("cart-add-item")
                .WithDescription("Closes a Cart")
                .AddOption("item-name", ApplicationCommandOptionType.String, "The name of the cart", isRequired: true)
                .AddOption("cart-name", ApplicationCommandOptionType.String, "The name of the cart", isRequired: true),
        };

        public static readonly Dictionary<string, Action<SocketSlashCommand>> responses = new()
        {
            // Add the command name like this, followed by comma:
            { "create-cart",
                // Write this exact line: (Though honestly the "command" part can be anything, but let's not fry our brains here)
                command => {
                    // Everything inside will be what the command does
                    List<object> parameters = command.Data.Options.Select(x => x.Value).ToList();

                    Simp owner = null;
                    foreach(Simp simp in PsuedoDB.Simps)
                    {
                        if (simp.name == command.User.Username)
                        {
                            owner = simp;
                            break;
                        }
                    }
                    owner ??= new(command.User.Username);

                    Cart cart = new(parameters[0].ToString(), owner);
                    PsuedoDB.carts.AddAutoKey(cart);
                    
                    // The Bot will reply with whatever you write here
                    command.RespondAsync($"Created Cart:\n {cart}");
                }
            },
            { "get-cart",
                command => {
                    List<object> parameters = command.Data.Options.Select(x => x.Value).ToList();

                    string userName = command.User.Username;
                    string cartName = parameters[0] as string;

                    Cart cart = null;
                    foreach(Cart cartLookup in PsuedoDB.carts.Values)
                        if(cartLookup.cartName == cartName) cart = cartLookup;

                    if (cart == null)
                    {
                        command.RespondAsync($"No Cart with Name or Id was found.");
                        return;
                    }

                    command.RespondAsync($"{cart}");
                }
            },
            { "cart-add-item",
                command => {
                    List<object> parameters = command.Data.Options.Select(x => x.Value).ToList();

                    string userName = command.User.Username;
                    string itemName = parameters[0] as string;
                    string cartName = parameters[1] as string;

                    Cart cart = null;
                    foreach(Cart cartLookup in PsuedoDB.carts.Values)
                        if(cartLookup.cartName == cartName) cart = cartLookup;

                    if (cart == null)
                    {
                        command.RespondAsync($"No Cart with Name or Id was found.");
                        return;
                    }

                    if(!cart.stillOpen)
                    {
                        command.RespondAsync($"Cart is already closed.");
                        return;
                    }

                    Simp simp = null;
                    foreach(Simp simpLookup in PsuedoDB.Simps)
                    {
                        if (simpLookup.name == userName)
                        {
                            simp = simpLookup;
                            break;
                        }
                    }
                    simp ??= new(command.User.Username);

                    cart.addItem(new(itemName), simp);

                    command.RespondAsync($"{userName} added {itemName} to Cart:\n {cart}");
                }
            },
            { "close-cart",
                command => {
                    List<object> parameters = command.Data.Options.Select(x => x.Value).ToList();

                    string userName = command.User.Username;
                    string cartName = parameters[0] as string;

                    Cart cart = null;
                    foreach(Cart cartLookup in PsuedoDB.carts.Values)
                        if(cartLookup.cartName == cartName) cart = cartLookup;

                    if (cart == null)
                    {
                        command.RespondAsync($"No Cart with Name or Id was found.");
                        return;
                    }

                    cart.closeCart();

                    command.RespondAsync($"{userName} has closed: \n {cart}");
                }
            },
        };
    }
}
