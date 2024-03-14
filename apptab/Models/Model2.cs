// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace apptab.Models
{
    public partial class Model2 : DbContext
    {
        public Model2()
            : base("name=Model22")
        {
        }

        public virtual DbSet<MOP> MOP { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTLOC)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTDEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTRAP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTTVA)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTAUTRETAXE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTRETENUE)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTTVADEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTTVARAP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTAUTRETAXEDEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTAUTRETAXERAP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTRETENUEDEV)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTRETENUERAP)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.MONTANTCOMMISSION)
                .HasPrecision(30, 6);

            modelBuilder.Entity<MOP>()
                .Property(e => e.TAXECOMMISSION)
                .HasPrecision(30, 6);
        }
    }
}
