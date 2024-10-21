namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class GA_AVANCE_COMPLEMENT
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NUMERO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string NUMERO_AVANCE { get; set; }

        public DateTime? DEBUT_ECHEANCE { get; set; }

        public DateTime ECHEANCE { get; set; }

        [StringLength(255)]
        public string COMMENTAIRE { get; set; }
    }
}
