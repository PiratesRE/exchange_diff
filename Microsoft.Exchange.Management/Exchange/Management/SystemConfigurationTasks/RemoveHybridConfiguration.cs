using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Hybrid;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "HybridConfiguration", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveHybridConfiguration : RemoveSingletonSystemConfigurationObjectTask<HybridConfiguration>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return HybridStrings.RemoveHybidConfigurationConfirmation;
			}
		}
	}
}
