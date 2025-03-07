namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Soas
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime? DeletionDate { get; set; }

        public DateTime? CreationDate { get; set; }
    }
}
