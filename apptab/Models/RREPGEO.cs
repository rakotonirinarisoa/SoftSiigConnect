namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RREPGEO")]
    public partial class RREPGEO
    {
        [Key]
        [StringLength(10)]
        public string CODE { get; set; }

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
    }
}
