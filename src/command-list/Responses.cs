using Discord;
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
                    // ..if you're a dummy, Dictionary takes "string" as key returns "object" as value.
                    // ..use the name you put back during .AddOption() to as the key.
                    var parameters = MoLibrary.ReadCommandParameter(command);

                    // This line gets the discord username of the person who called the command
                    string userName = command.User.Username;
                    // .GetCastedValueOrDefault, takes the name you put back during .AddOption(),
                    // ..if the parameter is not filled during commandd,
                    // ./.it will return the value to the right of the comma.
                    string cartName = parameters.GetCastedValueOrDefault("cart-name", string.Empty);

                    // Then, you write the logic here
                    Simp owner = Simp.GetSimp(userName);
                    owner ??= new Simp(userName);

                    Cart cart = new(cartName, owner);

                    // This last line determine what
                    // ..the bot responds with.
                    command.RespondAsync($"Created Cart:\n{cart.getDetails()}");

                    // Now you're a genius!
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

                    if (cartList.IsNullOrEmpty())
                    {
                        command.RespondAsync($"No Cart detected, what a cheapskate group");
                        return;
                    }

                    foreach(Cart cart in cartList)
                        strResult.AppendLine(cart.getDetails());

                    command.RespondAsync($"{strResult}");
                }
            },
            { "get-cart-stats-by-id",
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

                    Dictionary<double, uint> itemFreqMap = new();
                    Dictionary<double, uint> simpFreqMap = new();
                    foreach (var simp in cart.cartItems)
                    {
                        double totalSimp = 0;
                        foreach (var item in simp.Value)
                        {
                            totalSimp += item.Key.priceSGD * item.Value;
                            itemFreqMap.AddFrequency(item.Key.priceSGD, item.Value);
                        }
                        simpFreqMap.AddFrequency(totalSimp);
                    }
                    if (itemFreqMap.IsNullOrEmpty() || simpFreqMap.IsNullOrEmpty())
                    {
                        command.RespondAsync($"Cart was empty, pitiful weakling.");
                        return;
                    }

                    TotalStatistics statsByItem = TotalStatistics.Calculate(itemFreqMap);
                    TotalStatistics statsBySimp = TotalStatistics.Calculate(simpFreqMap);

                    StringBuilder strResult = new();
                    strResult.AppendLine($"# Statistics for Cart: {cart}");
                    strResult.AppendLine($"## By Items:");
                    strResult.AppendLine($"Total Items: {statsByItem.Count:N0}");
                    strResult.AppendLine($"Average Cost: {statsByItem.Mean:C2}");
                    strResult.AppendLine($"Standard Deviation: {statsByItem.StandardDeviation:C2}");
                    strResult.AppendLine($"Gini Coefficient: {statsByItem.GiniCoefficient:P2}");
                    strResult.AppendLine($"Most Expensive: {statsByItem.Maximum:P2}");
                    strResult.AppendLine($"Box Plot: {statsByItem.LowerFence:C2} <- [ {statsByItem.Q1:C2} | {statsByItem.Median:C2} | {statsByItem.Q3:C2} ] -> {statsByItem.UpperFence:C2}");

                    strResult.AppendLine($"## By Simps:");
                    strResult.AppendLine($"Total Perticipants: {statsBySimp.Count:N0}");
                    strResult.AppendLine($"Average Spending: {statsBySimp.Mean:C2}");
                    strResult.AppendLine($"Standard Deviation: {statsBySimp.StandardDeviation:C2}");
                    strResult.AppendLine($"Gini Coefficient: {statsBySimp.GiniCoefficient:P2}");
                    strResult.AppendLine($"Biggest Spender: {statsBySimp.Maximum:P2}");
                    strResult.AppendLine($"Box Plot: {statsBySimp.LowerFence:C2} <- [ {statsBySimp.Q1:C2} | {statsBySimp.Median:C2} | {statsBySimp.Q3:C2} ] -> {statsBySimp.UpperFence:C2}");

                    command.RespondAsync($"{strResult}");
                }
            },
        }.ToImmutableDictionary();
}
}