namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RCONVENTION")]
    public partial class RCONVENTION
    {
        [Key]
        [StringLength(10)]
        public string CODE { get; set; }

        [StringLength(60)]
        public string LIBELLE { get; set; }

        [StringLength(50)]
        public string MONDRF { get; set; }

        [StringLength(50)]
        public string MONSUIVI1 { get; set; }

        [StringLength(50)]
        public string MONSUIVI2 { get; set; }

        [StringLength(4000)]
        public string COMMENT1 { get; set; }

        [StringLength(255)]
        public string COMMENT2 { get; set; }

        [StringLength(255)]
        public string COMMENT3 { get; set; }

        [StringLength(255)]
        public string COMMENT4 { get; set; }

        public DateTime? DATEDEBTRIDRF { get; set; }

        public DateTime? DATEFINTRIDRF { get; set; }

        [StringLength(50)]
        public string ETATDRF { get; set; }

        public double? NUMDRF { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TAUXDRF { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TAUXSUIVI1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TAUXSUIVI2 { get; set; }

        public DateTime? DATEDEBTRIDPD { get; set; }

        public DateTime? DATEFINTRIDPD { get; set; }

        [StringLength(50)]
        public string ETATDPD { get; set; }

        public double? NUMDPD { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TAUXDPD { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TAUXSUIVI1DPD { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TAUXSUIVI2DPD { get; set; }

        public DateTime? DATEDEBEDITION { get; set; }

        public DateTime? DATEFINEDITION { get; set; }

        public DateTime? DATELIMITEDEM { get; set; }

        public DateTime? DATEFINEXEC { get; set; }

        [StringLength(50)]
        public string NUMEROPROJET { get; set; }

        [StringLength(50)]
        public string NATURECOMPTA { get; set; }

        [StringLength(50)]
        public string SITE { get; set; }

        [StringLength(50)]
        public string SECTEUR { get; set; }

        [StringLength(10)]
        public string BAILLEUR { get; set; }

        [StringLength(10)]
        public string JL { get; set; }

        public DateTime? DATECRE { get; set; }

        public DateTime? DATEMAJ { get; set; }

        [StringLength(10)]
        public string USERCRE { get; set; }

        [StringLength(10)]
        public string USERMAJ { get; set; }

        [StringLength(10)]
        public string PLANCONSOLIDE { get; set; }

        [StringLength(10)]
        public string SIGNATAIRE { get; set; }

        [StringLength(50)]
        public string SUIVIPAR { get; set; }

        public DateTime? DATEDECISION { get; set; }

        public DateTime? DATEOUVERTURE { get; set; }

        public DateTime? DATECLOTURE { get; set; }

        public bool? STATUT { get; set; }

        public double? NUMCAA { get; set; }

        public DateTime? DATEDEBTRICAA { get; set; }

        public DateTime? DATEFINTRICAA { get; set; }

        [StringLength(50)]
        public string ETATCAA { get; set; }

        public DateTime? DATEDEBEDITIONCAA { get; set; }

        public DateTime? DATEFINEDITIONCAA { get; set; }

        public int? LASTNUMDEMANDECAA { get; set; }

        [StringLength(10)]
        public string CODECAA { get; set; }

        public bool? RPA { get; set; }

        public DateTime? DATEDEBUT { get; set; }

        public DateTime? DATEFIN { get; set; }
    }
}
