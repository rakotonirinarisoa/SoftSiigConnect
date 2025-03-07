namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RPLAN6
    {
        [Key]
        [StringLength(20)]
        public string CODE { get; set; }

        [StringLength(10)]
        public string CODEBIS { get; set; }

        [StringLength(100)]
        public string LIBELLELONG { get; set; }

        [StringLength(60)]
        public string LIBELLE { get; set; }

        [StringLength(100)]
        public string LIBELLELONGM { get; set; }

        [StringLength(30)]
        public string LIBELLEM { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? NIVEAU { get; set; }

        [StringLength(10)]
        public string PLANCONSOLIDE { get; set; }

        public bool STATUT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? PU { get; set; }

        [StringLength(10)]
        public string UNITE { get; set; }

        public bool? SUIVIQTE { get; set; }

        public DateTime? DATECRE { get; set; }

        public DateTime? DATEMAJ { get; set; }

        [StringLength(10)]
        public string USERCRE { get; set; }

        [StringLength(10)]
        public string USERMAJ { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? IMPORTID { get; set; }
    }
}
