namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CPTADMIN_MODELEETATS_AVANCE
    {
        [Key]
        [StringLength(20)]
        public string NUMERO { get; set; }

        [StringLength(70)]
        public string LIBELLE { get; set; }

        [StringLength(70)]
        public string MODELE { get; set; }

        [StringLength(10)]
        public string EXTENSION { get; set; }

        public DateTime? DATECRE { get; set; }

        public DateTime? DATEMAJ { get; set; }

        [StringLength(10)]
        public string USERCRE { get; set; }

        [StringLength(10)]
        public string USERMAJ { get; set; }
    }
}
