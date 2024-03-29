namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SI_USERSHISTO
    {
        public int ID { get; set; }

        public int? IDPROJET { get; set; }

        public int? IDUSER { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? CONNEX { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DISCONNEX { get; set; }
    }
}
