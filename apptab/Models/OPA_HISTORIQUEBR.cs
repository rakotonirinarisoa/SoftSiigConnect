namespace apptab.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OPA_HISTORIQUEBR
    {
        public int ID { get; set; }

        [StringLength(100)]
        public string NUMENREG { get; set; }

        public DateTime? DATEAFB { get; set; }

        public int? IDUSER { get; set; }

        [StringLength(100)]
        public string AFB { get; set; }

        public int? IDSOCIETE { get; set; }

        [StringLength(50)]
        public string SITE { get; set; }

        public bool? NOTIF { get; set; }

        [StringLength(255)]
        public string LIEN { get; set; }

        public string OBJET { get; set; }

        public string TITLE { get; set; }

        public string DOC { get; set; }

        public string MESSAGE { get; set; }

        public int? NUMREG { get; set; }
    }
}
