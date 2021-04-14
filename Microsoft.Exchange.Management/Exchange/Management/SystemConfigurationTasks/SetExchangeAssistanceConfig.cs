using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "ExchangeAssistanceConfig", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetExchangeAssistanceConfig : SetMultitenancySingletonSystemConfigurationObjectTask<ExchangeAssistance>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (base.Identity != null)
				{
					return Strings.ConfirmationMessageSetExchangeAssistanceId(base.Identity.ToString());
				}
				return Strings.ConfirmationMessageSetExchangeAssistance;
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
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
