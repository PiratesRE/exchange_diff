using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "HostedContentFilterPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetHostedContentFilterPolicy : GetMultitenancySystemConfigurationObjectTask<HostedContentFilterPolicyIdParameter, HostedContentFilterPolicy>
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

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ExchangeVersion, ExchangeObjectVersion.Exchange2012);
				if (base.InternalFilter != null)
				{
					return new AndFilter(new QueryFilter[]
					{
						base.InternalFilter,
						queryFilter
					});
				}
				return queryFilter;
			}
		}
	}
}
