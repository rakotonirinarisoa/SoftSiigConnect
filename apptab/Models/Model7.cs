// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace apptab.Models
{
    public partial class Model7 : DbContext
    {
        public Model7()
            : base("name=Model7")
        {
        }

        public virtual DbSet<OPA_HISTORIQUEBR> OPA_HISTORIQUEBR { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
