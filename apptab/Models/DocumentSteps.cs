namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DocumentSteps
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DocumentSteps()
        {
            DocumentsFields = new HashSet<DocumentsFields>();
            UsersSteps = new HashSet<UsersSteps>();
            ValidationsHistory = new HashSet<ValidationsHistory>();
        }

        public Guid Id { get; set; }

        public int StepNumber { get; set; }

        public double ProcessingDuration { get; set; }

        public int Role { get; set; }

        [StringLength(255)]
        public string Color { get; set; }

        [StringLength(500)]
        public string Message { get; set; }
        public string ProcessingDescription { get; set; }

        public Guid DocumentId { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? DeletionDate { get; set; }

        public virtual Documents Documents { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentsFields> DocumentsFields { get; set; }

        public virtual UserDocumentRoles UserDocumentRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsersSteps> UsersSteps { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ValidationsHistory> ValidationsHistory { get; set; }
    }
}
