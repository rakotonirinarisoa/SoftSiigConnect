namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class VerificationTokens
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VerificationTokens()
        {
            VerificationTokensHistory = new HashSet<VerificationTokensHistory>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(142)]
        public string Content { get; set; }

        public DateTime CreationDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VerificationTokensHistory> VerificationTokensHistory { get; set; }
    }
}
