using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MMOR.Utils.Consts;
using MMOR.Utils.Utilities;

namespace HoloSimpID
{
    public partial class Cart
    {
        public async Task<string> GetDetails(AppDbContext? db = null) { return await GetDetails(uDex, db); }

        public static async Task<string> GetDetails(int uDex, AppDbContext? db = null)
        {
            bool localContext = db == null;
            db ??= new AppDbContext();

            Cart? cart = await TryGet(uDex, db: db);

            if (cart == null)
            {
                return string.Empty;
            }

            Simp? owner = await Simp.TryGet(cart.Owner.uDex, db: db);

            StringBuilder strResult = new();
            strResult.AppendLine($"# {cart.CartName} (id: {uDex})");
            strResult.AppendLine($"Owned by: {owner!.simpName}");
            strResult.Append("Status: ");
            strResult.AppendLine($"{cart.Status}");
            strResult.AppendLine($"Opened at: {cart.DateOpen.ToLocalTime()}");
            strResult.AppendLine("Item List:");

            IEnumerable<CartItems> cartItems = await db.CartItems
                .Where(x => x.cartDex == uDex)
                .Include(c => c.Cart)
                .Include(c => c.Simp)
                .ToListAsync();
            if (!cartItems.IsNullOrEmpty())
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
                        strResult.Append("  - ")
                            .Append(simpCart.Items[i])
                            .Append(" (").Append(simpCart.Items[i].PriceSGD.toCurrency()).Append(")")
                            .Append(Consts.multiplierSign).Append(simpCart.Quantities[i])
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

        public static async Task<Cart> OpenCart(Simp owner, string cartName, DateTime? datePlan = null,
            decimal shippingCost = 0m,
            AppDbContext? db = null)
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

        public static async Task<Cart?> TryGet(int uDex, Expression<Func<Cart, bool>>? predicate = null,
            AppDbContext? db = null)
        {
            predicate ??= _ => true;
            bool localContext = db == null;
            db ??= new AppDbContext();

            Cart? cart = await db.Carts
                .Where(predicate)
                .Include(c => c.Owner)
                .SingleOrDefaultAsync(c => c.uDex == uDex);

            if (localContext)
            {
                await db.DisposeAsync();
            }
            return cart;
        }

        public static async Task<Cart?> TryGet(string cartName, Expression<Func<Cart, bool>>? predicate = null,
            AppDbContext? db = null)
        {
            predicate ??= _ => true;
            bool localContext = db == null;
            db ??= new AppDbContext();

            Cart? cart = await db.Carts
                .Where(predicate)
                .Include(c => c.Owner)
                .SingleOrDefaultAsync(c => c.CartName == cartName);

            if (localContext)
            {
                await db.DisposeAsync();
            }
            return cart;
        }

        public static async Task<List<Cart>> GetAllCarts(Expression<Func<Cart, bool>>? predicate = null,
            AppDbContext? db = null)
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

        public async Task CloseCart(AppDbContext? db = null) { await CloseCart(uDex, db); }

        public static async Task CloseCart(int uDex, AppDbContext? db = null)
        {
            bool localContext = db == null;
            db ??= new AppDbContext();

            await db.Carts
                .Where(cart => cart.uDex == uDex)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(cart => cart.DateClose, DateTime.UtcNow)
                );

            if (localContext)
            {
                await db.DisposeAsync();
            }
        }

        public async Task<bool> UpsertItem(Simp simp, Item item, int quantity = 1, AppDbContext? db = null)
        {
            return await UpsertItem(this, simp, item, quantity, db);
        }

        public static async Task<bool> UpsertItem(Cart cart, Simp simp, Item item, int quantity = 1,
            AppDbContext? db = null)
        {
            if (cart.Status != CartStatus.Open)
            {
                return false;
            }

            bool localContext = db == null;
            db ??= new AppDbContext();

            CartItems? cartItems = await db.CartItems.SingleOrDefaultAsync(x =>
                x.cartDex == cart.uDex &&
                x.simpDex == simp.uDex);

            if (cartItems == null)
            {
                cartItems = new CartItems
                {
                    cartDex = cart.uDex,
                    simpDex = simp.uDex,
                    Items = [item],
                    Quantities = [quantity]
                };
                await db.CartItems.AddAsync(cartItems);
            }
            else
            {
                int index = cartItems.Items.FindIndex(i => i.Name == item.Name);
                if (index >= 0)
                {
                    cartItems.Quantities[index] += quantity;
                }
                else
                {
                    cartItems.Items.Add(item);
                    cartItems.Quantities.Add(quantity);
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