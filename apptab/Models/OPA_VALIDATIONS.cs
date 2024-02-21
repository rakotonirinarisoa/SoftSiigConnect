namespace apptab.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OPA_VALIDATIONS
    {
        public int ID { get; set; }

        public decimal? IDREGLEMENT { get; set; }

        public int? ETAT { get; set; }

        public int? IDPROJET { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DateIn { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DateOut { get; set; }

        public string ComptaG { get; set; }

        [StringLength(50)]
        public string auxi { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DateP { get; set; }

        [StringLength(50)]
        public string Journal { get; set; }

        public string dateOrdre { get; set; }

        [StringLength(50)]
        public string NoPiece { get; set; }

        [StringLength(50)]
        public string Compte { get; set; }

        public string Libelle { get; set; }

        public decimal? Debit { get; set; }

        public decimal? Credit { get; set; }

        public decimal? MontantDevise { get; set; }

        [StringLength(50)]
        public string Mon { get; set; }

        [StringLength(50)]
        public string Rang { get; set; }

        [StringLength(50)]
        public string Poste { get; set; }

        public string FinancementCategorie { get; set; }

        public string Commune { get; set; }

        public string Plan6 { get; set; }

        [StringLength(50)]
        public string Marche { get; set; }

        [StringLength(50)]
        public string Statut { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DATECREA { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DATESEND { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DATEVAL { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DATEANNULER { get; set; }

        public string MOTIF { get; set; }

        public string COMS { get; set; }

        public int? IDUSER { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DATEACCEPT { get; set; }

        [StringLength(10)]
        public string Devise { get; set; }
    }
}
