namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DocumentsDynamicAttachements
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Filename { get; set; }

        [Required]
        [StringLength(255)]
        public string FilePath { get; set; }

        public Guid DocumentId { get; set; }

        public Guid DynamicFieldId { get; set; }

        public DateTime? DeletionDate { get; set; }

        public virtual Documents Documents { get; set; }

        public virtual DynamicFields DynamicFields { get; set; }
    }
}