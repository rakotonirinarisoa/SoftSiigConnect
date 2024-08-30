namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DocumentTypeUnion")]
    public partial class DocumentTypeUnion
    {
        public int Id { get; set; }

        public Guid? DocumentID { get; set; }

        public Guid? TypeDocID { get; set; }
    }
}
