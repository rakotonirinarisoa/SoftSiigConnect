namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DigitalSignatures
    {
        [StringLength(41)]
        public string Id { get; set; }

        [Required]
        [StringLength(800)]
        public string EncryptedSignature { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
