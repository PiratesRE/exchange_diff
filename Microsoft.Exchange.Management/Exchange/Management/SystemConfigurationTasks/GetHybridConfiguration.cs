using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "HybridConfiguration")]
	public sealed class GetHybridConfiguration : GetSingletonSystemConfigurationObjectTask<HybridConfiguration>
	{
		protected override ObjectId RootId
		{
			get
			{
				ADObjectId currentOrgContainerId = base.CurrentOrgContainerId;
				return HybridConfiguration.GetWellKnownParentLocation(currentOrgContainerId);
			}
		}
	}
}
