namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CPTADMIN_COMMENTAIRE_AVANCE
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string NUMEROAVANCE { get; set; }

        [StringLength(20)]
        public string CODESITE { get; set; }

        [StringLength(255)]
        public string COMMENTAIRE { get; set; }

        [StringLength(20)]
        public string ETAPEMVT { get; set; }

        public DateTime? DATECRE { get; set; }

        public DateTime? DATEMAJ { get; set; }

        [StringLength(10)]
        public string USERCRE { get; set; }

        [StringLength(10)]
        public string USERMAJ { get; set; }
    }
}
