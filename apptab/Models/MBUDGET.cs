namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MBUDGET")]
    public partial class MBUDGET
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(4)]
        public string ANNEE { get; set; }

        [Key]
        [Column(Order = 1, TypeName = "numeric")]
        public decimal NUMBUD { get; set; }

        [Key]
        [Column(Order = 2, TypeName = "numeric")]
        public decimal NUMENREG { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(2)]
        public string SITE { get; set; }

        [StringLength(20)]
        public string COGE { get; set; }

        [StringLength(20)]
        public string POSTE { get; set; }

        [StringLength(20)]
        public string GEO { get; set; }

        [StringLength(20)]
        public string ACTI { get; set; }

        [StringLength(10)]
        public string CONVENTION { get; set; }

        [StringLength(3)]
        public string CATEGORIE { get; set; }

        //[StringLength(3)]
        //public string SOUSCATEGORIE { get; set; }

        [StringLength(20)]
        public string PLAN6 { get; set; }

        //[StringLength(10)]
        //public string UO { get; set; }

        //[Column(TypeName = "numeric")]
        //public decimal? QUO { get; set; }

        //[Column(TypeName = "numeric")]
        //public decimal? PUO { get; set; }

        //[StringLength(100)]
        //public string LIBELLE { get; set; }

        //[Column(TypeName = "numeric")]
        //public decimal? REPARTITION { get; set; }

        //[Column(TypeName = "numeric")]
        //public decimal? MONTBUDGET { get; set; }

        //[StringLength(1)]
        //public string TYPEBUDGET { get; set; }

        //[Column(TypeName = "numeric")]
        //public decimal? NBRREPF { get; set; }

        //[Column(TypeName = "numeric")]
        //public decimal? NBRREPD { get; set; }

        //[StringLength(100)]
        //public string LIBELLEM { get; set; }

        //[StringLength(15)]
        //public string CODEDP { get; set; }

        //public bool REGIE { get; set; }

        //public DateTime? DATECRE { get; set; }

        //public DateTime? DATEMAJ { get; set; }

        //[StringLength(10)]
        //public string USERCRE { get; set; }

        //[StringLength(10)]
        //public string USERMAJ { get; set; }

        //[StringLength(1)]
        //public string NATSOURCE { get; set; }

        //[StringLength(50)]
        //public string SIGLE { get; set; }

        //[StringLength(25)]
        //public string NUMLIGNEBUD { get; set; }

        //[StringLength(20)]
        //public string PLAN7 { get; set; }

        //[Column(TypeName = "numeric")]
        //public decimal? IMPORTID { get; set; }

        //[StringLength(20)]
        //public string PLAN8 { get; set; }

        //[StringLength(36)]
        //public string TABLEID { get; set; }

        public virtual RBUDGET RBUDGET { get; set; }

        public virtual RPOST1 RPOST1 { get; set; }
    }
}
