namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DocumentTypesUsersSteps
    {
        public long Id { get; set; }

        public Guid UserId { get; set; }

        public Guid StepId { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? DeletionDate { get; set; }

        public Guid? DeletedBy { get; set; }

        public virtual DocumentTypesSteps DocumentTypesSteps { get; set; }

        public virtual Users Users { get; set; }

        public virtual Users Users1 { get; set; }

        public virtual Users Users2 { get; set; }
    }
}
