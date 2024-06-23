namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SI_RSF
    {
        public int ID { get; set; }

        public int? IDUSER { get; set; }

        public int? IDPROJET { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? CREATIONDATE { get; set; }

        public string TITLE { get; set; }
        public string MOIS { get; set; }

        public int? ANNEE { get; set; }

        public string PERIODE { get; set; }

        public string TYPE { get; set; }

        public string LIEN { get; set; }
        public string TITLEDOCS { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DELETIONDATE { get; set; }
    }
}
