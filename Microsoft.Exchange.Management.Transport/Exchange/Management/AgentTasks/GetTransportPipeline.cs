using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Management.AgentTasks
{
	[OutputType(new Type[]
	{
		typeof(TransportEvent)
	})]
	[Cmdlet("Get", "TransportPipeline")]
	public class GetTransportPipeline : DataAccessTask<Server>
	{
		[Parameter]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 79, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\Agents\\GetTransportPipeline.cs");
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			bool flag = false;
			for (int i = 0; i < GetTransportPipeline.AgentGroups.Length; i++)
			{
				AgentSubscription agentSubscription = AgentSubscription.Load(GetTransportPipeline.AgentGroups[i]);
				if (agentSubscription != null)
				{
					try
					{
						foreach (string eventTopic in GetTransportPipeline.EventTopics[i])
						{
							TransportEvent sendToPipeline = new TransportEvent(eventTopic, agentSubscription[eventTopic]);
							base.WriteObject(sendToPipeline);
							flag = true;
						}
					}
					finally
					{
						agentSubscription.Dispose();
					}
				}
			}
			if (!flag)
			{
				this.WriteWarning(AgentStrings.NoTransportPipelineData);
			}
			TaskLogger.LogExit();
		}

		private static readonly string[] AgentGroups = new string[]
		{
			"Microsoft.Exchange.Data.Transport.Smtp.SmtpReceiveAgent",
			"Microsoft.Exchange.Data.Transport.Routing.RoutingAgent"
		};

		private static readonly string[][] EventTopics = new string[][]
		{
			SmtpEventBindings.All,
			RoutingEventBindings.All
		};
	}
}
