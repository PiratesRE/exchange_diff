using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.DataClassification
{
	[Cmdlet("Get", "DataClassificationConfig", DefaultParameterSetName = "Identity")]
	public sealed class GetDataClassificationConfig : GetMultitenancySingletonSystemConfigurationObjectTask<DataClassificationConfig>
	{
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
				return SharedTenantConfigurationMode.Static;
			}
		}
	}
}
