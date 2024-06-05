namespace apptab
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class SI_TYPEPROCESSUS
    {
        public int ID { get; set; }

        public int? VALDEPENSES { get; set; }

        public int? VALPAIEMENTS { get; set; }

        public int? IDPROJET { get; set; }

        public int? IDUSER { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DELETIONDATE { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? CREATIONDATE { get; set; }

        public int? PAD { get; set; }

        public int? PCOP { get; set; }
    }
}
