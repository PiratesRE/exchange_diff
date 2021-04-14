using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "X400AuthoritativeDomain", DefaultParameterSetName = "Identity")]
	public sealed class GetX400AuthoritativeDomain : GetSystemConfigurationObjectTask<X400AuthoritativeDomainIdParameter, X400AuthoritativeDomain>
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
