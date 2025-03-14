namespace apptab.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RUSER")]
    public partial class RUSER
    {
        [Key]
        [StringLength(10)]
        public string LOGIN { get; set; }

        [StringLength(30)]
        public string PASSWORD { get; set; }

        [StringLength(255)]
        public string GROUPE { get; set; }

        [StringLength(30)]
        public string NOM { get; set; }

        [StringLength(30)]
        public string PRENOMS { get; set; }

        [Column(TypeName = "image")]
        public byte[] PHOTO { get; set; }

        [Column(TypeName = "text")]
        public string SITE { get; set; }

        [StringLength(5)]
        public string LANGUE { get; set; }

        [Column(TypeName = "text")]
        public string JOURNAL { get; set; }

        public bool? ACTIF { get; set; }

        public DateTime? DATECRE { get; set; }

        public DateTime? DATEMAJ { get; set; }

        [StringLength(10)]
        public string USERCRE { get; set; }

        [StringLength(10)]
        public string USERMAJ { get; set; }

        [Column(TypeName = "text")]
        public string JOURNALBROUILLARD { get; set; }

        public bool? SITETOUS { get; set; }

        public bool? JLTOUS { get; set; }

        [StringLength(50)]
        public string MAIL { get; set; }

        public bool? ACTIVATED { get; set; }

        public DateTime? VALIDITYDATE { get; set; }

        public DateTime? LASTPASSWORDUPDATE { get; set; }

        [StringLength(255)]
        public string TSKSITE { get; set; }

        public bool? TSKSITETOUS { get; set; }

        [StringLength(255)]
        public string TMASITE { get; set; }

        public bool? TMASITETOUS { get; set; }

        [StringLength(255)]
        public string ETABGRH { get; set; }

        public bool? ETABGRHETOUS { get; set; }

        [StringLength(255)]
        public string ETABPAIE { get; set; }

        public bool? ETABPAIETOUS { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? SEUILMINNUMENREG { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? SEUILMINNUMENREGBUDGET { get; set; }

        public bool? ISLITE { get; set; }

        [StringLength(255)]
        public string CONVENTION { get; set; }

        public bool? CONVENTIONTOUS { get; set; }

        [StringLength(50)]
        public string DEFAULTMENU { get; set; }

        [StringLength(255)]
        public string THEME { get; set; }

        public string TMTPLANANALYTIQUE { get; set; }

        public bool? TMTPLANANALYTIQUETOUS { get; set; }

        public string TMTSITE { get; set; }

        public bool? TMTSITETOUS { get; set; }
    }
}
