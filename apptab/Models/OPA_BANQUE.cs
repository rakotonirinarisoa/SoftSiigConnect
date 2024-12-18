namespace apptab.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OPA_BANQUE
    {
        public int ID { get; set; }

        public string NOM_BANQUE { get; set; }

        [StringLength(11)]
        public string RIB_BANQUE { get; set; }

        [StringLength(6)]
        public string GUICHET { get; set; }

        [StringLength(2)]
        public string CLE { get; set; }

        [StringLength(6)]
        public string AGENCE { get; set; }

        public string REGION { get; set; }
    }
}
