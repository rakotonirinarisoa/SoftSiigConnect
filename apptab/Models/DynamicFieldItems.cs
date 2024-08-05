namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DynamicFieldItems
    {
        public Guid Id { get; set; }

        [StringLength(255)]
        public string Value { get; set; }

        public Guid DynamicFieldId { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? DeletionDate { get; set; }

        public Guid? DeletedBy { get; set; }

        public virtual Users Users { get; set; }

        public virtual Users Users1 { get; set; }

        public virtual DynamicFields DynamicFields { get; set; }
    }
}
