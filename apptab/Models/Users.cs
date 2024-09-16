namespace apptab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Users()
        {
            Attachements = new HashSet<Attachements>();
            Documents = new HashSet<Documents>();
            DocumentsReceptions = new HashSet<DocumentsReceptions>();
            DocumentTypes = new HashSet<DocumentTypes>();
            DocumentTypes1 = new HashSet<DocumentTypes>();
            DocumentTypesSteps = new HashSet<DocumentTypesSteps>();
            DocumentTypesSteps1 = new HashSet<DocumentTypesSteps>();
            DocumentTypesUsersSteps = new HashSet<DocumentTypesUsersSteps>();
            DocumentTypesUsersSteps1 = new HashSet<DocumentTypesUsersSteps>();
            DocumentTypesUsersSteps2 = new HashSet<DocumentTypesUsersSteps>();
            DynamicFieldItems = new HashSet<DynamicFieldItems>();
            DynamicFieldItems1 = new HashSet<DynamicFieldItems>();
            DynamicFields = new HashSet<DynamicFields>();
            DynamicFields1 = new HashSet<DynamicFields>();
            Suppliers = new HashSet<Suppliers>();
            SuppliersDocumentsAcknowledgements = new HashSet<SuppliersDocumentsAcknowledgements>();
            SuppliersDocumentsSendings = new HashSet<SuppliersDocumentsSendings>();
            TomProConnections = new HashSet<TomProConnections>();
            TomProConnections1 = new HashSet<TomProConnections>();
            TomProDatabases = new HashSet<TomProDatabases>();
            TomProDatabases1 = new HashSet<TomProDatabases>();
            Users1 = new HashSet<Users>();
            UsersConnections = new HashSet<UsersConnections>();
            UsersDocumentsAccesses = new HashSet<UsersDocumentsAccesses>();
            UsersDocumentsAccesses1 = new HashSet<UsersDocumentsAccesses>();
            UsersDocumentsAccesses2 = new HashSet<UsersDocumentsAccesses>();
            UserSignatures = new HashSet<UserSignatures>();
            UsersSteps = new HashSet<UsersSteps>();
            ValidationsHistory = new HashSet<ValidationsHistory>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        [StringLength(255)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255)]
        public string LastName { get; set; }

        public int RoleId { get; set; }

        public Guid? ProjectId { get; set; }

        public bool IsADocumentsReceiver { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? DeletionDate { get; set; }

        public Guid? DeletedBy { get; set; }

        public string Sites { get; set; }

        [StringLength(255)]
        public string Fonction { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Attachements> Attachements { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Documents> Documents { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentsReceptions> DocumentsReceptions { get; set; }

        public virtual DocumentsSenders DocumentsSenders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentTypes> DocumentTypes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentTypes> DocumentTypes1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentTypesSteps> DocumentTypesSteps { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentTypesSteps> DocumentTypesSteps1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentTypesUsersSteps> DocumentTypesUsersSteps { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentTypesUsersSteps> DocumentTypesUsersSteps1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentTypesUsersSteps> DocumentTypesUsersSteps2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DynamicFieldItems> DynamicFieldItems { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DynamicFieldItems> DynamicFieldItems1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DynamicFields> DynamicFields { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DynamicFields> DynamicFields1 { get; set; }

        public virtual Projects Projects { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Suppliers> Suppliers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SuppliersDocumentsAcknowledgements> SuppliersDocumentsAcknowledgements { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SuppliersDocumentsSendings> SuppliersDocumentsSendings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TomProConnections> TomProConnections { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TomProConnections> TomProConnections1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TomProDatabases> TomProDatabases { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TomProDatabases> TomProDatabases1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Users> Users1 { get; set; }

        public virtual Users Users2 { get; set; }

        public virtual UsersRoles UsersRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsersConnections> UsersConnections { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsersDocumentsAccesses> UsersDocumentsAccesses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsersDocumentsAccesses> UsersDocumentsAccesses1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsersDocumentsAccesses> UsersDocumentsAccesses2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserSignatures> UserSignatures { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsersSteps> UsersSteps { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ValidationsHistory> ValidationsHistory { get; set; }
    }
}
