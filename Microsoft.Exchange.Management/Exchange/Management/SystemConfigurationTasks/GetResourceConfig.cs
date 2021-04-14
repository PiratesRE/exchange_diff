using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "ResourceConfig")]
	public sealed class GetResourceConfig : GetMultitenancySingletonSystemConfigurationObjectTask<ResourceBookingConfig>
	{
		protected override ObjectId RootId
		{
			get
			{
				ADObjectId orgContainerId = base.CurrentOrgContainerId;
				if (base.SharedConfiguration != null)
				{
					orgContainerId = base.SharedConfiguration.SharedConfigurationCU.Id;
				}
				return ResourceBookingConfig.GetWellKnownParentLocation(orgContainerId);
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}
	}
}
