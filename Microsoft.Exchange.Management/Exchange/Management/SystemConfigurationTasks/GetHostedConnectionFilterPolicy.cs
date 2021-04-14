using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "HostedConnectionFilterPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetHostedConnectionFilterPolicy : GetMultitenancySystemConfigurationObjectTask<HostedConnectionFilterPolicyIdParameter, HostedConnectionFilterPolicy>
	{
		[Parameter]
		public SwitchParameter IgnoreDehydratedFlag { get; set; }

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Dehydrateable;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}
	}
}
