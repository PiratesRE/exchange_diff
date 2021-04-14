using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "AdSiteLink", DefaultParameterSetName = "Identity")]
	public sealed class GetAdSiteLink : GetSystemConfigurationObjectTask<AdSiteLinkIdParameter, ADSiteLink>
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
