using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Management.ProvisioningAgentTasks
{
	[Cmdlet("Set", "CmdletExtensionAgent", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetCmdletExtensionAgent : SetTopologySystemConfigurationObjectTask<CmdletExtensionAgentIdParameter, CmdletExtensionAgent>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetCmdletExtensionAgent(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public byte Priority
		{
			get
			{
				if (base.Fields["Priority"] != null)
				{
					return (byte)base.Fields["Priority"];
				}
				return 0;
			}
			set
			{
				base.Fields["Priority"] = value;
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

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.DataObject.IsSystem && base.Fields["Priority"] != null && this.Priority != this.DataObject.Priority)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorAgentPriorityCannotBeChanged), ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (base.Fields.Contains("Priority"))
			{
				byte priority = (byte)base.Fields["Priority"];
				if (!this.agentsGlobalConfig.IsPriorityAvailable(priority, this.DataObject) && !this.agentsGlobalConfig.FreeUpPriorityValue(priority))
				{
					base.WriteError(new ArgumentException(Strings.NotEnoughFreePrioritiesAvailable(priority.ToString())), ErrorCategory.InvalidArgument, null);
				}
				this.DataObject.Priority = priority;
			}
			if (this.agentsGlobalConfig.ObjectsToSave != null)
			{
				foreach (CmdletExtensionAgent instance in this.agentsGlobalConfig.ObjectsToSave)
				{
					base.DataSession.Save(instance);
				}
			}
			base.InternalProcessRecord();
			ProvisioningLayer.RefreshProvisioningBroker(this);
			TaskLogger.LogExit();
		}

		private CmdletExtensionAgentsGlobalConfig agentsGlobalConfig;
	}
}
