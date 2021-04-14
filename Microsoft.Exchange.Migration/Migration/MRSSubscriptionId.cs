using System;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MRSSubscriptionId : ISubscriptionId, ISnapshotId, IMigrationSerializable
	{
		public MRSSubscriptionId(MigrationType migrationType, IMailboxData mailboxData)
		{
			this.subscriptionId = Guid.Empty;
			this.MigrationType = migrationType;
			this.MailboxData = mailboxData;
		}

		public MRSSubscriptionId(Guid id, MigrationType migrationType, IMailboxData mailboxData)
		{
			MigrationUtil.ThrowOnGuidEmptyArgument(id, "id");
			this.subscriptionId = id;
			this.MigrationType = migrationType;
			this.MailboxData = mailboxData;
		}

		public Guid Id
		{
			get
			{
				if (this.RequestType == MRSRequestType.Move)
				{
					MailboxData mailboxData = this.MailboxData as MailboxData;
					if (mailboxData != null)
					{
						if (mailboxData.ExchangeObjectId != Guid.Empty)
						{
							return mailboxData.ExchangeObjectId;
						}
						if (mailboxData.UserMailboxADObjectId != null)
						{
							return mailboxData.UserMailboxADObjectId.ObjectGuid;
						}
					}
				}
				return this.subscriptionId;
			}
		}

		public Guid RequestGuid
		{
			get
			{
				return this.subscriptionId;
			}
		}

		public MRSRequestType RequestType
		{
			get
			{
				return MRSSubscriptionId.MRSRequestTypeFromMigrationType(this.MigrationType);
			}
		}

		public MigrationType MigrationType { get; private set; }

		public IMailboxData MailboxData { get; private set; }

		public PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return MRSSubscriptionId.MRSSubscriptionIdPropertyDefinitions;
			}
		}

		public bool HasValue
		{
			get
			{
				return this.Id != Guid.Empty;
			}
		}

		public override string ToString()
		{
			return this.Id.ToString();
		}

		public override bool Equals(object obj)
		{
			MRSSubscriptionId mrssubscriptionId = obj as MRSSubscriptionId;
			return mrssubscriptionId != null && this.Id.Equals(mrssubscriptionId.Id) && object.Equals(this.MailboxData.OrganizationId, mrssubscriptionId.MailboxData.OrganizationId);
		}

		public override int GetHashCode()
		{
			return this.Id.GetHashCode() ^ this.MailboxData.OrganizationId.GetHashCode();
		}

		public IIdentityParameter GetIdParameter()
		{
			if (this.RequestType == MRSRequestType.Move)
			{
				return this.MailboxData.GetIdParameter<MoveRequestIdParameter>();
			}
			if (this.RequestType == MRSRequestType.Sync)
			{
				return new SyncRequestIdParameter(this.subscriptionId, this.MailboxData.OrganizationId, this.MailboxData.MailboxIdentifier);
			}
			RequestIndexEntryObjectId identity = new RequestIndexEntryObjectId(this.subscriptionId, this.RequestType, this.MailboxData.OrganizationId, new RequestIndexId(RequestIndexLocation.AD), null);
			MRSRequestType requestType = this.RequestType;
			switch (requestType)
			{
			case MRSRequestType.Merge:
				return new MergeRequestIdParameter(identity);
			case MRSRequestType.MailboxImport:
				return new MailboxImportRequestIdParameter(identity);
			default:
				if (requestType != MRSRequestType.PublicFolderMailboxMigration)
				{
					throw new NotSupportedException("don't support request type yet...");
				}
				return new PublicFolderMailboxMigrationRequestIdParameter(identity);
			}
		}

		public bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.subscriptionId = MigrationHelper.GetGuidProperty(message, MigrationBatchMessageSchema.MigrationJobItemMRSId, false);
			return this.subscriptionId != Guid.Empty;
		}

		public void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			if (this.subscriptionId == Guid.Empty)
			{
				message.Delete(MigrationBatchMessageSchema.MigrationJobItemMRSId);
				return;
			}
			message[MigrationBatchMessageSchema.MigrationJobItemMRSId] = this.Id;
		}

		public XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			return new XElement("MRSSubscriptionId", new XElement("RequestGuid", this.RequestGuid));
		}

		private static MRSRequestType MRSRequestTypeFromMigrationType(MigrationType migrationType)
		{
			if (migrationType <= MigrationType.ExchangeRemoteMove)
			{
				switch (migrationType)
				{
				case MigrationType.IMAP:
				case MigrationType.XO1:
					return MRSRequestType.Sync;
				case MigrationType.IMAP | MigrationType.XO1:
					goto IL_45;
				case MigrationType.ExchangeOutlookAnywhere:
					return MRSRequestType.Merge;
				default:
					if (migrationType != MigrationType.ExchangeRemoteMove)
					{
						goto IL_45;
					}
					break;
				}
			}
			else if (migrationType != MigrationType.ExchangeLocalMove)
			{
				if (migrationType == MigrationType.PSTImport)
				{
					return MRSRequestType.MailboxImport;
				}
				if (migrationType != MigrationType.PublicFolder)
				{
					goto IL_45;
				}
				return MRSRequestType.PublicFolderMailboxMigration;
			}
			return MRSRequestType.Move;
			IL_45:
			MigrationUtil.AssertOrThrow(false, "Unsupported migration type '{0}' for MRSSubscriptionId.", new object[]
			{
				migrationType
			});
			return MRSRequestType.Move;
		}

		internal static readonly PropertyDefinition[] MRSSubscriptionIdPropertyDefinitions = new StorePropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationJobItemMRSId
		};

		private Guid subscriptionId;
	}
}
