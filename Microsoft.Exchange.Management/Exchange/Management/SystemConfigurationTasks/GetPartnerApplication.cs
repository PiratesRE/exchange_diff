using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "PartnerApplication")]
	public sealed class GetPartnerApplication : GetMultitenancySystemConfigurationObjectTask<PartnerApplicationIdParameter, PartnerApplication>
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
