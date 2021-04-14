using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "MobileDeviceStatistics", DefaultParameterSetName = "Identity")]
	public class GetMobileDeviceStatistics : GetMobileDeviceStatisticsBase<MobileDeviceIdParameter, MobileDevice>
	{
	}
}
