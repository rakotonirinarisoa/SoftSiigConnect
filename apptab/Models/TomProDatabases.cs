namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TomProDatabases
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        public string DatabaseName { get; set; }

        public int DatabaseId { get; set; }

        public Guid TomProConnectionId { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? DeletionDate { get; set; }

        public Guid? DeletedBy { get; set; }

        public virtual TomProConnections TomProConnections { get; set; }

        public virtual Users Users { get; set; }

        public virtual Users Users1 { get; set; }
    }
}
