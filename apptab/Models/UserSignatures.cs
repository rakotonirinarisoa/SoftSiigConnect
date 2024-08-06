namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserSignatures
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public UserSignatures()
        //{
        //    VerificationTokensHistory = new HashSet<VerificationTokensHistory>();
        //}

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? DeletionDate { get; set; }

        public virtual Users Users { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VerificationTokensHistory> VerificationTokensHistory { get; set; }
    }
}
