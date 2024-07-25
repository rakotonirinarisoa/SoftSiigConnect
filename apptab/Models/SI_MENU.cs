namespace apptab
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class SI_MENU
    {
        public int ID { get; set; }

        public string MTNON { get; set; }
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
        public string MD3 { get; set; }
        public string MOP0 { get; set; }
        public string MOP1 { get; set; }
        public string MOP2 { get; set; }
        public string TDB0 { get; set; }
        public string TDB1 { get; set; }
        public string TDB2 { get; set; }
        public string TDB3 { get; set; }
        public string TDB4 { get; set; }
        public string TDB5 { get; set; }
        public string TDB6 { get; set; }
        public string TDB7 { get; set; }
        public string TDB8 { get; set; }
        public string J0 { get; set; }
        public string J1 { get; set; }
        public string J2 { get; set; }
        public string J3 { get; set; }
        public string JR { get; set; }
        public string JRA { get; set; }
        public string RSF { get; set; }
        public string RSFT { get; set; }
        public string TDB9 { get; set; }
        public string TDB11 { get; set; }
        public string TDB12 { get; set; }
        public string TDB13 { get; set; }
        public bool? TDB9i { get; set; }
        public bool? TDB0i { get; set; }
        public bool? JRi { get; set; }
        public bool? JRAi { get; set; }
        public bool? TDB1i { get; set; }
        public bool? TDB2i { get; set; }
        public bool? TDB3i { get; set; }
        public bool? TDB4i { get; set; }
        public bool? TDB5i { get; set; }
        public bool? TDB6i { get; set; }
        public bool? TDB7i { get; set; }
        public bool? TDB8i { get; set; }
        public bool? TDB11i { get; set; }
        public bool? TDB12i { get; set; }
        public bool? TDB13i { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? CREATIONDATE { get; set; }
    }
}
