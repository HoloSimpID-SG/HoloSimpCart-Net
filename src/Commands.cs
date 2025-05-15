using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoloSimpID
{
    public static class CommandConsts
    {
        public static readonly SlashCommandBuilder[] commands =
        {
            new SlashCommandBuilder()
                .WithName("create-cart")
                .WithDescription("Create a new cart")
                .AddOption("cart-name", ApplicationCommandOptionType.String, "The name of the cart", isRequired: true),

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
            { "create-cart",
                command => {
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
