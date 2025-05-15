using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HoloSimpID
{
    public static class PsuedoDB
    {
        public static readonly Simp[] Simps = new Simp[]
        {
            new("J779"),
            new("Ndaru"),
            new("Ekavins"),
            new("Cahyadi"),
            new("Keti"),
            new("Mute"),
            new("Mute"),
        };

        public static Dictionary<uint, IIndexer> carts = new();

        public static void AddAutoKey(this Dictionary<uint, IIndexer> db, IIndexer obj) => 
            db.Add(obj.uDex, obj);
    }
}
