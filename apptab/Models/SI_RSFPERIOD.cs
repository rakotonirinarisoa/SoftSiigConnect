namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SI_RSFPERIOD
    {
        public int ID { get; set; }

        public string PERIODE { get; set; }
    }
}
