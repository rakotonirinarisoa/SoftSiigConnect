namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DocumentsFields
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Variable { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public int FirstPage { get; set; }

        public int LastPage { get; set; }

        public int FieldTypeId { get; set; }

        public Guid DocumentStepId { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? DeletionDate { get; set; }

        public virtual DocumentSteps DocumentSteps { get; set; }

        public virtual FieldTypes FieldTypes { get; set; }
    }
}
