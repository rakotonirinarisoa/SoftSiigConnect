namespace apptab.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OPA_DONNEURORDRE
    {
        public int ID { get; set; }

        [StringLength(10)]
        public string CODE_J { get; set; }

        public DateTime? DATE_PAIEMENT { get; set; }

        [StringLength(24)]
        public string DONNEUR_ORDRE { get; set; }

        [StringLength(5)]
        public string CODE_GUICHET { get; set; }

        [StringLength(11)]
        public string NUM_COMPTE { get; set; }

        [StringLength(5)]
        public string CODE_BANQUE { get; set; }

        [StringLength(2)]
        public string CLE { get; set; }

        [StringLength(100)]
        public string BASE { get; set; }

        public int? IDSOCIETE { get; set; }

        [StringLength(100)]
        public string APPLICATION { get; set; }

        [StringLength(50)]
        public string NIF { get; set; }

        [StringLength(10)]
        public string MONNAIELOCAL { get; set; }

        [StringLength(50)]
        public string VILLE { get; set; }

        [StringLength(50)]
        public string ADDRESSE1 { get; set; }

        [StringLength(50)]
        public string ADDRESSE2 { get; set; }

        [StringLength(10)]
        public string PAYS { get; set; }
    }
}
