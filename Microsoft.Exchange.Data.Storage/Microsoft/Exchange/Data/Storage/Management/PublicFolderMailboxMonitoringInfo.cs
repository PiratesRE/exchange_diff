using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public abstract class PublicFolderMailboxMonitoringInfo : ConfigurableObject
	{
		public PublicFolderMailboxMonitoringInfo() : base(new SimplePropertyBag(SimpleProviderObjectSchema.Identity, SimpleProviderObjectSchema.ObjectState, SimpleProviderObjectSchema.ExchangeVersion))
		{
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = new PublicFolderMailboxDiagnosticsInfoId();
			this.DisplayName = "Public Folder Diagnostics Information";
			this.propertyBag.ResetChangeTracking();
		}

		public string DisplayName
		{
			get
			{
				return (string)this[PublicFolderMailboxMonitoringInfoSchema.DisplayName];
			}
			private set
			{
				this[PublicFolderMailboxMonitoringInfoSchema.DisplayName] = value;
			}
		}

		public string LastSyncFailure
		{
			get
			{
				return (string)this[PublicFolderMailboxMonitoringInfoSchema.LastSyncFailure];
			}
			internal set
			{
				this[PublicFolderMailboxMonitoringInfoSchema.LastSyncFailure] = value;
			}
		}

		public ExDateTime? LastAttemptedSyncTime
		{
			get
			{
				return (ExDateTime?)this[PublicFolderMailboxMonitoringInfoSchema.LastAttemptedSyncTime];
			}
			internal set
			{
				this[PublicFolderMailboxMonitoringInfoSchema.LastAttemptedSyncTime] = value;
			}
		}

		public ExDateTime? LastSuccessfulSyncTime
		{
			get
			{
				return (ExDateTime?)this[PublicFolderMailboxMonitoringInfoSchema.LastSuccessfulSyncTime];
			}
			internal set
			{
				this[PublicFolderMailboxMonitoringInfoSchema.LastSuccessfulSyncTime] = value;
			}
		}

		public ExDateTime? LastFailedSyncTime
		{
			get
			{
				return (ExDateTime?)this[PublicFolderMailboxMonitoringInfoSchema.LastFailedSyncTime];
			}
			internal set
			{
				this[PublicFolderMailboxMonitoringInfoSchema.LastFailedSyncTime] = value;
			}
		}

		public int? NumberofAttemptsAfterLastSuccess
		{
			get
			{
				return (int?)this[PublicFolderMailboxMonitoringInfoSchema.NumberofAttemptsAfterLastSuccess];
			}
			internal set
			{
				this[PublicFolderMailboxMonitoringInfoSchema.NumberofAttemptsAfterLastSuccess] = value;
			}
		}

		public ExDateTime? FirstFailedSyncTimeAfterLastSuccess
		{
			get
			{
				return (ExDateTime?)this[PublicFolderMailboxMonitoringInfoSchema.FirstFailedSyncTimeAfterLastSuccess];
			}
			internal set
			{
				this[PublicFolderMailboxMonitoringInfoSchema.FirstFailedSyncTimeAfterLastSuccess] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return PublicFolderMailboxMonitoringInfo.schema;
			}
		}

		public string LastSyncCycleLog
		{
			get
			{
				return (string)this[PublicFolderMailboxMonitoringInfoSchema.LastSyncCycleLog];
			}
			internal set
			{
				this[PublicFolderMailboxMonitoringInfoSchema.LastSyncCycleLog] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal const string DiagnosticsInfoDisplayName = "Public Folder Diagnostics Information";

		private static readonly PublicFolderMailboxMonitoringInfoSchema schema = ObjectSchema.GetInstance<PublicFolderMailboxMonitoringInfoSchema>();
	}
}
