namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SI_MAIL
    {
        public int ID { get; set; }

        public int? IDPROJET { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DELETIONDATE { get; set; }

        public string MAILTE { get; set; }

        public string MAILTV { get; set; }

        public string MAILPI { get; set; }

        public string MAILPE { get; set; }

        public string MAILPV { get; set; }

        public string MAILPP { get; set; }

        public string MAILPB { get; set; }

        public int? IDUSER { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? CREATIONDATE { get; set; }
    }
}
