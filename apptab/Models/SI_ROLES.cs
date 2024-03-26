namespace apptab
{
    using System.ComponentModel.DataAnnotations;

    public partial class SI_ROLES
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string INTITULES { get; set; }
      
    }
    public enum Role
    {
        SAdministrateur, Administrateur, Autre, Organe_de_Suivi, Validateur_paiements//, ORDSEC, Consultation, PRORDESEC
    }
}
