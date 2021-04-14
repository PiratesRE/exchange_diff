using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.PerimeterConfig
{
	[Cmdlet("Get", "PerimeterConfig")]
	public sealed class GetPerimeterConfig : GetMultitenancySingletonSystemConfigurationObjectTask<PerimeterConfig>
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
