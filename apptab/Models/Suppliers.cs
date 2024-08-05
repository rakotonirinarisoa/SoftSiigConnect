namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Suppliers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Suppliers()
        {
            SuppliersEmails = new HashSet<SuppliersEmails>();
        }

        public Guid Id { get; set; }

        [StringLength(255)]
        public string NIF { get; set; }

        [StringLength(255)]
        public string STAT { get; set; }

        [StringLength(255)]
        public string MAIL { get; set; }

        [StringLength(255)]
        public string CONTACT { get; set; }

        [StringLength(255)]
        public string CIN { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public Guid ProjectId { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? DeletionDate { get; set; }

        public Guid? DeletedBy { get; set; }

        public virtual DocumentsSenders DocumentsSenders { get; set; }

        public virtual Projects Projects { get; set; }

        public virtual Users Users { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SuppliersEmails> SuppliersEmails { get; set; }
    }
}
