using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class SyncInfo : ConfigurableObject
	{
		public SyncInfo(string displayName, string url) : base(new SimplePropertyBag(SimpleProviderObjectSchema.Identity, SimpleProviderObjectSchema.ObjectState, SimpleProviderObjectSchema.ExchangeVersion))
		{
			if (string.IsNullOrEmpty(displayName))
			{
				throw new ArgumentNullException("displayName");
			}
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = new SyncInfoId();
			this.propertyBag.ResetChangeTracking();
			this.DisplayName = displayName;
			this.Url = url;
		}

		public string DisplayName
		{
			get
			{
				return (string)this[SyncInfoSchema.DisplayName];
			}
			private set
			{
				this[SyncInfoSchema.DisplayName] = value;
			}
		}

		public string Url
		{
			get
			{
				return (string)this[SyncInfoSchema.Url];
			}
			internal set
			{
				this[SyncInfoSchema.Url] = value;
			}
		}

		public string LastSyncFailure
		{
			get
			{
				return (string)this[SyncInfoSchema.LastSyncFailure];
			}
			internal set
			{
				this[SyncInfoSchema.LastSyncFailure] = value;
			}
		}

		public ExDateTime? FirstAttemptedSyncTime
		{
			get
			{
				return (ExDateTime?)this[SyncInfoSchema.FirstAttemptedSyncTime];
			}
			internal set
			{
				this[SyncInfoSchema.FirstAttemptedSyncTime] = value;
			}
		}

		public ExDateTime? LastAttemptedSyncTime
		{
			get
			{
				return (ExDateTime?)this[SyncInfoSchema.LastAttemptedSyncTime];
			}
			internal set
			{
				this[SyncInfoSchema.LastAttemptedSyncTime] = value;
			}
		}

		public ExDateTime? LastSuccessfulSyncTime
		{
			get
			{
				return (ExDateTime?)this[SyncInfoSchema.LastSuccessfulSyncTime];
			}
			internal set
			{
				this[SyncInfoSchema.LastSuccessfulSyncTime] = value;
			}
		}

		public ExDateTime? LastFailedSyncTime
		{
			get
			{
				return (ExDateTime?)this[SyncInfoSchema.LastFailedSyncTime];
			}
			internal set
			{
				this[SyncInfoSchema.LastFailedSyncTime] = value;
			}
		}

		public ExDateTime? LastFailedSyncEmailTime
		{
			get
			{
				return (ExDateTime?)this[SyncInfoSchema.LastFailedSyncEmailTime];
			}
			internal set
			{
				this[SyncInfoSchema.LastFailedSyncEmailTime] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return SyncInfo.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly SyncInfoSchema schema = ObjectSchema.GetInstance<SyncInfoSchema>();
	}
}
