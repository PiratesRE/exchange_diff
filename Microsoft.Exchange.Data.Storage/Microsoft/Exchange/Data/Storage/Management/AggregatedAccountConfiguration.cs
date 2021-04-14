using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class AggregatedAccountConfiguration : UserConfigurationObject
	{
		internal override UserConfigurationObjectSchema Schema
		{
			get
			{
				return AggregatedAccountConfiguration.schema;
			}
		}

		public AggregatedAccountConfiguration()
		{
			base.SetExchangeVersion(ExchangeObjectVersion.Exchange2012);
		}

		public SmtpAddress? EmailAddress
		{
			get
			{
				return (SmtpAddress?)this[AggregatedAccountConfigurationSchema.EmailAddress];
			}
			set
			{
				this[AggregatedAccountConfigurationSchema.EmailAddress] = value;
			}
		}

		public int? SyncFailureCode
		{
			get
			{
				return (int?)this[AggregatedAccountConfigurationSchema.SyncFailureCode];
			}
			set
			{
				this[AggregatedAccountConfigurationSchema.SyncFailureCode] = value;
			}
		}

		public ExDateTime? SyncFailureTimestamp
		{
			get
			{
				return (ExDateTime?)this[AggregatedAccountConfigurationSchema.SyncFailureTimestamp];
			}
			set
			{
				this[AggregatedAccountConfigurationSchema.SyncFailureTimestamp] = value;
			}
		}

		public string SyncFailureType
		{
			get
			{
				return (string)this[AggregatedAccountConfigurationSchema.SyncFailureType];
			}
			set
			{
				this[AggregatedAccountConfigurationSchema.SyncFailureType] = value;
			}
		}

		public ExDateTime? SyncLastUpdateTimestamp
		{
			get
			{
				return (ExDateTime?)this[AggregatedAccountConfigurationSchema.SyncLastUpdateTimestamp];
			}
			set
			{
				this[AggregatedAccountConfigurationSchema.SyncLastUpdateTimestamp] = value;
			}
		}

		public ExDateTime? SyncQueuedTimestamp
		{
			get
			{
				return (ExDateTime?)this[AggregatedAccountConfigurationSchema.SyncQueuedTimestamp];
			}
			set
			{
				this[AggregatedAccountConfigurationSchema.SyncQueuedTimestamp] = value;
			}
		}

		public Guid? SyncRequestGuid
		{
			get
			{
				return (Guid?)this[AggregatedAccountConfigurationSchema.SyncRequestGuid];
			}
			set
			{
				this[AggregatedAccountConfigurationSchema.SyncRequestGuid] = value;
			}
		}

		public ExDateTime? SyncStartTimestamp
		{
			get
			{
				return (ExDateTime?)this[AggregatedAccountConfigurationSchema.SyncStartTimestamp];
			}
			set
			{
				this[AggregatedAccountConfigurationSchema.SyncStartTimestamp] = value;
			}
		}

		public RequestStatus? SyncStatus
		{
			get
			{
				return (RequestStatus?)this[AggregatedAccountConfigurationSchema.SyncStatus];
			}
			set
			{
				this[AggregatedAccountConfigurationSchema.SyncStatus] = value;
			}
		}

		public ExDateTime? SyncSuspendedTimestamp
		{
			get
			{
				return (ExDateTime?)this[AggregatedAccountConfigurationSchema.SyncSuspendedTimestamp];
			}
			set
			{
				this[AggregatedAccountConfigurationSchema.SyncSuspendedTimestamp] = value;
			}
		}

		public override void Delete(MailboxStoreTypeProvider session)
		{
			using (UserConfiguration mailboxConfiguration = UserConfigurationHelper.GetMailboxConfiguration(session.MailboxSession, "AggregatedAccount", UserConfigurationTypes.Dictionary, false))
			{
				if (mailboxConfiguration == null)
				{
					return;
				}
			}
			UserConfigurationHelper.DeleteMailboxConfiguration(session.MailboxSession, "AggregatedAccount");
		}

		public override IConfigurable Read(MailboxStoreTypeProvider session, ObjectId identity)
		{
			base.Principal = ExchangePrincipal.FromADUser(session.ADUser, null);
			IConfigurable result;
			using (UserConfigurationDictionaryAdapter<AggregatedAccountConfiguration> userConfigurationDictionaryAdapter = new UserConfigurationDictionaryAdapter<AggregatedAccountConfiguration>(session.MailboxSession, "AggregatedAccount", new GetUserConfigurationDelegate(UserConfigurationHelper.GetMailboxConfiguration), AggregatedAccountConfiguration.aggregatedAccountProperties))
			{
				result = userConfigurationDictionaryAdapter.Read(base.Principal);
			}
			return result;
		}

		public override void Save(MailboxStoreTypeProvider session)
		{
			using (UserConfigurationDictionaryAdapter<AggregatedAccountConfiguration> userConfigurationDictionaryAdapter = new UserConfigurationDictionaryAdapter<AggregatedAccountConfiguration>(session.MailboxSession, "AggregatedAccount", SaveMode.NoConflictResolutionForceSave, new GetUserConfigurationDelegate(UserConfigurationHelper.GetMailboxConfiguration), AggregatedAccountConfiguration.aggregatedAccountProperties))
			{
				userConfigurationDictionaryAdapter.Save(this);
			}
			base.ResetChangeTracking();
		}

		internal static object SmtpAddressGetter(IPropertyBag propertyBag)
		{
			string text = propertyBag[AggregatedAccountConfigurationSchema.EmailAddressRaw] as string;
			if (text != null)
			{
				return new SmtpAddress(text);
			}
			return null;
		}

		internal static void SmtpAddressSetter(object value, IPropertyBag propertyBag)
		{
			SmtpAddress? smtpAddress = value as SmtpAddress?;
			propertyBag[AggregatedAccountConfigurationSchema.EmailAddressRaw] = ((smtpAddress != null) ? smtpAddress.Value.ToString() : null);
		}

		internal static object SyncRequestGuidGetter(IPropertyBag propertyBag)
		{
			byte[] array = propertyBag[AggregatedAccountConfigurationSchema.SyncRequestGuidRaw] as byte[];
			if (array != null && 16 == array.Length)
			{
				return new Guid(array);
			}
			return null;
		}

		internal static void SyncRequestGuidSetter(object value, IPropertyBag propertyBag)
		{
			Guid? guid = value as Guid?;
			propertyBag[AggregatedAccountConfigurationSchema.SyncRequestGuidRaw] = ((guid != null) ? guid.Value.ToByteArray() : null);
		}

		private static AggregatedAccountConfigurationSchema schema = ObjectSchema.GetInstance<AggregatedAccountConfigurationSchema>();

		private static SimplePropertyDefinition[] aggregatedAccountProperties = new SimplePropertyDefinition[]
		{
			AggregatedAccountConfigurationSchema.EmailAddressRaw,
			AggregatedAccountConfigurationSchema.SyncFailureCode,
			AggregatedAccountConfigurationSchema.SyncFailureTimestamp,
			AggregatedAccountConfigurationSchema.SyncFailureType,
			AggregatedAccountConfigurationSchema.SyncLastUpdateTimestamp,
			AggregatedAccountConfigurationSchema.SyncQueuedTimestamp,
			AggregatedAccountConfigurationSchema.SyncRequestGuidRaw,
			AggregatedAccountConfigurationSchema.SyncStartTimestamp,
			AggregatedAccountConfigurationSchema.SyncStatus,
			AggregatedAccountConfigurationSchema.SyncSuspendedTimestamp
		};
	}
}
