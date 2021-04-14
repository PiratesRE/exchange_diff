using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "SenderReputationConfig", SupportsShouldProcess = true)]
	public sealed class SetSenderReputationConfig : SetSingletonSystemConfigurationObjectTask<SenderReputationConfig>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetSenderReputationConfig;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return (base.DataSession as IConfigurationSession).GetOrgContainerId();
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
