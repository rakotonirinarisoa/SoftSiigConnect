namespace apptab
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class GA_AVANCE_JUSTIFICATIF
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(36)]
        public string ID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(2)]
        public string SITE { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string NUMERO_AVANCE { get; set; }

        [StringLength(36)]
        public string NUMERO_AVANCE_MOUVEMENT { get; set; }

        public DateTime? DATE { get; set; }

        [StringLength(20)]
        public string NUMERO_PIECE { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? MONTANT { get; set; }

        [StringLength(250)]
        public string COMMENTAIRE { get; set; }

        [StringLength(14)]
        public string NORD { get; set; }

        public DateTime? DATE_COMPTABILISATION { get; set; }
    }
}
