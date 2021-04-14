using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class MailboxAuditLogRecord : ConfigurableObject
	{
		public MailboxAuditLogRecord() : base(new SimpleProviderPropertyBag())
		{
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public MailboxAuditLogRecord(MailboxAuditLogRecordId identity, string mailboxResolvedName, string guid, DateTime? lastAccessed) : this()
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (string.IsNullOrEmpty(mailboxResolvedName))
			{
				throw new ArgumentNullException("mailbox resolved name");
			}
			this.propertyBag[SimpleProviderObjectSchema.Identity] = identity;
			this.MailboxResolvedOwnerName = ((mailboxResolvedName == null) ? null : mailboxResolvedName);
			this.MailboxGuid = guid;
			this.LastAccessed = lastAccessed;
		}

		public string MailboxGuid
		{
			get
			{
				return this.propertyBag[MailboxAuditLogRecordSchema.MailboxGuid] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogRecordSchema.MailboxGuid] = value;
			}
		}

		public string MailboxResolvedOwnerName
		{
			get
			{
				return this.propertyBag[MailboxAuditLogRecordSchema.MailboxResolvedOwnerName] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogRecordSchema.MailboxResolvedOwnerName] = value;
			}
		}

		public DateTime? LastAccessed
		{
			get
			{
				return (DateTime?)this.propertyBag[MailboxAuditLogRecordSchema.LastAccessed];
			}
			private set
			{
				this.propertyBag[MailboxAuditLogRecordSchema.LastAccessed] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MailboxAuditLogRecord.schema;
			}
		}

		private static readonly ObjectSchema schema = ObjectSchema.GetInstance<MailboxAuditLogRecordSchema>();
	}
}
