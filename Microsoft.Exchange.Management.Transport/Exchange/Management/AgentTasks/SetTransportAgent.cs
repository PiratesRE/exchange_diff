using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;

namespace Microsoft.Exchange.Management.AgentTasks
{
	[Cmdlet("Set", "TransportAgent", SupportsShouldProcess = true)]
	public class SetTransportAgent : AgentBaseTask
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return AgentStrings.ConfirmationMessageSetTransportAgent(this.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			this.cleanIdentity = base.ValidateAndNormalizeAgentIdentity(this.Identity.ToString());
			if (!base.AgentExists(this.cleanIdentity))
			{
				base.WriteError(new ArgumentException(AgentStrings.AgentNotFound(this.Identity.ToString()), "Identity"), ErrorCategory.InvalidArgument, null);
			}
			IList<AgentInfo> publicAgentList = base.MExConfiguration.GetPublicAgentList();
			if (this.Priority != null)
			{
				if (this.Priority < 1 || this.Priority > publicAgentList.Count)
				{
					base.WriteError(new ArgumentOutOfRangeException(AgentStrings.PriorityOutOfRange(publicAgentList.Count.ToString())), ErrorCategory.InvalidArgument, null);
				}
				foreach (AgentInfo agentInfo in publicAgentList)
				{
					if (string.Compare(agentInfo.AgentName, this.cleanIdentity, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						base.MExConfiguration.AgentList.Remove(agentInfo);
						int index = this.Priority.Value - 1 + base.MExConfiguration.GetPreExecutionInternalAgents().Count;
						base.MExConfiguration.AgentList.Insert(index, agentInfo);
						base.Save();
						this.WriteWarning(AgentStrings.RestartServiceForChanges(base.GetTransportServiceName()));
						TaskLogger.LogExit();
						return;
					}
				}
				base.WriteError(new ArgumentException(AgentStrings.AgentNotFound(this.Identity.ToString()), "Identity"), ErrorCategory.InvalidArgument, null);
				return;
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

		[Parameter(Mandatory = false)]
		public int? Priority
		{
			get
			{
				return (int?)base.Fields["Priority"];
			}
			set
			{
				base.Fields["Priority"] = value;
			}
		}

		private string cleanIdentity;
	}
}
