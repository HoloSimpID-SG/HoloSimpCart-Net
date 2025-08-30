using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MMOR.NET.Consts;
using MMOR.NET.Utilities;

namespace HoloSimpID
{
  public partial class Cart
  {
    public async Task<string> GetDetails(AppDbContext? db = null)
    {
      return await GetDetails(this, db);
    }

    public async Task<IReadOnlyList<CartItems>> TryGetCartItems(AppDbContext? db = null) =>
      await TryGetCartItems(this, db);

    public static async Task<IReadOnlyList<CartItems>> TryGetCartItems(
      Cart cart,
      AppDbContext? db = null
    )
    {
      bool localContext = db == null;
      db ??= new AppDbContext();

      IReadOnlyList<CartItems> result = await db
        .CartItems.Where(x => x.cartDex == cart.uDex)
        .Include(c => c.Cart)
        .Include(c => c.Simp)
        .ToListAsync();

      if (localContext)
        await db.DisposeAsync();
      return result;
    }

    public static async Task<string> GetDetails(Cart cart, AppDbContext? db = null)
    {
      bool localContext = db == null;
      db ??= new AppDbContext();

      Simp? owner = await Simp.TryGet(cart.OwnerDex, db: db);

      StringBuilder strResult = new();
      strResult.AppendLine($"# {cart}");
      strResult.AppendLine($"Owned by: {owner!.simpName}");
      strResult.Append("Status: ");
      strResult.AppendLine($"{cart.Status}");
      strResult.AppendLine($"Opened at: {cart.DateOpen.ToLocalTime()}");
      strResult.AppendLine("Item List:");

      IEnumerable<CartItems> cartItems = await TryGetCartItems(cart, db);
      {
        foreach (CartItems simpCart in cartItems)
        {
          Simp? simp = await Simp.TryGet(simpCart.simpDex, db: db);
          if (simp == null)
          {
            continue;
          }

          strResult.AppendLine($"- {simp}:");
          for (var i = 0; i < simpCart.Items.Count; i++)
          {
            strResult
              .Append("  - ")
              .Append(simpCart.Items[i])
              .Append(" (")
              .Append(simpCart.Items[i].PriceSGD.ToCurrency())
              .Append(")")
              .Append(Consts.multiplierSign)
              .Append(simpCart.Quantities[i])
              .AppendLine();
          }
        }
      }
      if (localContext)
      {
        await db.DisposeAsync();
      }
      return strResult.ToString();
    }

    public static async Task<Cart> OpenCart(
      Simp owner,
      string cartName,
      DateTime? datePlan = null,
      decimal shippingCost = 0m,
      AppDbContext? db = null
    )
    {
      bool localContext = db == null;
      db ??= new AppDbContext();

      Cart cart = _OpenCart(owner, cartName, datePlan, shippingCost);

      db.Carts.Add(cart);
      await db.SaveChangesAsync();

      if (localContext)
      {
        await db.DisposeAsync();
      }
      return cart;
    }

    public static async Task<Cart?> TryGet(
      int uDex,
      Expression<Func<Cart, bool>>? predicate = null,
      AppDbContext? db = null
    )
    {
      predicate ??= _ => true;
      bool localContext = db == null;
      db ??= new AppDbContext();

      Cart? cart = await db
        .Carts.Where(predicate)
        .Include(c => c.Owner)
        .SingleOrDefaultAsync(c => c.uDex == uDex);

      if (localContext)
      {
        await db.DisposeAsync();
      }
      return cart;
    }

    public static async Task<Cart?> TryGet(
      string cartName,
      Expression<Func<Cart, bool>>? predicate = null,
      AppDbContext? db = null
    )
    {
      predicate ??= _ => true;
      bool localContext = db == null;
      db ??= new AppDbContext();

      Cart? cart = await db
        .Carts.Where(predicate)
        .Include(c => c.Owner)
        .SingleOrDefaultAsync(c => c.CartName == cartName);

      if (localContext)
      {
        await db.DisposeAsync();
      }
      return cart;
    }

    public static async Task<List<Cart>> GetAllCarts(
      Expression<Func<Cart, bool>>? predicate = null,
      AppDbContext? db = null
    )
    {
      predicate ??= _ => true;
      bool localContext = db == null;
      db ??= new AppDbContext();
      List<Cart> result = await db.Carts.Where(predicate).ToListAsync();
      if (localContext)
      {
        await db.DisposeAsync();
      }
      return result;
    }

    public async Task CloseCart(AppDbContext? db = null)
    {
      await CloseCart(this, db);
    }

    public static async Task CloseCart(Cart cart, AppDbContext? db = null)
    {
      bool localContext = db == null;
      db ??= new AppDbContext();

      await db
        .Carts.Where(c => c.uDex == cart.uDex)
        .ExecuteUpdateAsync(setter => setter.SetProperty(c => c.DateClose, DateTime.UtcNow));

      if (localContext)
      {
        await db.DisposeAsync();
      }
    }

    public async Task SetCartDelivered(AppDbContext? db = null) => await SetCartDelivered(this, db);

    public static async Task SetCartDelivered(Cart cart, AppDbContext? db = null)
    {
      bool localContext = db == null;
      db ??= new AppDbContext();

      await db
        .Carts.Where(c => c.uDex == cart.uDex)
        .ExecuteUpdateAsync(setter => setter.SetProperty(c => c.DateDelivered, DateTime.UtcNow));

      if (localContext)
      {
        await db.DisposeAsync();
      }
    }

    public async Task<bool> UpsertItem(
      Simp simp,
      Item item,
      int quantity = 1,
      AppDbContext? db = null
    )
    {
      return await UpsertItem(this, simp, item, quantity, db);
    }

    public static async Task<bool> UpsertItem(
      Cart cart,
      Simp simp,
      Item item,
      int quantity = 1,
      AppDbContext? db = null
    )
    {
      if (cart.Status != CartStatus.Open)
      {
        return false;
      }

      bool localContext = db == null;
      db ??= new AppDbContext();

      CartItems? cartItems = await db.CartItems.SingleOrDefaultAsync(x =>
        x.cartDex == cart.uDex && x.simpDex == simp.uDex
      );

      if (cartItems == null)
      {
        if (quantity > 0)
        {
          cartItems = new CartItems
          {
            cartDex = cart.uDex,
            simpDex = simp.uDex,
            Items = [item],
            Quantities = [quantity],
          };
          await db.CartItems.AddAsync(cartItems);
        }
        else
        {
          if (localContext)
            await db.DisposeAsync();
          return false;
        }
      }
      else
      {
        int index = cartItems.Items.FindIndex(i => i.Name == item.Name);
        if (index >= 0)
        {
          if (quantity <= 0)
          {
            cartItems.Items.RemoveAt(index);
            cartItems.Quantities.RemoveAt(index);
          }
          else
          {
            cartItems.Quantities[index] = quantity;
          }
        }
        else
        {
          if (quantity >= 0)
          {
            cartItems.Items.Add(item);
            cartItems.Quantities.Add(quantity);
          }
          else
          {
            if (localContext)
              await db.DisposeAsync();
            return false;
          }
        }
        db.CartItems.Update(cartItems);
      }

      await db.SaveChangesAsync();
      if (localContext)
      {
        await db.DisposeAsync();
      }
      return true;
    }
  }
}
