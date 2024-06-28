namespace apptab.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RPAYS
    {
        [Key]
        [StringLength(3)]
        public string CODE { get; set; }

        [StringLength(50)]
        public string LIBELLE { get; set; }

        [StringLength(50)]
        public string LIBELLEM { get; set; }

        public DateTime? DATECRE { get; set; }

        public DateTime? DATEMAJ { get; set; }

        [StringLength(10)]
        public string USERCRE { get; set; }

        [StringLength(10)]
        public string USERMAJ { get; set; }

        [Column(TypeName = "image")]
        public byte[] DRAPEAU { get; set; }
    }
}
