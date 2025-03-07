namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Documents
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Documents()
        {
            Attachements = new HashSet<Attachements>();
            DocumentDynamicFields = new HashSet<DocumentDynamicFields>();
            DocumentsDynamicAttachements = new HashSet<DocumentsDynamicAttachements>();
            DocumentsReceptions = new HashSet<DocumentsReceptions>();
            DocumentSteps = new HashSet<DocumentSteps>();
            UsersDocumentsAccesses = new HashSet<UsersDocumentsAccesses>();
            ValidationsHistory = new HashSet<ValidationsHistory>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Filename { get; set; }

        [Required]
        [StringLength(255)]
        public string OriginalFilename { get; set; }

        [Required]
        [StringLength(255)]
        public string Url { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        public string Object { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        public int Status { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid SenderId { get; set; }

        public DateTime? DeletionDate { get; set; }

        public Guid? DeletedBy { get; set; }

        public bool CanBeAccessedByAnyone { get; set; }

        [StringLength(255)]
        public string PhysicalLocation { get; set; }

        public bool RSF { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Montant { get; set; }

        [Required]
        [StringLength(50)]
        public string Site { get; set; }

        public Guid? ProjectId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Attachements> Attachements { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentDynamicFields> DocumentDynamicFields { get; set; }

        public virtual Users Users { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentsDynamicAttachements> DocumentsDynamicAttachements { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentsReceptions> DocumentsReceptions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentSteps> DocumentSteps { get; set; }

        public virtual DocumentsSenders DocumentsSenders { get; set; }

        public virtual SuppliersDocumentsSendings SuppliersDocumentsSendings { get; set; }

        public virtual SuppliersDocumentsAcknowledgements SuppliersDocumentsAcknowledgements { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsersDocumentsAccesses> UsersDocumentsAccesses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ValidationsHistory> ValidationsHistory { get; set; }
    }
}
