using System;

namespace apptab.Data.Entities
{
    public class TraitementEngagementDetails
    {
        public string BENEFICIAIRE { get; set; }
        public string MONTENGAGEMENT { get; set; }
        public DateTime? DATENGAGEMENT { get; set; }
        public string MONTPAIE { get; set; }
        public DateTime? DATEPAIE { get; set; }
        public DateTime? DATETRANSFERTRAF { get; set; }
        public DateTime? DATEVALORDSEC { get; set; }
        public DateTime? DATESENDSIIG { get; set; }
        public DateTime? DATESIIGFP { get; set; }
    }
}
