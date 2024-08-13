namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SuppliersDocumentsAcknowledgements
    {
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid InitiatorId { get; set; }

        public virtual Documents Documents { get; set; }

        public virtual Users Users { get; set; }
        [StringLength(50)]
        public string ReferenceInterne { get; set; }
        public int Serie { get; set; }
        public int DocumentYear { get; set; }
        [StringLength(50)]
        public string DocumentSite { get; set; }
    }
}
