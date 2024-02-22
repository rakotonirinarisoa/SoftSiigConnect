namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SI_MENU
    {
        public int ID { get; set; }

        public string MT0 { get; set; }
        public string MT1 { get; set; }
        public string MT2 { get; set; }
        public string MP1 { get; set; }
        public string MP2 { get; set; }
        public string MP3 { get; set; }
        public string MP4 { get; set; }
        public string MD0 { get; set; }
        public string MD1 { get; set; }
        public string MD2 { get; set; }
        public string MOP0 { get; set; }
        public string MOP1 { get; set; }
        public string MOP2 { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? CREATIONDATE { get; set; }
    }
}
