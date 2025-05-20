using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace HoloSimpID
{
    /// <summary>
    /// <br/> - <strong>COMMANDS WA KOKO DESUWA</strong>
    /// <br/> - Even Gio's Biology Brain should be able to understand this.
    /// </summary>
    public static partial class CommandConsts
    {
        public static readonly ImmutableDictionary<string, Action<SocketSlashCommand>> responses = new Dictionary<string, Action<SocketSlashCommand>>()
        {
            // Add the command name like this, followed by comma:
            { "create-cart",
                // Write this exact line: (Though honestly the "command" part can be anything, but let's not fry our brains here)
                command => {
                    // This is a reader I made so that is simple
                    // ..This is a Dictionary<string, object>,
                    // ..use the name you put back during .AddOption() to as the key.
                    var parameters = MoLibrary.ReadCommandParameter(command);

                    // This line gets the discord username of the person who called the command
                    string userName = command.User.Username;
                    // Make sure to perform Conversion as this params values are object.
                    // ..for numerics, call a "Convert.ToType(parameters[key])"
                    // ..for string a simple "as string" is enough
                    string cartName = parameters.GetCastedValueOrDefault("cart-name", string.Empty);

                    Simp owner = Simp.GetSimp(userName);
                    owner ??= new Simp(userName);

                    Cart cart = new(cartName, owner);

                    // The Bot will reply with whatever you write here
                    command.RespondAsync($"Created Cart:\n{cart.getDetails()}");
                }
            },

            //-+-+-+-+-+-+-+-+
            // Get Cart Commands
            //-+-+-+-+-+-+-+-+
            { "get-cart-by-name",
                command => {
                    var parameters = MoLibrary.ReadCommandParameter(command);

                    string userName = command.User.Username;
                    string cartName = parameters.GetCastedValueOrDefault("cart-name", string.Empty);

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
                    var parameters = MoLibrary.ReadCommandParameter(command);

                    string userName = command.User.Username;
                    int cartId = parameters.GetCastedValueOrDefault("cart-id", -1);

                    Cart cart = Cart.GetCart(cartId);
                    if (cart == null)
                    {
                        command.RespondAsync($"No Cart with id: {cartId} was found.");
                        return;
                    }

                    command.RespondAsync(cart.getDetails());
                }
            },

            //-+-+-+-+-+-+-+-+
            // Close Cart Commands
            //-+-+-+-+-+-+-+-+
            { "close-cart-by-name",
                command => {
                    var parameters = MoLibrary.ReadCommandParameter(command);

                    string userName = command.User.Username;
                    string cartName = parameters.GetCastedValueOrDefault("cart-name", string.Empty);

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
                    var parameters = MoLibrary.ReadCommandParameter(command);

                    string userName = command.User.Username;
                    int cartId = parameters.GetCastedValueOrDefault("cart-id", -1);

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

            //-+-+-+-+-+-+-+-+
            // Add Item to Cart Commands
            //-+-+-+-+-+-+-+-+
            { "add-item-by-id",
                command => {
                    var parameters = MoLibrary.ReadCommandParameter(command);

                    string userName = command.User.Username;
                    int cartId = parameters.GetCastedValueOrDefault("cart-id", -1);
                    string itemName = parameters.GetCastedValueOrDefault("item-name", string.Empty);
                    string itemLink = parameters.GetCastedValueOrDefault("item-link", string.Empty);
                    double itemPrice = parameters.GetCastedValueOrDefault("item-price", 0.0);
                    uint quantity = parameters.GetCastedValueOrDefault("quantity", 1u);

                    Cart cart = Cart.GetCart(cartId);
                    if (cart == null)
                    {
                        command.RespondAsync($"No Cart with id: {cartId} was found.");
                        return;
                    }
                    Simp simp = Simp.GetSimp(userName);
                    simp ??= new Simp(userName);

                    if(!cart.addItem(simp, new(itemName, itemLink, itemPrice), quantity))
                    {
                        command.RespondAsync($"{cart} is already closed.");
                        return;
                    }

                    command.RespondAsync($"{userName} added {itemName} to {cart}");
                }
            },
            { "add-item-by-name",
                command => {
                    var parameters = MoLibrary.ReadCommandParameter(command);

                    string userName = command.User.Username;
                    string cartName = parameters.GetCastedValueOrDefault("cart-name", string.Empty);
                    string itemName = parameters.GetCastedValueOrDefault("item-name", string.Empty);
                    string itemLink = parameters.GetCastedValueOrDefault("item-link", string.Empty);
                    double itemPrice = parameters.GetCastedValueOrDefault("item-price", 0.0);
                    uint quantity = parameters.GetCastedValueOrDefault("quantity", 1u);

                    Cart cart = Cart.GetCart(cartName);
                    if (cart == null)
                    {
                        command.RespondAsync($"No Cart with name: {cartName} was found.");
                        return;
                    }

                    Simp simp = Simp.GetSimp(userName);
                    simp ??= new Simp(userName);

                    if(!cart.addItem(simp, new(itemName, itemLink, itemPrice), quantity))
                    {
                        command.RespondAsync($"{cart} is already closed.");
                        return;
                    }

                    command.RespondAsync($"{userName} added {itemName} to {cart}");
                }
            },

            //-+-+-+-+-+-+-+-+
            // List All Carts
            //-+-+-+-+-+-+-+-+
            { "list-all-carts",
                command => {
                    var parameters = MoLibrary.ReadCommandParameter(command);

                    string userName = command.User.Username;
                    bool onlyOpen = parameters.GetCastedValueOrDefault("only-open-carts", x => Convert.ToBoolean(x), false);


                    StringBuilder strResult = new();
                    List<Cart> cartList = new();
                    if (onlyOpen)
                        Cart.GetAllCarts(cartList, (i, list) => list[i].stillOpen);
                    else
                        Cart.GetAllCarts(cartList);
                    foreach(Cart cart in cartList)
                        strResult.AppendLine(cart.getDetails());

                    command.RespondAsync($"{strResult}");
                }
            },
        }.ToImmutableDictionary();
}
}