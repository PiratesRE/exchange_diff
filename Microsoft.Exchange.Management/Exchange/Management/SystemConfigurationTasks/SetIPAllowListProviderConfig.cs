using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "IPAllowListProvidersConfig", SupportsShouldProcess = true)]
	public sealed class SetIPAllowListProviderConfig : SetSingletonSystemConfigurationObjectTask<IPAllowListProviderConfig>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetIPAllowListProvidersConfig;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}
	}
}
