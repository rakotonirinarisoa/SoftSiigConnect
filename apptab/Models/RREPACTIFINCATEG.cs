namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RREPACTIFINCATEG")]
    public partial class RREPACTIFINCATEG
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string ACTI { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CONVENTION { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(3)]
        public string CATEGORIE { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? MONTREP1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? MONTREP2 { get; set; }

        public DateTime? DATECRE { get; set; }

        public DateTime? DATEMAJ { get; set; }

        [StringLength(10)]
        public string USERCRE { get; set; }

        [StringLength(10)]
        public string USERMAJ { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? MONTLOC1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? MONTLOC2 { get; set; }
    }
}
