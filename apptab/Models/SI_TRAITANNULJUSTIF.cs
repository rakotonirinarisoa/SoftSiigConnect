namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SI_TRAITANNULJUSTIF
    {
        public int ID { get; set; }

        public Guid? No { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DATEANNUL { get; set; }

        [StringLength(50)]
        public string MOTIF { get; set; }

        public string COMMENTAIRE { get; set; }

        public int? IDPROJET { get; set; }

        public int? IDUSER { get; set; }
    }
}
