namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Projects
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Projects()
        {
            DocumentTypes = new HashSet<DocumentTypes>();
            DynamicFields = new HashSet<DynamicFields>();
            Suppliers = new HashSet<Suppliers>();
            TomProConnections = new HashSet<TomProConnections>();
            Users = new HashSet<Users>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Storage { get; set; }

        public bool HasAccessToInternalUsersHandling { get; set; }

        public bool HasAccessToSuppliersHandling { get; set; }

        public bool HasAccessToProcessingCircuitsHandling { get; set; }

        public bool HasAccessToSignMySelfFeature { get; set; }

        public bool HasAccessToArchiveImmediatelyFeature { get; set; }

        public bool HasAccessToGlobalDynamicFieldsHandling { get; set; }

        public bool HasAccessToPhysicalLocationHandling { get; set; }

        public bool HasAccessToTomProLinking { get; set; }

        public bool HasAccessToUsersConnectionsInformation { get; set; }

        public bool HasAccessToNumericLibrary { get; set; }

        public bool HasAccessToDocumentTypesHandling { get; set; }

        public bool HasAccessToDocumentsAccessesHandling { get; set; }

        public bool HasAccessToRSF { get; set; }

        [StringLength(255)]
        public string ServerName { get; set; }

        [StringLength(255)]
        public string Login { get; set; }

        [StringLength(255)]
        public string Password { get; set; }

        [StringLength(255)]
        public string DataBaseName { get; set; }

        public int? DatabaseId { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? DeletionDate { get; set; }

        public int? SoaId { get; set; }

        [StringLength(255)]
        public string SoaName { get; set; }

        public string Sites { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentTypes> DocumentTypes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DynamicFields> DynamicFields { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Suppliers> Suppliers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TomProConnections> TomProConnections { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Users> Users { get; set; }
    }
}
