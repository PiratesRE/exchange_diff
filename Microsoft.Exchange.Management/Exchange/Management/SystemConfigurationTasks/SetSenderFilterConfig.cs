using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "SenderFilterConfig", SupportsShouldProcess = true)]
	public sealed class SetSenderFilterConfig : SetSingletonSystemConfigurationObjectTask<SenderFilterConfig>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetSenderFilterConfig;
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
