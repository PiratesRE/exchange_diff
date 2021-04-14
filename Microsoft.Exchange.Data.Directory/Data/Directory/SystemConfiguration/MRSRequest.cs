using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class MRSRequest : ADConfigurationObject
	{
		public Guid MailboxMoveRequestGuid
		{
			get
			{
				return (Guid)this[MRSRequestSchema.MailboxMoveRequestGuid];
			}
			set
			{
				this[MRSRequestSchema.MailboxMoveRequestGuid] = value;
			}
		}

		public RequestStatus MailboxMoveStatus
		{
			get
			{
				return (RequestStatus)this[MRSRequestSchema.MailboxMoveStatus];
			}
			set
			{
				this[MRSRequestSchema.MailboxMoveStatus] = value;
			}
		}

		public RequestFlags MailboxMoveFlags
		{
			get
			{
				return (RequestFlags)this[MRSRequestSchema.MailboxMoveFlags];
			}
			set
			{
				this[MRSRequestSchema.MailboxMoveFlags] = value;
			}
		}

		public string MailboxMoveBatchName
		{
			get
			{
				return (string)this[MRSRequestSchema.MailboxMoveBatchName];
			}
			set
			{
				this[MRSRequestSchema.MailboxMoveBatchName] = value;
			}
		}

		public string MailboxMoveRemoteHostName
		{
			get
			{
				return (string)this[MRSRequestSchema.MailboxMoveRemoteHostName];
			}
			set
			{
				this[MRSRequestSchema.MailboxMoveRemoteHostName] = value;
			}
		}

		public string MailboxMoveFilePath
		{
			get
			{
				return (string)this[MRSRequestSchema.MailboxMoveFilePath];
			}
			set
			{
				this[MRSRequestSchema.MailboxMoveFilePath] = value;
			}
		}

		public ADObjectId MailboxMoveTargetMDB
		{
			get
			{
				return (ADObjectId)this[MRSRequestSchema.MailboxMoveTargetMDB];
			}
			set
			{
				this[MRSRequestSchema.MailboxMoveTargetMDB] = ADObjectIdResolutionHelper.ResolveDN(value);
			}
		}

		public ADObjectId MailboxMoveSourceMDB
		{
			get
			{
				return (ADObjectId)this[MRSRequestSchema.MailboxMoveSourceMDB];
			}
			set
			{
				this[MRSRequestSchema.MailboxMoveSourceMDB] = ADObjectIdResolutionHelper.ResolveDN(value);
			}
		}

		public ADObjectId MailboxMoveStorageMDB
		{
			get
			{
				return (ADObjectId)this[MRSRequestSchema.MailboxMoveStorageMDB];
			}
			set
			{
				this[MRSRequestSchema.MailboxMoveStorageMDB] = ADObjectIdResolutionHelper.ResolveDN(value);
			}
		}

		public ADObjectId MailboxMoveTargetUser
		{
			get
			{
				return (ADObjectId)this[MRSRequestSchema.MailboxMoveTargetUser];
			}
			set
			{
				this[MRSRequestSchema.MailboxMoveTargetUser] = value;
			}
		}

		public ADObjectId MailboxMoveSourceUser
		{
			get
			{
				return (ADObjectId)this[MRSRequestSchema.MailboxMoveSourceUser];
			}
			set
			{
				this[MRSRequestSchema.MailboxMoveSourceUser] = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[MRSRequestSchema.DisplayName];
			}
			set
			{
				this[MRSRequestSchema.DisplayName] = value;
			}
		}

		public MRSRequestType RequestType
		{
			get
			{
				return (MRSRequestType)this[MRSRequestSchema.MRSRequestType];
			}
			set
			{
				this[MRSRequestSchema.MRSRequestType] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return MRSRequest.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchMRSRequest";
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return MRSRequest.parentPath;
			}
		}

		internal const string MRSContainerName = "CN=Mailbox Replication";

		internal const string MergeContainerName = "CN=MergeRequests";

		internal const string MailboxImportContainerName = "CN=MailboxImportRequests";

		internal const string MailboxExportContainerName = "CN=MailboxExportRequests";

		internal const string MailboxRestoreContainerName = "CN=MailboxRestoreRequests";

		internal const string PublicFolderMoveContainerName = "CN=PublicFolderMoveRequests";

		internal const string PublicFolderMigrationContainerName = "CN=PublicFolderMigrationRequests";

		internal const string PublicFolderMailboxMigrationContainerName = "CN=PublicFolderMailboxMigrationRequests";

		internal const string SyncContainerName = "CN=SyncRequests";

		internal const string FolderMoveContainerName = "CN=FolderMoveRequests";

		private const string MostDerivedClass = "msExchMRSRequest";

		private static MRSRequestSchema schema = ObjectSchema.GetInstance<MRSRequestSchema>();

		private static ADObjectId parentPath = new ADObjectId("CN=Mailbox Replication");
	}
}
