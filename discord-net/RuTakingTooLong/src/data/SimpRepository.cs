using System.Linq.Expressions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace HoloSimpID {
  public partial class Simp {
    public static async Task<Simp> RegisterSimp(
        SocketUser user, AppDbContext? db = null) => await RegisterSimp(user.Username,
        user.GlobalName, user.GetAvatarUrl(), db: db);
    public static async Task<Simp> RegisterSimp(string dcUserName, string simpName = "",
        string profilePicPath = "", AppDbContext? db = null) {
      bool localContext   = db == null;
      db                ??= new AppDbContext();

      Simp simp = _RegisterSimp(dcUserName, simpName, profilePicPath);
      await db.Simps.AddAsync(simp);
      await db.SaveChangesAsync();

      if (localContext) {
        await db.DisposeAsync();
      }
      return simp;
    }

    public static async Task<Simp?> TryGet(
        SocketUser user, AppDbContext? db = null) => await TryGet(user.Username, db: db);

    public static async Task<Simp?> TryGet(
        int uDex, Expression<Func<Simp, bool>>? predicate = null, AppDbContext? db = null) {
      predicate ??=
          _              => true;
      bool localContext   = db == null;
      db                ??= new AppDbContext();

      Simp? simp = await db.Simps.Where(predicate).SingleOrDefaultAsync(c => c.uDex == uDex);

      if (localContext) {
        await db.DisposeAsync();
      }
      return simp;
    }

    public static async Task<Simp?> TryGet(
        string userName, Expression<Func<Simp, bool>>? predicate = null, AppDbContext? db = null) {
      predicate ??=
          _              => true;
      bool localContext   = db == null;
      db                ??= new AppDbContext();

      Simp? simp =
          await db.Simps.Where(predicate).SingleOrDefaultAsync(c => c.dcUserName == userName);

      if (localContext) {
        await db.DisposeAsync();
      }
      return simp;
    }

    public async Task UpdateNickname(
        string nickname, AppDbContext? db = null) => await UpdateNickname(this, nickname, db);
    public static async Task UpdateNickname(Simp simp, string nickname, AppDbContext? db = null) {
      bool localContext   = db == null;
      db                ??= new AppDbContext();

      Simp? dbSimp = await TryGet(simp.uDex, db: db);
      if (dbSimp != null) {
        dbSimp.simpName = nickname;
        await db.SaveChangesAsync();
      }
      if (localContext)
        await db.DisposeAsync();
    }
  }
}
