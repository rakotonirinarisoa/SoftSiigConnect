namespace apptab.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SI_SITE
    {
        public int ID { get; set; }

        public int? IDPROJET { get; set; }

        public int? IDUSER { get; set; }

        public string SITE { get; set; }
    }
}
