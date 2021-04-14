using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;

namespace Microsoft.Exchange.Management.AgentTasks
{
	[Cmdlet("Uninstall", "TransportAgent", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public class UninstallTransportAgent : AgentBaseTask
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			string strB = base.ValidateAndNormalizeAgentIdentity(this.Identity.ToString());
			foreach (AgentInfo agentInfo in base.MExConfiguration.GetPublicAgentList())
			{
				if (string.Compare(agentInfo.AgentName, strB, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					base.MExConfiguration.AgentList.Remove(agentInfo);
					base.Save();
					this.WriteWarning(AgentStrings.RestartServiceForChanges(base.GetTransportServiceName()));
					TaskLogger.LogExit();
					return;
				}
			}
			base.WriteError(new ArgumentException(AgentStrings.AgentNotFound(this.Identity.ToString()), "Identity"), ErrorCategory.InvalidArgument, null);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return AgentStrings.ConfirmationMessageUninstallTransportAgent(this.Identity.ToString());
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
