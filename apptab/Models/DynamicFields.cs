namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DynamicFields
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DynamicFields()
        {
            DocumentDynamicFields = new HashSet<DocumentDynamicFields>();
            DocumentsDynamicAttachements = new HashSet<DocumentsDynamicAttachements>();
            DynamicFieldItems = new HashSet<DynamicFieldItems>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Label { get; set; }

        public bool IsForUsersProject { get; set; }

        public bool IsForSuppliers { get; set; }

        public bool IsRequired { get; set; }

        public int Type { get; set; }

        public bool IsGlobal { get; set; }

        public Guid ProjectId { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? DeletionDate { get; set; }

        public Guid? DeletedBy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentDynamicFields> DocumentDynamicFields { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentsDynamicAttachements> DocumentsDynamicAttachements { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DynamicFieldItems> DynamicFieldItems { get; set; }

        public virtual Users Users { get; set; }

        public virtual Users Users1 { get; set; }

        public virtual Projects Projects { get; set; }

        public virtual DynamicFieldTypes DynamicFieldTypes { get; set; }
    }
}
