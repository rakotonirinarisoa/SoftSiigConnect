namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OPA_ANOMALIEBR
    {
        [Key]
        [StringLength(100)]
        public string NUM { get; set; }

        public int? IDSOCIETE { get; set; }
    }
}
