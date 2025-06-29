using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace HoloSimpID
{
    public partial class Simp
    {
        public static async Task<Simp> RegisterSimp(string dcUserName, string simpName = "", string profilePicPath = "",
            AppDbContext? db = null)
        {
            bool localContext = db == null;
            db ??= new AppDbContext();

            Simp simp = _RegisterSimp(dcUserName, simpName, profilePicPath);
            await db.Simps.AddAsync(simp);
            await db.SaveChangesAsync();

            if (localContext)
            {
                await db.DisposeAsync();
            }
            return simp;
        }

        public static async Task<Simp?> TryGet(int uDex, Expression<Func<Simp, bool>>? predicate = null,
            AppDbContext? db = null)
        {
            predicate ??= _ => true;
            bool localContext = db == null;
            db ??= new AppDbContext();

            Simp? simp = await db.Simps
                .Where(predicate)
                .SingleOrDefaultAsync(c => c.uDex == uDex);

            if (localContext)
            {
                await db.DisposeAsync();
            }
            return simp;
        }

        public static async Task<Simp?> TryGet(string userName, Expression<Func<Simp, bool>>? predicate = null,
            AppDbContext? db = null)
        {
            predicate ??= _ => true;
            bool localContext = db == null;
            db ??= new AppDbContext();

            Simp? simp = await db.Simps
                .Where(predicate)
                .SingleOrDefaultAsync(c => c.dcUserName == userName);

            if (localContext)
            {
                await db.DisposeAsync();
            }
            return simp;
        }
    }
}