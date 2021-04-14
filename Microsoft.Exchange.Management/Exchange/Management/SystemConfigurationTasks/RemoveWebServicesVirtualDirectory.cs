using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "WebServicesVirtualDirectory", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveWebServicesVirtualDirectory : RemoveExchangeVirtualDirectory<ADWebServicesVirtualDirectory>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter Force { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveWebServicesVirtualDirectory(this.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			this.WriteWarning(Strings.WarningMessageRemoveWebServicesVirtualDirectory(this.Identity.ToString()));
			if (!this.Force && !base.ShouldContinue(Strings.ConfirmationMessageWebServicesVirtualDirectoryContinue))
			{
				TaskLogger.LogExit();
				return;
			}
			base.InternalProcessRecord();
		}
	}
}
