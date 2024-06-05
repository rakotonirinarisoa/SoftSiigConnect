namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MBUDALLOC")]
    public partial class MBUDALLOC
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
        public DateTime MOIS { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(2)]
        public string SITE { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? MONTANT { get; set; }

        public DateTime? DATECRE { get; set; }

        public DateTime? DATEMAJ { get; set; }

        [StringLength(10)]
        public string USERCRE { get; set; }

        [StringLength(10)]
        public string USERMAJ { get; set; }

        [StringLength(50)]
        public string SIGLE { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? IMPORTID { get; set; }

        [StringLength(36)]
        public string TABLEID { get; set; }
    }
}
