using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "ExchangeAssistanceConfig")]
	public sealed class GetExchangeAssistanceConfig : GetMultitenancySingletonSystemConfigurationObjectTask<ExchangeAssistance>
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, NewExchangeAssistanceConfig.CurrentVersionContainerName);
			}
		}
	}
}
