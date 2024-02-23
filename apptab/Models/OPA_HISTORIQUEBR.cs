namespace apptab
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class OPA_HISTORIQUEBR
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [StringLength(100)]
        public string NUMENREG { get; set; }

        public DateTime? DATEAFB { get; set; }

        public int? IDUSER { get; set; }

        [StringLength(100)]
        public string AFB { get; set; }

        public int? IDSOCIETE { get; set; }
    }
}
