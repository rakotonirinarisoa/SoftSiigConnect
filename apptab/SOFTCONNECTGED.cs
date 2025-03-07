using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace apptab
{
    public partial class SOFTCONNECTGED : DbContext
    {
        public SOFTCONNECTGED()
            : base(connex)
        {
        }

        public static string connex = "name=SOFTCONNECTGED";

        public virtual DbSet<Attachements> Attachements { get; set; }
        public virtual DbSet<DigitalSignatures> DigitalSignatures { get; set; }
        public virtual DbSet<DocumentDynamicFields> DocumentDynamicFields { get; set; }
        public virtual DbSet<Documents> Documents { get; set; }
        public virtual DbSet<DocumentsDynamicAttachements> DocumentsDynamicAttachements { get; set; }
        public virtual DbSet<DocumentsFields> DocumentsFields { get; set; }
        public virtual DbSet<DocumentsReceptions> DocumentsReceptions { get; set; }
        public virtual DbSet<DocumentsSenders> DocumentsSenders { get; set; }
        public virtual DbSet<DocumentSteps> DocumentSteps { get; set; }
        public virtual DbSet<DocumentTypes> DocumentTypes { get; set; }
        public virtual DbSet<DocumentTypesSteps> DocumentTypesSteps { get; set; }
        public virtual DbSet<DocumentTypesUsersSteps> DocumentTypesUsersSteps { get; set; }
        public virtual DbSet<DynamicFieldItems> DynamicFieldItems { get; set; }
        public virtual DbSet<DynamicFields> DynamicFields { get; set; }
        public virtual DbSet<DynamicFieldTypes> DynamicFieldTypes { get; set; }
        public virtual DbSet<FieldTypes> FieldTypes { get; set; }
        public virtual DbSet<Projects> Projects { get; set; }
        public virtual DbSet<Sites> Sites { get; set; }
        public virtual DbSet<Soas> Soas { get; set; }
        public virtual DbSet<Suppliers> Suppliers { get; set; }
        public virtual DbSet<SuppliersDocumentsAcknowledgements> SuppliersDocumentsAcknowledgements { get; set; }
        public virtual DbSet<SuppliersDocumentsSendings> SuppliersDocumentsSendings { get; set; }
        public virtual DbSet<SuppliersEmails> SuppliersEmails { get; set; }
        public virtual DbSet<TomProConnections> TomProConnections { get; set; }
        public virtual DbSet<TomProDatabases> TomProDatabases { get; set; }
        public virtual DbSet<UserDocumentRoles> UserDocumentRoles { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UsersConnections> UsersConnections { get; set; }
        public virtual DbSet<UsersDocumentsAccesses> UsersDocumentsAccesses { get; set; }
        public virtual DbSet<UserSignatures> UserSignatures { get; set; }
        public virtual DbSet<UsersProjectsSites> UsersProjectsSites { get; set; }
        public virtual DbSet<UsersRoles> UsersRoles { get; set; }
        public virtual DbSet<UsersSteps> UsersSteps { get; set; }
        public virtual DbSet<ValidationsHistory> ValidationsHistory { get; set; }
        public virtual DbSet<VerificationTokens> VerificationTokens { get; set; }
        public virtual DbSet<VerificationTokensHistory> VerificationTokensHistory { get; set; }
        public virtual DbSet<DocumentTypeUnion> DocumentTypeUnion { get; set; }

        public virtual DbSet<RedirectionsHistory> RedirectionsHistory { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DigitalSignatures>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<DigitalSignatures>()
                .Property(e => e.EncryptedSignature)
                .IsUnicode(false);

            modelBuilder.Entity<Documents>()
                .HasMany(e => e.Attachements)
                .WithRequired(e => e.Documents)
                .HasForeignKey(e => e.DocumentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Documents>()
                .HasMany(e => e.DocumentDynamicFields)
                .WithRequired(e => e.Documents)
                .HasForeignKey(e => e.DocumentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Documents>()
                .HasMany(e => e.DocumentsDynamicAttachements)
                .WithRequired(e => e.Documents)
                .HasForeignKey(e => e.DocumentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Documents>()
                .HasMany(e => e.DocumentsReceptions)
                .WithRequired(e => e.Documents)
                .HasForeignKey(e => e.DocumentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Documents>()
                .HasMany(e => e.DocumentSteps)
                .WithRequired(e => e.Documents)
                .HasForeignKey(e => e.DocumentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Documents>()
                .HasOptional(e => e.SuppliersDocumentsSendings)
                .WithRequired(e => e.Documents);

            modelBuilder.Entity<Documents>()
                .HasOptional(e => e.SuppliersDocumentsAcknowledgements)
                .WithRequired(e => e.Documents);

            modelBuilder.Entity<Documents>()
                .HasMany(e => e.UsersDocumentsAccesses)
                .WithRequired(e => e.Documents)
                .HasForeignKey(e => e.DocumentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Documents>()
                .HasMany(e => e.ValidationsHistory)
                .WithRequired(e => e.Documents)
                .HasForeignKey(e => e.DocumentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DocumentsSenders>()
                .HasMany(e => e.Documents)
                .WithRequired(e => e.DocumentsSenders)
                .HasForeignKey(e => e.SenderId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DocumentsSenders>()
                .HasOptional(e => e.Suppliers)
                .WithRequired(e => e.DocumentsSenders);

            modelBuilder.Entity<DocumentsSenders>()
                .HasOptional(e => e.Users)
                .WithRequired(e => e.DocumentsSenders);

            modelBuilder.Entity<DocumentSteps>()
                .HasMany(e => e.DocumentsFields)
                .WithRequired(e => e.DocumentSteps)
                .HasForeignKey(e => e.DocumentStepId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DocumentSteps>()
                .HasMany(e => e.UsersSteps)
                .WithRequired(e => e.DocumentSteps)
                .HasForeignKey(e => e.DocumentStepId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DocumentSteps>()
                .HasMany(e => e.ValidationsHistory)
                .WithOptional(e => e.DocumentSteps)
                .HasForeignKey(e => e.ToDocumentStepId);

            modelBuilder.Entity<DocumentTypes>()
                .HasMany(e => e.DocumentTypesSteps)
                .WithRequired(e => e.DocumentTypes)
                .HasForeignKey(e => e.DocumentTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DocumentTypesSteps>()
                .HasMany(e => e.DocumentTypesUsersSteps)
                .WithRequired(e => e.DocumentTypesSteps)
                .HasForeignKey(e => e.StepId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DynamicFields>()
                .HasMany(e => e.DocumentDynamicFields)
                .WithRequired(e => e.DynamicFields)
                .HasForeignKey(e => e.DynamicFieldId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DynamicFields>()
                .HasMany(e => e.DocumentsDynamicAttachements)
                .WithRequired(e => e.DynamicFields)
                .HasForeignKey(e => e.DynamicFieldId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DynamicFields>()
                .HasMany(e => e.DynamicFieldItems)
                .WithRequired(e => e.DynamicFields)
                .HasForeignKey(e => e.DynamicFieldId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DynamicFieldTypes>()
                .HasMany(e => e.DynamicFields)
                .WithRequired(e => e.DynamicFieldTypes)
                .HasForeignKey(e => e.Type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FieldTypes>()
                .HasMany(e => e.DocumentsFields)
                .WithRequired(e => e.FieldTypes)
                .HasForeignKey(e => e.FieldTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Projects>()
                .HasMany(e => e.DocumentTypes)
                .WithRequired(e => e.Projects)
                .HasForeignKey(e => e.ProjectId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Projects>()
                .HasMany(e => e.DynamicFields)
                .WithRequired(e => e.Projects)
                .HasForeignKey(e => e.ProjectId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Projects>()
                .HasMany(e => e.Suppliers)
                .WithRequired(e => e.Projects)
                .HasForeignKey(e => e.ProjectId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Projects>()
                .HasMany(e => e.TomProConnections)
                .WithRequired(e => e.Projects)
                .HasForeignKey(e => e.ProjectId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Projects>()
                .HasMany(e => e.Users)
                .WithOptional(e => e.Projects)
                .HasForeignKey(e => e.ProjectId);

            modelBuilder.Entity<Suppliers>()
                .Property(e => e.NIF)
                .IsUnicode(false);

            modelBuilder.Entity<Suppliers>()
                .Property(e => e.STAT)
                .IsUnicode(false);

            modelBuilder.Entity<Suppliers>()
                .Property(e => e.MAIL)
                .IsUnicode(false);

            modelBuilder.Entity<Suppliers>()
                .Property(e => e.CONTACT)
                .IsUnicode(false);

            modelBuilder.Entity<Suppliers>()
                .Property(e => e.CIN)
                .IsUnicode(false);

            modelBuilder.Entity<Suppliers>()
                .HasMany(e => e.SuppliersEmails)
                .WithRequired(e => e.Suppliers)
                .HasForeignKey(e => e.SupplierId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TomProConnections>()
                .Property(e => e.ServerName)
                .IsUnicode(false);

            modelBuilder.Entity<TomProConnections>()
                .Property(e => e.Login)
                .IsUnicode(false);

            modelBuilder.Entity<TomProConnections>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<TomProConnections>()
                .HasMany(e => e.TomProDatabases)
                .WithRequired(e => e.TomProConnections)
                .HasForeignKey(e => e.TomProConnectionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TomProDatabases>()
                .Property(e => e.DatabaseName)
                .IsUnicode(false);

            modelBuilder.Entity<UserDocumentRoles>()
                .HasMany(e => e.DocumentSteps)
                .WithRequired(e => e.UserDocumentRoles)
                .HasForeignKey(e => e.Role)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Attachements)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.DeletedBy);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Documents)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.DeletedBy);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.DocumentsReceptions)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.DocumentTypes)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.CreatedBy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.DocumentTypes1)
                .WithOptional(e => e.Users1)
                .HasForeignKey(e => e.DeletedBy);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.DocumentTypesSteps)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.CreatedBy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.DocumentTypesSteps1)
                .WithOptional(e => e.Users1)
                .HasForeignKey(e => e.DeletedBy);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.DocumentTypesUsersSteps)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.CreatedBy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.DocumentTypesUsersSteps1)
                .WithOptional(e => e.Users1)
                .HasForeignKey(e => e.DeletedBy);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.DocumentTypesUsersSteps2)
                .WithRequired(e => e.Users2)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.DynamicFieldItems)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.CreatedBy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.DynamicFieldItems1)
                .WithOptional(e => e.Users1)
                .HasForeignKey(e => e.DeletedBy);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.DynamicFields)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.CreatedBy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.DynamicFields1)
                .WithOptional(e => e.Users1)
                .HasForeignKey(e => e.DeletedBy);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Suppliers)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.DeletedBy);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.SuppliersDocumentsAcknowledgements)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.InitiatorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.SuppliersDocumentsSendings)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.InitiatorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.TomProConnections)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.CreatedBy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.TomProConnections1)
                .WithOptional(e => e.Users1)
                .HasForeignKey(e => e.DeletedBy);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.TomProDatabases)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.CreatedBy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.TomProDatabases1)
                .WithOptional(e => e.Users1)
                .HasForeignKey(e => e.DeletedBy);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Users1)
                .WithOptional(e => e.Users2)
                .HasForeignKey(e => e.DeletedBy);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.UsersConnections)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.UsersDocumentsAccesses)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.CreatedBy);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.UsersDocumentsAccesses1)
                .WithOptional(e => e.Users1)
                .HasForeignKey(e => e.DeletedBy);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.UsersDocumentsAccesses2)
                .WithRequired(e => e.Users2)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.UserSignatures)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.UsersSteps)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.ValidationsHistory)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.FromUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserSignatures>()
                .HasMany(e => e.VerificationTokensHistory)
                .WithRequired(e => e.UserSignatures)
                .HasForeignKey(e => e.SignatureId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UsersRoles>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.UsersRoles)
                .HasForeignKey(e => e.RoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<VerificationTokens>()
                .Property(e => e.Content)
                .IsUnicode(false);

            modelBuilder.Entity<VerificationTokens>()
                .HasMany(e => e.VerificationTokensHistory)
                .WithRequired(e => e.VerificationTokens)
                .HasForeignKey(e => e.VerificationTokenId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<VerificationTokensHistory>()
                .Property(e => e.Content)
                .IsUnicode(false);
        }
    }
}
