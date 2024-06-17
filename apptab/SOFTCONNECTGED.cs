// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace apptab
{
    public partial class SOFTCONNECTGED : DbContext
    {
        public SOFTCONNECTGED()
            : base(connex)
        {
        }

        public static string connex = "name=SOFTCONNECTGED";

        public virtual DbSet<Documents> Documents { get; set; }
        public virtual DbSet<Projects> Projects { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Projects>()
                .Property(e => e.ServerName)
                .IsUnicode(false);

            modelBuilder.Entity<Projects>()
                .Property(e => e.Login)
                .IsUnicode(false);

            modelBuilder.Entity<Projects>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Users1)
                .WithOptional(e => e.Users2)
                .HasForeignKey(e => e.DeletedBy);
        }
    }
}
