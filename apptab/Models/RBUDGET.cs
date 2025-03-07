namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RBUDGET")]
    public partial class RBUDGET
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RBUDGET()
        {
            MBUDGET = new HashSet<MBUDGET>();
        }

        [Key]
        [Column(TypeName = "numeric")]
        public decimal NUMBUD { get; set; }

        [StringLength(50)]
        public string LIBELLE { get; set; }

        [StringLength(50)]
        public string MONNAIE { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? COURS { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? NIVPOST { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? NIVACTI { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? NIVGEO { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? NIVCOGE { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? NIVFIN { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? NIVPLAN6 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? LIMITE { get; set; }

        public DateTime? DATECRE { get; set; }

        public DateTime? DATEMAJ { get; set; }

        [StringLength(10)]
        public string USERCRE { get; set; }

        [StringLength(10)]
        public string USERMAJ { get; set; }

        public bool? DISPPOST { get; set; }

        public bool? DISPACTI { get; set; }

        public bool? DISPGEO { get; set; }

        public bool? DISPCOGE { get; set; }

        public bool? DISPFIN { get; set; }

        public bool? DISPPLAN6 { get; set; }

        public int? NIVPLAN7 { get; set; }

        [StringLength(1)]
        public string TYPEBUDGET { get; set; }

        public int? NIVPLAN8 { get; set; }

        public bool? DISPPLAN7 { get; set; }

        public bool? DISPPLAN8 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MBUDGET> MBUDGET { get; set; }
    }
}
