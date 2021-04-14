using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DeltaSyncSettings
	{
		internal DeltaSyncSettings(ServiceSettingsPropertiesType serviceProperties, PropertiesType accountProperties)
		{
			SyncUtilities.ThrowIfArgumentNull("serviceProperties", serviceProperties);
			SyncUtilities.ThrowIfArgumentNull("accountProperties", accountProperties);
			this.serviceProperties = serviceProperties;
			this.accountProperties = accountProperties;
		}

		internal int MinSyncPollInterval
		{
			get
			{
				return this.serviceProperties.MinSyncPollInterval;
			}
		}

		internal int MinSettingsPollInterval
		{
			get
			{
				return this.serviceProperties.MinSettingsPollInterval;
			}
		}

		internal double SyncMultiplier
		{
			get
			{
				return this.serviceProperties.SyncMultiplier;
			}
		}

		internal int MaxObjectsInSync
		{
			get
			{
				return this.serviceProperties.MaxObjectsInSync;
			}
		}

		internal int MaxNumberOfEmailAdds
		{
			get
			{
				return this.serviceProperties.MaxNumberOfEmailAdds;
			}
		}

		internal int MaxNumberOfFolderAdds
		{
			get
			{
				return this.serviceProperties.MaxNumberOfFolderAdds;
			}
		}

		internal int MaxAttachments
		{
			get
			{
				return this.accountProperties.MaxAttachments;
			}
		}

		internal long MaxMessageSize
		{
			get
			{
				return this.accountProperties.MaxMessageSize;
			}
		}

		internal int MaxRecipients
		{
			get
			{
				return this.accountProperties.MaxRecipients;
			}
		}

		internal AccountStatusType AccountStatus
		{
			get
			{
				return this.accountProperties.AccountStatus;
			}
		}

		private ServiceSettingsPropertiesType serviceProperties;

		private PropertiesType accountProperties;
	}
}
