namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OPA_BASE
    {
        public int ID { get; set; }

        [StringLength(100)]
        public string NOMBASE { get; set; }

        public int? INCREMENTATION { get; set; }

        public int? INCRORDREVIR { get; set; }

        public int? IDSOCIETE { get; set; }
    }
}
