namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DocumentTypesSteps
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DocumentTypesSteps()
        {
            DocumentTypesUsersSteps = new HashSet<DocumentTypesUsersSteps>();
        }

        public Guid Id { get; set; }

        public int StepNumber { get; set; }

        public double ProcessingDuration { get; set; }

        public Guid DocumentTypeId { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? DeletionDate { get; set; }

        public Guid? DeletedBy { get; set; }

        public string ProcessingDescription { get; set; }

        public virtual DocumentTypes DocumentTypes { get; set; }

        public virtual Users Users { get; set; }

        public virtual Users Users1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentTypesUsersSteps> DocumentTypesUsersSteps { get; set; }
    }
}
