namespace apptab.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ANOMALIE_G
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string COMPTE_BANQUE { get; set; }

        [StringLength(50)]
        public string RIB { get; set; }

        [StringLength(50)]
        public string COMPTEG { get; set; }

        public string AUXI { get; set; }

        public string AD1 { get; set; }

        public string AD2 { get; set; }

        public string SITE { get; set; }

        public string DOM1 { get; set; }

        public int? IDPROJECT { get; set; }
    }
}
