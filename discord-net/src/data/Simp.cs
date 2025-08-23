using System.ComponentModel.DataAnnotations;
using MMOR.NET.Utilities;

namespace HoloSimpID
{
  public partial class Simp
  {
    [Key]
    public int uDex { get; set; }
    public string dcUserName { get; set; } = null!;
    public string simpName { get; set; }
    public string profilePicPath { get; set; }

    public override string ToString()
    {
      return simpName;
    }

    private static Simp _RegisterSimp(
      string dcUserName,
      string simpName = "",
      string profilePicPath = ""
    )
    {
      return new Simp
      {
        dcUserName = dcUserName,
        simpName = simpName.IsNullOrEmpty() ? dcUserName : simpName,
        profilePicPath = profilePicPath,
      };
    }
  }
}
