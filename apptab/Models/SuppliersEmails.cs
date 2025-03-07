namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SuppliersEmails
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        public Guid SupplierId { get; set; }

        public virtual Suppliers Suppliers { get; set; }
    }
}
