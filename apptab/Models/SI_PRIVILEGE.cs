namespace apptab
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class SI_PRIVILEGE
    {
        public int ID { get; set; }
        public int? IDUSERPRIV { get; set; }
        public int? MENUPAR1 { get; set; }
        public int? MENUPAR2 { get; set; }
        public int? MENUPAR3 { get; set; }
        public int? MENUPAR4 { get; set; }
        public int? MENUPAR5 { get; set; }
        public int? MENUPAR6 { get; set; }
        public int? MENUPAR7 { get; set; }
        public int? MENUPAR8 { get; set; }
        public int? MENUPAR9 { get; set; }
        public int? MENUPAR10 { get; set; }
        public int? MTNON { get; set; }
        public int? MT0 { get; set; }
        public int? MT1 { get; set; }
        public int? MT2 { get; set; }
        public int? MP1 { get; set; }
        public int? MP2 { get; set; }
        public int? MP3 { get; set; }
        public int? MP4 { get; set; }
        public int? GED { get; set; }
        public int? MD0 { get; set; }
        public int? MD1 { get; set; }
        public int? MD2 { get; set; }
        public int? MD3 { get; set; }
        public int? MOP0 { get; set; }
        public int? MOP1 { get; set; }
        public int? MOP2 { get; set; }

        public int? TDB0 { get; set; }
        public int? TDB1 { get; set; }
        public int? TDB2 { get; set; }
        public int? TDB3 { get; set; }
        public int? TDB4 { get; set; }
        public int? TDB5 { get; set; }
        public int? TDB6 { get; set; }
        public int? TDB7 { get; set; }
        public int? TDB8 { get; set; }
        public int? J0 { get; set; }
        public int? J1 { get; set; }
        public int? J2 { get; set; }
        public int? J3 { get; set; }
        public int? JR { get; set; }
        public int? JRA { get; set; }
        public int? RSF { get; set; }
        public int? RSFT { get; set; }
        public int? TDB9 { get; set; }
        public int? IDUSER { get; set; }

        public int? TDB11 { get; set; }
        public int? TDB12 { get; set; }
        public int? TDB13 { get; set; }
        public int? TDB14 { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? CREATIONDATE { get; set; }
    }
}
