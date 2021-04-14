using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "AvailabilityAddressSpace", DefaultParameterSetName = "Identity")]
	public sealed class GetAvailabilityAddressSpace : GetMultitenancySystemConfigurationObjectTask<AvailabilityAddressSpaceIdParameter, AvailabilityAddressSpace>
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
