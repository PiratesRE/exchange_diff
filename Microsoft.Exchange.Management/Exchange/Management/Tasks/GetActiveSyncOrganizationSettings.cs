using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "ActiveSyncOrganizationSettings", DefaultParameterSetName = "Identity")]
	public sealed class GetActiveSyncOrganizationSettings : GetMultitenancySystemConfigurationObjectTask<ActiveSyncOrganizationSettingsIdParameter, ActiveSyncOrganizationSettings>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}
	}
}
