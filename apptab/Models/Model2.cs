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

        public virtual DbSet<ANOMALIE_G> ANOMALIE_G { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
