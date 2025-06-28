﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HoloSimpID
{
    public partial class Simp
    {
        [Key]
        public int uDex { get; set; }
        public string dcUserName { get; set; }
        public string simpName { get; set; }
        public string profilePicPath { get; set; }

        public override string ToString() => simpName;

        private static Simp _RegisterSimp(string dcUserName, string simpName = "", string profilePicPath = "")
        {
            return new Simp
            {
                dcUserName = dcUserName,
                simpName = simpName,
                profilePicPath = profilePicPath
            };
        }
    }
}