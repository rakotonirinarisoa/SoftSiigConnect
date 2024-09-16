namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UsersSteps
    {
        public long Id { get; set; }

        public Guid UserId { get; set; }

        public Guid DocumentStepId { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? ProcessingDate { get; set; }

        public DateTime? DeletionDate { get; set; }
        public string Comment { get; set; }

        public bool? IsValidator { get; set; }

        public virtual DocumentSteps DocumentSteps { get; set; }

        public virtual Users Users { get; set; }
    }
}
