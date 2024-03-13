namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CPTADMIN_MAVANCEPJ
    {
        public Guid ID { get; set; }

        public Guid IDAVANCE { get; set; }

        [Required]
        [StringLength(36)]
        public string IDDOC { get; set; }

        [StringLength(255)]
        public string NOMDOC { get; set; }

        [StringLength(20)]
        public string FORMAT { get; set; }

        public DateTime? DATECRE { get; set; }

        public DateTime? DATEMAJ { get; set; }

        [StringLength(10)]
        public string USERCRE { get; set; }

        [StringLength(10)]
        public string USERMAJ { get; set; }

        public virtual CPTADMIN_FAVANCE CPTADMIN_FAVANCE { get; set; }
    }
}
