namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RSITE")]
    public partial class RSITE
    {
        [Key]
        [StringLength(2)]
        public string CODE { get; set; }

        [StringLength(50)]
        public string LIBELLE { get; set; }

        [StringLength(50)]
        public string LIBELLEM { get; set; }

        public bool ENCOURS { get; set; }

        [StringLength(20)]
        public string GEODEFAUT { get; set; }

        [StringLength(10)]
        public string ACTIDEFAUT { get; set; }

        public DateTime? DATECOPIE { get; set; }

        public DateTime? DATEINTEGRE { get; set; }

        [StringLength(1)]
        public string ETAT { get; set; }

        public DateTime? DATECRE { get; set; }

        public DateTime? DATEMAJ { get; set; }

        [StringLength(10)]
        public string USERCRE { get; set; }

        [StringLength(10)]
        public string USERMAJ { get; set; }

        [StringLength(2)]
        public string PARENT { get; set; }

        public bool? CONNECTED { get; set; }
    }
}
