namespace apptab.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CPTADMIN_FAUTREOPERATION
    {
        public Guid ID { get; set; }

        [Required]
        [StringLength(2)]
        public string SITE { get; set; }

        [StringLength(50)]
        public string NUMEROOPERATION { get; set; }

        [StringLength(20)]
        public string TYPEOPERATION { get; set; }

        [StringLength(50)]
        public string CODEMARCHE { get; set; }

        [StringLength(255)]
        public string DESCRIPTION { get; set; }

        public DateTime? DATEOPERATION { get; set; }

        [StringLength(3)]
        public string DEVISE { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? COURSDEVISE { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? COURSRAPPORT { get; set; }

        public DateTime? DATESERVICEFAIT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? MONTANTLOCAL { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? MONTANTRAPPORT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? MONTANTDEVISE { get; set; }

        [StringLength(10)]
        public string JOURNALPAIEMENT { get; set; }

        [StringLength(10)]
        public string JOURNALRECEPTION { get; set; }

        [StringLength(50)]
        public string NORDPAIEMENT { get; set; }

        [StringLength(50)]
        public string NORDRECEPTION { get; set; }

        [StringLength(250)]
        public string ACTIVITE { get; set; }

        [StringLength(250)]
        public string FINANCEMENT { get; set; }

        [StringLength(250)]
        public string CATEGORIE { get; set; }

        [StringLength(250)]
        public string NUMEROENGAGEMENT { get; set; }

        [StringLength(20)]
        public string TYPEENGAGEMENT { get; set; }

        [StringLength(20)]
        public string TYPEPROCEDURE { get; set; }

        [StringLength(250)]
        public string CDMT1 { get; set; }

        [StringLength(250)]
        public string CDMT2 { get; set; }

        [StringLength(250)]
        public string NUMEROBE { get; set; }

        [StringLength(250)]
        public string NUMEROMANDAT { get; set; }

        public DateTime? DATETAPE1 { get; set; }

        [StringLength(50)]
        public string ETAPE1USER { get; set; }

        public DateTime? DATETAPE2 { get; set; }

        [StringLength(50)]
        public string ETAPE2USER { get; set; }

        public DateTime? DATETAPE3 { get; set; }

        [StringLength(50)]
        public string ETAPE3USER { get; set; }

        public DateTime? DATETAPE4 { get; set; }

        [StringLength(50)]
        public string ETAPE4USER { get; set; }

        public DateTime? DATETAPE5 { get; set; }

        [StringLength(50)]
        public string ETAPE5USER { get; set; }

        public DateTime? DATETAPE6 { get; set; }

        [StringLength(50)]
        public string ETAPE6USER { get; set; }

        public DateTime? DATETAPE7 { get; set; }

        [StringLength(50)]
        public string ETAPE7USER { get; set; }

        public DateTime? DATETAPE8 { get; set; }

        [StringLength(50)]
        public string ETAPE8USER { get; set; }

        public DateTime? DATETAPE9 { get; set; }

        [StringLength(50)]
        public string ETAPE9USER { get; set; }

        public DateTime? DATETAPE10 { get; set; }

        [StringLength(50)]
        public string ETAPE10USER { get; set; }

        [StringLength(10)]
        public string USERCRE { get; set; }

        public DateTime? DATECRE { get; set; }

        [StringLength(10)]
        public string USERMAJ { get; set; }

        public DateTime? DATEMAJ { get; set; }
    }
}
