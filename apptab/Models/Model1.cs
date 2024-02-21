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
            : base("name=Model18")
        {
        }

        public virtual DbSet<OPA_VALIDATIONS> OPA_VALIDATIONS { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OPA_VALIDATIONS>()
                .Property(e => e.IDREGLEMENT)
                .HasPrecision(18, 0);

            modelBuilder.Entity<OPA_VALIDATIONS>()
                .Property(e => e.Debit)
                .HasPrecision(18, 0);

            modelBuilder.Entity<OPA_VALIDATIONS>()
                .Property(e => e.Credit)
                .HasPrecision(18, 0);

            modelBuilder.Entity<OPA_VALIDATIONS>()
                .Property(e => e.MontantDevise)
                .HasPrecision(18, 0);

            modelBuilder.Entity<OPA_VALIDATIONS>()
                .Property(e => e.Devise)
                .IsFixedLength();
        }
    }
}
