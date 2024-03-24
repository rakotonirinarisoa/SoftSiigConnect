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
            : base("name=Model12")
        {
        }

        public virtual DbSet<OPA_REGLEMENTBR> OPA_REGLEMENTBR { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OPA_REGLEMENTBR>()
                .Property(e => e.ID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<OPA_REGLEMENTBR>()
                .Property(e => e.MONTANT)
                .HasPrecision(18, 0);
        }
    }
}
