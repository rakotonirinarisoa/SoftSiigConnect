namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UsersProjectsSites
    {
        public int Id { get; set; }

        public Guid? UserId { get; set; }

        public string ProjectId { get; set; }

        public string Site { get; set; }
    }
}
