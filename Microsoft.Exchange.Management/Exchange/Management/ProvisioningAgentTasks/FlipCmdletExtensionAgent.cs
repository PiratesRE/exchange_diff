using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Management.ProvisioningAgentTasks
{
	public abstract class FlipCmdletExtensionAgent : SystemConfigurationObjectActionTask<CmdletExtensionAgentIdParameter, CmdletExtensionAgent>
	{
		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			this.agentsGlobalConfig = new CmdletExtensionAgentsGlobalConfig((ITopologyConfigurationSession)base.DataSession);
			if (this.agentsGlobalConfig.ConfigurationIssues.Length > 0)
			{
				base.WriteError(new InvalidOperationException(this.agentsGlobalConfig.ConfigurationIssues[0]), ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.DataObject.IsSystem && !this.FlipTo)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorAgentCannotBeDisabled), ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		internal abstract bool FlipTo { get; }

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.DataObject.Enabled = this.FlipTo;
			base.InternalProcessRecord();
			ProvisioningLayer.RefreshProvisioningBroker(this);
			TaskLogger.LogExit();
		}

		private CmdletExtensionAgentsGlobalConfig agentsGlobalConfig;
	}
}
