namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OPA_DROITS
    {
        public int ID { get; set; }

        public int? IDMAPPAGE { get; set; }

        public int? IDUSER { get; set; }

        public int? IDSOCIETE { get; set; }
    }
}
