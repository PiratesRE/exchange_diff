using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "ActiveSyncDeviceAutoblockThreshold", DefaultParameterSetName = "Identity")]
	public sealed class GetActiveSyncDeviceAutoblockThreshold : GetMultitenancySystemConfigurationObjectTask<ActiveSyncDeviceAutoblockThresholdIdParameter, ActiveSyncDeviceAutoblockThreshold>
	{
		public override OrganizationIdParameter Organization
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}
	}
}
