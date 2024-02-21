namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OPA_FTP
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string HOTE { get; set; }

        [StringLength(50)]
        public string IDENTIFIANT { get; set; }

        [StringLength(50)]
        public string FTPPWD { get; set; }

        public string PATH { get; set; }

        public int? PORT { get; set; }

        public int? IDPROJET { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DELETIONDATE { get; set; }

        public int? IDUSER { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? CREATIONDATE { get; set; }
    }
}
