namespace apptab.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SI_TYPEBANQUE
    {
        public int ID { get; set; }

        public string TYPE { get; set; }

        public int? IDUSER { get; set; }

        public DateTime? CREATIONDATE { get; set; }

        public int? IDPROJET { get; set; }
    }
}
