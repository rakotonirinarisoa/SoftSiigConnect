namespace apptab.Models
{
    using System.ComponentModel.DataAnnotations;

    public partial class OPA_ANOMALIEBR
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string NUM { get; set; }

        public int? IDSOCIETE { get; set; }
    }
}
