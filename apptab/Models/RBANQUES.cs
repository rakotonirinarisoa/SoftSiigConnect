namespace apptab.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RBANQUES
    {
        [Key]
        [StringLength(10)]
        public string CODE { get; set; }

        [StringLength(50)]
        public string NOM { get; set; }

        public DateTime? DATECRE { get; set; }

        public DateTime? DATEMAJ { get; set; }

        [StringLength(10)]
        public string USERCRE { get; set; }

        [StringLength(10)]
        public string USERMAJ { get; set; }

        [StringLength(20)]
        public string CODEBIC { get; set; }

        [StringLength(20)]
        public string CODEBANQUE { get; set; }

        [StringLength(255)]
        public string DOMICILIATION { get; set; }

        [StringLength(100)]
        public string VILLE { get; set; }

        [StringLength(20)]
        public string PAYS { get; set; }
    }
}
