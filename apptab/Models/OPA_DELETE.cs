namespace apptab.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OPA_DELETE
    {
        public int ID { get; set; }

        [Required]
        public string NUMEROOP { get; set; }

        public int NUMEREG { get; set; }

        [Required]
        public string USERMAIL { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime DATEDELETE { get; set; }

        public int? PROJETID { get; set; }
    }
}
