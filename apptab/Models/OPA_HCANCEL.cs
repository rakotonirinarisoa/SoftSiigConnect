namespace apptab.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OPA_HCANCEL
    {
        public int ID { get; set; }

        public int? IDUSER { get; set; }

        public int? IDREGLEMENT { get; set; }

        public DateTime? DATECANCEL { get; set; }

        public string MOTIF { get; set; }

        public string COMS { get; set; }

        public int? IDPROJETS { get; set; }

        public int? ETAT { get; set; }
    }
}
