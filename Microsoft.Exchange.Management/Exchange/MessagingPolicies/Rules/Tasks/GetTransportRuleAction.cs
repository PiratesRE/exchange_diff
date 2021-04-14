using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[OutputType(new Type[]
	{
		typeof(TransportRuleAction)
	})]
	[Cmdlet("Get", "TransportRuleAction")]
	public sealed class GetTransportRuleAction : Task
	{
		[Parameter(Mandatory = false, Position = 0)]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TypeMapping[] availableActionMappings = TransportRuleAction.GetAvailableActionMappings();
			IConfigDataProvider session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 47, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\TransportRule\\GetTransportRuleAction.cs");
			if (string.IsNullOrEmpty(this.name))
			{
				foreach (TransportRuleAction sendToPipeline in TransportRuleAction.CreateAllAvailableActions(availableActionMappings, session))
				{
					base.WriteObject(sendToPipeline);
				}
				return;
			}
			TransportRuleAction transportRuleAction = TransportRuleAction.CreateAction(availableActionMappings, this.name, session);
			if (transportRuleAction == null)
			{
				base.WriteError(new ArgumentException(Strings.InvalidAction, "Name"), ErrorCategory.InvalidArgument, this.Name);
				return;
			}
			base.WriteObject(transportRuleAction);
		}

		private string name;
	}
}
