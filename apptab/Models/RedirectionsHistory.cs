namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RedirectionsHistory")]
    public partial class RedirectionsHistory
    {
        public long Id { get; set; }

        public Guid FromUserId { get; set; }

        public Guid? FromDocumentStepId { get; set; }

        public Guid? ToDocumentStepId { get; set; }

        public Guid DocumentId { get; set; }

        [StringLength(255)]
        public string Comment { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
