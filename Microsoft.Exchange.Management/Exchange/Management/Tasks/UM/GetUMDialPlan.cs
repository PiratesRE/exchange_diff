using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Get", "UMDialPlan", DefaultParameterSetName = "Identity")]
	public sealed class GetUMDialPlan : GetMultitenancySystemConfigurationObjectTask<UMDialPlanIdParameter, UMDialPlan>
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
