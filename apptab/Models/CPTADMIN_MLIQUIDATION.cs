namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CPTADMIN_MLIQUIDATION
    {
        public Guid ID { get; set; }

        public Guid IDLIQUIDATION { get; set; }

        [Required]
        [StringLength(20)]
        public string COGE { get; set; }

        [StringLength(20)]
        public string AUXI { get; set; }

        [StringLength(255)]
        public string LIBELLE { get; set; }

        [StringLength(20)]
        public string ACTI { get; set; }

        [StringLength(20)]
        public string POSTE { get; set; }

        [StringLength(20)]
        public string GEO { get; set; }

        [StringLength(20)]
        public string PLAN6 { get; set; }

        [StringLength(20)]
        public string PLAN7 { get; set; }

        [StringLength(20)]
        public string PLAN8 { get; set; }

        [StringLength(20)]
        public string CONVENTION { get; set; }

        [StringLength(20)]
        public string CATEGORIE { get; set; }

        [StringLength(20)]
        public string SOUSCATEGORIE { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? MONTANTLOCAL { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? MONTANTRAPPORT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? MONTANTDEVISE { get; set; }

        [StringLength(10)]
        public string USERCRE { get; set; }

        public DateTime? DATECRE { get; set; }

        [StringLength(10)]
        public string USERMAJ { get; set; }

        public DateTime? DATEMAJ { get; set; }

        public virtual CPTADMIN_FLIQUIDATION CPTADMIN_FLIQUIDATION { get; set; }
    }
}
