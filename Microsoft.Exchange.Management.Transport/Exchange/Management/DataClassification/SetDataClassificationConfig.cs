using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.DataClassification
{
	[Cmdlet("Set", "DataClassificationConfig", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetDataClassificationConfig : SetMultitenancySingletonSystemConfigurationObjectTask<DataClassificationConfig>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetDataClassificationConfig;
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
