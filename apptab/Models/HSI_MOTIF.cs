﻿namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class HSI_MOTIF
    {
        public int ID { get; set; }

        public string MOTIFTRAIT { get; set; }

        public string MOTIFPAI { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DELETIONDATE { get; set; }

        public int? IDUSER { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? CREATIONDATE { get; set; }

        public int? IDPARENT { get; set; }
    }
}
