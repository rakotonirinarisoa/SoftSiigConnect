namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RCATEGORIE")]
    public partial class RCATEGORIE
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string CONVENTION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(3)]
        public string CODE { get; set; }

        [StringLength(10)]
        public string CODEBIS { get; set; }

        [StringLength(100)]
        public string LIBELLELONG { get; set; }

        [StringLength(60)]
        public string LIBELLE { get; set; }

        [StringLength(100)]
        public string LIBELLELONGM { get; set; }

        [StringLength(60)]
        public string LIBELLEM { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TAXE { get; set; }

        [StringLength(50)]
        public string MODELE { get; set; }

        [StringLength(10)]
        public string PLANCONSOLIDE { get; set; }

        [StringLength(3)]
        public string MONNAIEDRF { get; set; }

        [StringLength(3)]
        public string MONNAIESUIVI1 { get; set; }

        [StringLength(3)]
        public string MONNAIESUIVI2 { get; set; }

        public bool STATUT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? MONTSEUILLOC { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? MONTSEUILDEV { get; set; }

        public DateTime? DATECRE { get; set; }

        public DateTime? DATEMAJ { get; set; }

        [StringLength(10)]
        public string USERCRE { get; set; }

        [StringLength(10)]
        public string USERMAJ { get; set; }

        [StringLength(10)]
        public string CODECAA { get; set; }
    }
}
