using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Management.ProvisioningAgentTasks
{
	[Cmdlet("Remove", "CmdletExtensionAgent", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveCmdletExtensionAgent : RemoveSystemConfigurationObjectTask<CmdletExtensionAgentIdParameter, CmdletExtensionAgent>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveCmdletExtensionAgent(this.Identity.ToString());
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			this.agentsGlobalConfig = new CmdletExtensionAgentsGlobalConfig((ITopologyConfigurationSession)base.DataSession);
			foreach (LocalizedString text in this.agentsGlobalConfig.ConfigurationIssues)
			{
				this.WriteWarning(text);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			ProvisioningLayer.RefreshProvisioningBroker(this);
			TaskLogger.LogExit();
		}

		private CmdletExtensionAgentsGlobalConfig agentsGlobalConfig;
	}
}
