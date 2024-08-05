namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DocumentsReceptions
    {
        public int Id { get; set; }

        public Guid DocumentId { get; set; }

        public Guid UserId { get; set; }

        public DateTime CreationDate { get; set; }

        public virtual Documents Documents { get; set; }

        public virtual Users Users { get; set; }
    }
}
