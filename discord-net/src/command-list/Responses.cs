using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Discord.WebSocket;
using MMOR.NET.Random;
using MMOR.NET.Statistics;
using MMOR.NET.Utilities;

namespace HoloSimpID
{
  /// <summary>
  ///     <br /> - <strong>COMMANDS WA KOKO DESUWA</strong>
  ///     <br /> - Even Gio's Biology Brain should be able to understand this.
  /// </summary>
  public static partial class CommandConsts
  {
    public static readonly ImmutableDictionary<string, Func<SocketSlashCommand, Task>> responses =
      new Dictionary<string, Func<SocketSlashCommand, Task>>
      {
        // Add the command name like this, followed by comma:
        {
          "create-cart",
          // Write this exact line: (Though honestly the "command" part can be anything, but let's not fry our brains here)
          async command =>
          {
            // This line reads all the .AddOption() you added
            // ..as some sort of List.
            Dictionary<string, object> parameters = MoLibrary.ReadCommandParameter(command);

            // This line gets the discord username of the person who called the command
            string userName = command.User.Username;
            // we access parameters with .GetCastedValueOrDefault()
            // ..the first parameter is the name you used in .AddOption()
            // ..the second parameter is the default value if the user did not provide it
            string cartName = parameters.GetCastedValueOrDefault("cart-name", string.Empty);

            // Then, you write the logic here
            Simp owner = await Simp.TryGet(command.User) ?? await Simp.RegisterSimp(command.User);

            var cart = await Cart.OpenCart(owner, cartName);

            // This last line determines what
            // ..the bot responds with.
            await command.RespondAsync($"Created Cart:\n{await cart.GetDetails()}");

            // Now you're a genius!
          }
        },
        //-+-+-+-+-+-+-+-+
        // Get Cart Commands
        //-+-+-+-+-+-+-+-+
        {
          "get-cart-by-name",
          async command =>
          {
            Dictionary<string, object> parameters = MoLibrary.ReadCommandParameter(command);

            string userName = command.User.Username;
            string cartName = parameters.GetCastedValueOrDefault("cart-name", string.Empty);

            Cart? cart = await Cart.TryGet(cartName);
            if (cart == null)
            {
              await command.RespondAsync($"No Cart with name: {cartName} was found.");
              return;
            }

            await command.RespondAsync(await cart.GetDetails());
          }
        },
        {
          "get-cart-by-id",
          async command =>
          {
            Dictionary<string, object> parameters = MoLibrary.ReadCommandParameter(command);

            string userName = command.User.Username;
            int cartId = parameters.GetCastedValueOrDefault("cart-id", -1);

            Cart? cart = await Cart.TryGet(cartId);
            if (cart == null)
            {
              await command.RespondAsync($"No Cart with id: {cartId} was found.");
              return;
            }

            await command.RespondAsync(await cart.GetDetails());
          }
        },
        //-+-+-+-+-+-+-+-+
        // Close Cart Commands
        //-+-+-+-+-+-+-+-+
        {
          "close-cart-by-name",
          async command =>
          {
            Dictionary<string, object> parameters = MoLibrary.ReadCommandParameter(command);

            string userName = command.User.Username;
            string cartName = parameters.GetCastedValueOrDefault("cart-name", string.Empty);

            Cart? cart = await Cart.TryGet(cartName);
            Simp? simp = await Simp.TryGet(userName);

            if (cart == null)
            {
              await command.RespondAsync($"No Cart with name: {cartName} was found.");
              return;
            }

            if (simp == null || simp.uDex != cart.OwnerDex)
              await command.RespondAsync(
                "You are not allowed to perform this action as you are not the owner of this cart."
              );

            await cart.CloseCart();

            await command.RespondAsync($"{userName} has closed: \n {cart}");
          }
        },
        {
          "close-cart-by-id",
          async command =>
          {
            Dictionary<string, object> parameters = MoLibrary.ReadCommandParameter(command);

            string userName = command.User.Username;
            int cartId = parameters.GetCastedValueOrDefault("cart-id", -1);

            Cart? cart = await Cart.TryGet(cartId);
            Simp? simp = await Simp.TryGet(userName);

            if (cart == null)
            {
              await command.RespondAsync($"No Cart with id: {cartId} was found.");
              return;
            }

            if (simp == null || simp.uDex != cart.OwnerDex)
              await command.RespondAsync(
                "You are not allowed to perform this action as you are not the owner of this cart."
              );

            await cart.CloseCart();

            await command.RespondAsync($"{userName} has closed: \n {cart}");
          }
        },
        {
          "set-cart-delivered-by-name",
          async command =>
          {
            Dictionary<string, object> parameters = MoLibrary.ReadCommandParameter(command);

            string userName = command.User.Username;
            string cartName = parameters.GetCastedValueOrDefault("cart-name", string.Empty);

            Cart? cart = await Cart.TryGet(cartName);
            Simp? simp = await Simp.TryGet(userName);

            if (cart == null)
            {
              await command.RespondAsync($"No Cart with name: {cartName} was found.");
              return;
            }

            if (simp == null || simp.uDex != cart.OwnerDex)
              await command.RespondAsync(
                "You are not allowed to perform this action as you are not the owner of this cart."
              );

            await cart.SetCartDelivered();

            await command.RespondAsync($"{userName} has closed: \n {cart}");
          }
        },
        {
          "set-cart-delivered-by-id",
          async command =>
          {
            Dictionary<string, object> parameters = MoLibrary.ReadCommandParameter(command);

            string userName = command.User.Username;
            int cartId = parameters.GetCastedValueOrDefault("cart-id", -1);

            Cart? cart = await Cart.TryGet(cartId);
            Simp? simp = await Simp.TryGet(userName);

            if (cart == null)
            {
              await command.RespondAsync($"No Cart with id: {cartId} was found.");
              return;
            }

            if (simp == null || simp.uDex != cart.OwnerDex)
              await command.RespondAsync(
                "You are not allowed to perform this action as you are not the owner of this cart."
              );

            await cart.SetCartDelivered();

            await command.RespondAsync($"{userName} has closed: \n {cart}");
          }
        },
        //-+-+-+-+-+-+-+-+
        // Add Item to Cart Commands
        //-+-+-+-+-+-+-+-+
        {
          "upsert-item-by-id",
          async command =>
          {
            Dictionary<string, object> parameters = MoLibrary.ReadCommandParameter(command);

            string userName = command.User.Username;
            int cartId = parameters.GetCastedValueOrDefault("cart-id", -1);
            string itemName = parameters.GetCastedValueOrDefault("item-name", string.Empty);
            string itemLink = parameters.GetCastedValueOrDefault("item-link", string.Empty);
            double itemPrice = parameters.GetCastedValueOrDefault("item-price", 0.0);
            int quantity = parameters.GetCastedValueOrDefault("quantity", 1);

            Cart? cart = await Cart.TryGet(cartId);
            if (cart == null)
            {
              await command.RespondAsync($"No Cart with id: {cartId} was found.");
              return;
            }
            Simp simp = await Simp.TryGet(userName) ?? await Simp.RegisterSimp(userName);

            var item = new Item(itemName, itemLink, (decimal)itemPrice);
            if (!await cart.UpsertItem(simp, item, quantity))
            {
              await command.RespondAsync($"{cart} is already closed.");
              return;
            }

            await command.RespondAsync($"{userName} added {itemName} to {cart}");
          }
        },
        {
          "upsert-item-by-name",
          async command =>
          {
            Dictionary<string, object> parameters = MoLibrary.ReadCommandParameter(command);

            string userName = command.User.Username;
            string cartName = parameters.GetCastedValueOrDefault("cart-name", string.Empty);
            string itemName = parameters.GetCastedValueOrDefault("item-name", string.Empty);
            string itemLink = parameters.GetCastedValueOrDefault("item-link", string.Empty);
            double itemPrice = parameters.GetCastedValueOrDefault("item-price", 0.0);
            int quantity = parameters.GetCastedValueOrDefault("quantity", 1);

            Cart? cart = await Cart.TryGet(cartName);
            if (cart == null)
            {
              await command.RespondAsync($"No Cart with name: {cartName} was found.");
              return;
            }

            Simp simp = await Simp.TryGet(userName) ?? await Simp.RegisterSimp(userName);

            var item = new Item(itemName, itemLink, (decimal)itemPrice);
            if (!await cart.UpsertItem(simp, item, quantity))
            {
              await command.RespondAsync($"{cart} is already closed.");
              return;
            }

            await command.RespondAsync($"{userName} added {itemName} to {cart}");
          }
        },
        //-+-+-+-+-+-+-+-+
        // List All Carts
        //-+-+-+-+-+-+-+-+
        {
          "list-all-carts",
          async command =>
          {
            Dictionary<string, object> parameters = MoLibrary.ReadCommandParameter(command);

            string userName = command.User.Username;
            bool onlyOpen = parameters.GetCastedValueOrDefault(
              "only-open-carts",
              x => Convert.ToBoolean(x)
            );

            StringBuilder strResult = new();
            List<Cart> cartList = await Cart.GetAllCarts();

            if (cartList.IsNullOrEmpty())
            {
              await command.RespondAsync("No Cart detected, what a cheapskate group");
              return;
            }

            foreach (Cart cart in cartList)
            {
              if (onlyOpen && cart.Status != Cart.CartStatus.Open)
                continue;
              strResult.AppendLine(await cart.GetDetails());
            }

            await command.RespondAsync($"{strResult}");
          }
        },
        {
          "get-cart-stats-by-id",
          async command =>
          {
            Dictionary<string, object> parameters = MoLibrary.ReadCommandParameter(command);

            string userName = command.User.Username;
            int cartId = parameters.GetCastedValueOrDefault("cart-id", -1);

            Cart? cart = await Cart.TryGet(cartId);
            if (cart == null)
            {
              await command.RespondAsync($"No Cart with id: {cartId} was found.");
              return;
            }

            Dictionary<decimal, uint> itemFreqMap = new();
            Dictionary<decimal, uint> simpFreqMap = new();
            IReadOnlyList<CartItems> cartItems = await cart.TryGetCartItems();
            foreach (CartItems ci in cartItems)
            {
              decimal totalSimp = 0;
              int len = ci.Items.Count;
              for (var i = 0; i < len; i++)
              {
                decimal price = ci.Items[i].PriceSGD;
                var quantity = (uint)ci.Quantities[i];
                totalSimp += price * quantity;
                itemFreqMap.AddFrequency(price, quantity);
              }
              simpFreqMap.AddFrequency(totalSimp);
            }
            if (itemFreqMap.IsNullOrEmpty() || simpFreqMap.IsNullOrEmpty())
            {
              await command.RespondAsync("Cart was empty, pitiful weakling.");
              return;
            }

            TotalStatistics statsByItem = TotalStatistics.Calculate(itemFreqMap);
            TotalStatistics statsBySimp = TotalStatistics.Calculate(simpFreqMap);

            StringBuilder strResult = new();
            strResult.AppendLine($"# Statistics for Cart: {cart}");
            strResult.AppendLine("## By Items:");
            strResult.AppendLine($"Total Items: {statsByItem.Count:N0}");
            strResult.AppendLine($"Average Cost: {statsByItem.Mean:C2}");
            strResult.AppendLine($"Standard Deviation: {statsByItem.StandardDeviation:C2}");
            strResult.AppendLine($"Gini Coefficient: {statsByItem.GiniCoefficient:P2}");
            strResult.AppendLine($"Most Expensive: {statsByItem.Maximum:C2}");
            strResult
              .Append("Box Plot: ")
              .Append($"{statsByItem.LowerFence:C2}")
              .Append(" <- [ ")
              .Append($"{statsByItem.Q1:C2}")
              .Append(" | ")
              .Append($"{statsByItem.Median:C2}")
              .Append(" | ")
              .Append($"{statsByItem.Q3:C2}")
              .Append(" ] -> ")
              .Append($"{statsByItem.UpperFence:C2}")
              .AppendLine();

            strResult.AppendLine("## By Simps:");
            strResult.AppendLine($"Total Perticipants: {statsBySimp.Count:N0}");
            strResult.AppendLine($"Average Spending: {statsBySimp.Mean:C2}");
            strResult.AppendLine($"Standard Deviation: {statsBySimp.StandardDeviation:C2}");
            strResult.AppendLine($"Gini Coefficient: {statsBySimp.GiniCoefficient:P2}");
            strResult.AppendLine($"Biggest Spender: {statsBySimp.Maximum:C2}");
            strResult
              .Append("Box Plot: ")
              .Append($"{statsBySimp.LowerFence:C2}")
              .Append(" <- [ ")
              .Append($"{statsBySimp.Q1:C2}")
              .Append(" | ")
              .Append($"{statsBySimp.Median:C2}")
              .Append(" | ")
              .Append($"{statsBySimp.Q3:C2}")
              .Append(" ] -> ")
              .Append($"{statsBySimp.UpperFence:C2}")
              .AppendLine();

            await command.RespondAsync($"{strResult}");
          }
        },
        {
          "register-me",
          async command =>
          {
            Dictionary<string, object> parameters = MoLibrary.ReadCommandParameter(command);

            string userName = command.User.Username;
            string nickname = parameters.GetCastedValueOrDefault("nickname", userName);

            StringBuilder strResult = new();
            try
            {
              Simp? simp = await Simp.TryGet(userName);
              if (simp != null)
              {
                await Simp.UpdateNickname(simp, nickname);
                strResult.AppendLine("You are already registered. Updating your nickname.");
              }
              else
              {
                simp = await Simp.RegisterSimp(userName, nickname, command.User.GetAvatarUrl());
                strResult.AppendLine(
                  $"Successfully registered Simp: {simp.simpName} ({simp.uDex})"
                );
              }
            }
            catch (Exception e)
            {
              strResult.AppendLine($"Error registering Simp:\r\n{e.ToStringDemystified()}");
            }
            finally
            {
              await command.RespondAsync(strResult.ToString());
            }
          }
        },
        {
          "bau-bau",
          async command =>
          {
            Dictionary<string, object> parameters = MoLibrary.ReadCommandParameter(command);

            string userName = command.User.Username;
            int count = parameters.GetCastedValueOrDefault("how-many", 1);
            if (userName == "jagerking779")
            {
              await command.RespondAsync("Beli Hina dulu baru bisa.");
            }
            else
            {
              StringBuilder strResult = new();
              for (var i = 0; i < count; i++)
                strResult.AppendLine(PCG.global.NextDouble() < (3.0 / 100) ? "Umapyoi" : "Bau Bau");
              await command.RespondAsync(strResult.ToString());
            }
          }
        },
        {
          "schedule-weekly-uma",
          async command =>
          {
            Dictionary<string, object> parameters = MoLibrary.ReadCommandParameter(command);
            string userName = command.User.Username;

            //int count = parameters.GetCastedValueOrDefault("how-many", 3);
            await command.RespondAsync(UmaScheduler.CreateSchedule());
          }
        },
      }.ToImmutableDictionary();
  }
}
