using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncSubscriptionId : ISubscriptionId, ISnapshotId, IMigrationSerializable
	{
		internal SyncSubscriptionId(IMailboxData mailboxData)
		{
			this.MailboxData = mailboxData;
		}

		internal SyncSubscriptionId(Guid? subscriptionId, StoreObjectId subscriptionMessageId, IMailboxData mailboxData) : this(mailboxData)
		{
			this.Id = subscriptionId;
			this.MessageId = subscriptionMessageId;
		}

		public StoreObjectId MessageId
		{
			get
			{
				return this.messageId;
			}
			private set
			{
				this.messageId = value;
			}
		}

		public Guid? Id
		{
			get
			{
				return this.id;
			}
			private set
			{
				this.id = value;
			}
		}

		public bool HasValue
		{
			get
			{
				return this.MessageId != null;
			}
		}

		public ADObjectId MailboxOwner
		{
			get
			{
				return ((MailboxData)this.MailboxData).UserMailboxADObjectId;
			}
		}

		public PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return SyncSubscriptionId.SyncSubscriptionIdPropertyDefinitions;
			}
		}

		public MigrationType MigrationType
		{
			get
			{
				return MigrationType.IMAP;
			}
		}

		public IMailboxData MailboxData { get; private set; }

		public override string ToString()
		{
			if (this.Id != null)
			{
				return this.Id.Value.ToString();
			}
			if (this.MessageId != null)
			{
				return this.MessageId.ToString();
			}
			return string.Empty;
		}

		public bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.MessageId = MigrationHelper.GetObjectIdProperty(message, MigrationBatchMessageSchema.MigrationJobItemSubscriptionMessageId, false);
			Guid guidProperty = MigrationHelper.GetGuidProperty(message, MigrationBatchMessageSchema.MigrationJobItemSubscriptionId, false);
			this.Id = null;
			if (guidProperty != Guid.Empty)
			{
				this.Id = new Guid?(guidProperty);
			}
			return this.HasValue;
		}

		public void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			if (this.MessageId == null)
			{
				message.Delete(MigrationBatchMessageSchema.MigrationJobItemSubscriptionMessageId);
			}
			else
			{
				message[MigrationBatchMessageSchema.MigrationJobItemSubscriptionMessageId] = this.MessageId.GetBytes();
			}
			if (this.Id == null)
			{
				message.Delete(MigrationBatchMessageSchema.MigrationJobItemSubscriptionId);
				return;
			}
			message[MigrationBatchMessageSchema.MigrationJobItemSubscriptionId] = this.Id;
		}

		public XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			return new XElement(MigrationBatchMessageSchema.MigrationJobItemSubscriptionId.Name, new object[]
			{
				new XElement("Id", this.Id),
				new XElement("MessageId", this.MessageId)
			});
		}

		public static readonly PropertyDefinition[] SyncSubscriptionIdPropertyDefinitions = new StorePropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationJobItemSubscriptionId,
			MigrationBatchMessageSchema.MigrationJobItemSubscriptionMessageId
		};

		private StoreObjectId messageId;

		private Guid? id;
	}
}
