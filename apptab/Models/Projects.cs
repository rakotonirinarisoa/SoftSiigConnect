namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Projects
    {
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

        [StringLength(255)]
        public string ServerName { get; set; }

        [StringLength(255)]
        public string Login { get; set; }

        [StringLength(255)]
        public string Password { get; set; }

        public int? DatabaseId { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? DeletionDate { get; set; }
    }
}
