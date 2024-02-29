namespace apptab
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class HSI_MAIL
    {
        public int ID { get; set; }

        public int? IDPROJET { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DELETIONDATE { get; set; }

        public string MAILTE { get; set; }

        public string MAILTV { get; set; }

        public string MAILSIIG { get; set; }

        public string MAILPI { get; set; }

        public string MAILPE { get; set; }

        public string MAILPV { get; set; }

        public string MAILPP { get; set; }

        public string MAILPB { get; set; }

        public int? IDUSER { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? CREATIONDATE { get; set; }

        public int? IDPARENT { get; set; }
    }
}
