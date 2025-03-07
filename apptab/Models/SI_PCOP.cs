namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SI_PCOP
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string PCOP { get; set; }
    }
}
