// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace apptab.Models
{
    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model110")
        {
        }

        public virtual DbSet<CPTADMIN_FAUTREOPERATION> CPTADMIN_FAUTREOPERATION { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CPTADMIN_FAUTREOPERATION>()
                .Property(e => e.COURSDEVISE)
                .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_FAUTREOPERATION>()
                .Property(e => e.COURSRAPPORT)
                .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_FAUTREOPERATION>()
                .Property(e => e.MONTANTLOCAL)
                .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_FAUTREOPERATION>()
                .Property(e => e.MONTANTRAPPORT)
                .HasPrecision(30, 12);

            modelBuilder.Entity<CPTADMIN_FAUTREOPERATION>()
                .Property(e => e.MONTANTDEVISE)
                .HasPrecision(30, 12);
        }
    }
}
