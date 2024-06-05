namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RPOST1
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RPOST1()
        {
            MBUDGET = new HashSet<MBUDGET>();
        }

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

        [StringLength(60)]
        public string LIBELLEM { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? NIVEAU { get; set; }

        [StringLength(10)]
        public string PLANCONSOLIDE { get; set; }

        [Column(TypeName = "numeric")]
        public decimal STATUT { get; set; }

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MBUDGET> MBUDGET { get; set; }
    }
}
