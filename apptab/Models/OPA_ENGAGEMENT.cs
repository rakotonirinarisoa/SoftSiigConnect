namespace apptab.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OPA_ENGAGEMENT
    {
        public int ID { get; set; }

        [Required]
        public string NUMEROOP { get; set; }

        public int NUMEROREG { get; set; }

        public decimal MONTANTVAL { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime DATEVALRAF { get; set; }

        [Required]
        public string USERVAL { get; set; }

        public int PROJECTID { get; set; }
    }
}
