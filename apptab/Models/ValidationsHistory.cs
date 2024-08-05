namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ValidationsHistory")]
    public partial class ValidationsHistory
    {
        public long Id { get; set; }

        public Guid FromUserId { get; set; }

        public Guid? ToDocumentStepId { get; set; }

        public Guid DocumentId { get; set; }

        [StringLength(255)]
        public string Comment { get; set; }

        public int ActionType { get; set; }

        public DateTime CreationDate { get; set; }

        public virtual Documents Documents { get; set; }

        public virtual DocumentSteps DocumentSteps { get; set; }

        public virtual Users Users { get; set; }
    }
}
