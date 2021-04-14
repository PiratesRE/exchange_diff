using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class LegacyMailboxDatabase : LegacyDatabase
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return LegacyMailboxDatabase.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return LegacyMailboxDatabase.MostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			errors.AddRange(LegacyDatabase.ValidateAscendingQuotas(this.propertyBag, new ProviderPropertyDefinition[]
			{
				LegacyDatabaseSchema.IssueWarningQuota,
				LegacyMailboxDatabaseSchema.ProhibitSendQuota,
				LegacyMailboxDatabaseSchema.ProhibitSendReceiveQuota
			}, this.Identity));
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(LegacyMailboxDatabaseSchema.ProhibitSendReceiveQuota))
			{
				this.ProhibitSendReceiveQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromMB(2355UL));
			}
			if (!base.IsModified(LegacyMailboxDatabaseSchema.ProhibitSendQuota))
			{
				this.ProhibitSendQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromGB(2UL));
			}
			base.StampPersistableDefaultValues();
		}

		public ADObjectId JournalRecipient
		{
			get
			{
				return (ADObjectId)this[LegacyMailboxDatabaseSchema.JournalRecipient];
			}
			set
			{
				this[LegacyMailboxDatabaseSchema.JournalRecipient] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MailboxRetention
		{
			get
			{
				return (EnhancedTimeSpan)this[LegacyMailboxDatabaseSchema.MailboxRetention];
			}
			set
			{
				this[LegacyMailboxDatabaseSchema.MailboxRetention] = value;
			}
		}

		public ADObjectId OfflineAddressBook
		{
			get
			{
				return (ADObjectId)this[LegacyMailboxDatabaseSchema.OfflineAddressBook];
			}
			set
			{
				this[LegacyMailboxDatabaseSchema.OfflineAddressBook] = value;
			}
		}

		public ADObjectId OriginalDatabase
		{
			get
			{
				return (ADObjectId)this[LegacyMailboxDatabaseSchema.OriginalDatabase];
			}
			internal set
			{
				this[LegacyMailboxDatabaseSchema.OriginalDatabase] = value;
			}
		}

		public ADObjectId PublicFolderDatabase
		{
			get
			{
				return (ADObjectId)this[LegacyMailboxDatabaseSchema.PublicFolderDatabase];
			}
			set
			{
				this[LegacyMailboxDatabaseSchema.PublicFolderDatabase] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ProhibitSendReceiveQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[LegacyMailboxDatabaseSchema.ProhibitSendReceiveQuota];
			}
			set
			{
				this[LegacyMailboxDatabaseSchema.ProhibitSendReceiveQuota] = value;
			}
		}

		public bool Recovery
		{
			get
			{
				return (bool)this[LegacyMailboxDatabaseSchema.Recovery];
			}
			internal set
			{
				this[LegacyMailboxDatabaseSchema.Recovery] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ProhibitSendQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[LegacyMailboxDatabaseSchema.ProhibitSendQuota];
			}
			set
			{
				this[LegacyMailboxDatabaseSchema.ProhibitSendQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IndexEnabled
		{
			get
			{
				return (bool)this[LegacyMailboxDatabaseSchema.IndexEnabled];
			}
			set
			{
				this[LegacyMailboxDatabaseSchema.IndexEnabled] = value;
			}
		}

		private static LegacyMailboxDatabaseSchema schema = ObjectSchema.GetInstance<LegacyMailboxDatabaseSchema>();

		internal static readonly string MostDerivedClass = "msExchPrivateMDB";
	}
}
