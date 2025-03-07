namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VerificationTokensHistory")]
    public partial class VerificationTokensHistory
    {
        public long Id { get; set; }

        [Required]
        [StringLength(142)]
        public string Content { get; set; }

        public Guid SignatureId { get; set; }

        public Guid VerificationTokenId { get; set; }

        public DateTime CreationDate { get; set; }

        public virtual UserSignatures UserSignatures { get; set; }

        public virtual VerificationTokens VerificationTokens { get; set; }
    }
}
