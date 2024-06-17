namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Documents
    {
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
    }
}
