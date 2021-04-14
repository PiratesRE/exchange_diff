using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.AgentTasks
{
	[Cmdlet("Disable", "TransportAgent", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public class DisableTransportAgent : AgentBaseTask
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			base.SetAgentEnabled(this.Identity.ToString(), false);
			base.Save();
			this.WriteWarning(AgentStrings.RestartServiceForChanges(base.GetTransportServiceName()));
			TaskLogger.LogExit();
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return AgentStrings.ConfirmationMessageDisableTransportAgent(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, ParameterSetName = "Identity", Position = 0)]
		public TransportAgentObjectId Identity
		{
			get
			{
				return (TransportAgentObjectId)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}
	}
}
