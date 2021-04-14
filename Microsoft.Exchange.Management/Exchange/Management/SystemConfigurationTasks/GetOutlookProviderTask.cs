using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "OutlookProvider", DefaultParameterSetName = "Identity")]
	public sealed class GetOutlookProviderTask : GetMultitenancySystemConfigurationObjectTask<OutlookProviderIdParameter, OutlookProvider>
	{
		protected override ObjectId RootId
		{
			get
			{
				return OutlookProvider.GetParentContainer(base.DataSession as ITopologyConfigurationSession);
			}
		}
	}
}
