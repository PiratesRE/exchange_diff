using System;
using System.Collections;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Monitoring;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "ServerComponentState")]
	public sealed class GetServerComponentState : Task
	{
		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public ServerIdParameter Identity
		{
			get
			{
				return this.serverId;
			}
			set
			{
				this.serverId = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Component
		{
			get
			{
				return (string)base.Fields["Component"];
			}
			set
			{
				base.Fields["Component"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Fqdn DomainController
		{
			get
			{
				return this.domainController;
			}
			set
			{
				this.domainController = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.Component != null && !ServerComponentStateManager.IsValidComponent(this.Component))
			{
				base.WriteError(new ArgumentException(Strings.ServerComponentStateInvalidComponentName(this.Component)), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.serverId,
				this.Component
			});
			ADComputer adcomputer = null;
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 98, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\ComponentStates\\GetServerComponentState.cs");
			string text = (!string.IsNullOrWhiteSpace(this.serverId.Fqdn)) ? this.serverId.Fqdn : this.serverId.ToString();
			string text2 = text;
			int num = text.IndexOf('.');
			if (num > 0)
			{
				text2 = text.Substring(0, num);
			}
			Server server = topologyConfigurationSession.FindServerByName(text2);
			if (server == null)
			{
				topologyConfigurationSession.UseConfigNC = false;
				topologyConfigurationSession.UseGlobalCatalog = true;
				adcomputer = topologyConfigurationSession.FindComputerByHostName(text2);
				if (adcomputer == null)
				{
					base.WriteError(new ADServerNotFoundException(text), ErrorCategory.InvalidArgument, null);
				}
			}
			if (string.IsNullOrEmpty(this.Component))
			{
				using (IEnumerator enumerator = Enum.GetValues(typeof(ServerComponentEnum)).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						ServerComponentEnum serverComponent = (ServerComponentEnum)obj;
						if (ServerComponentStateManager.IsValidComponent(serverComponent))
						{
							base.WriteObject(new ServerComponentStatePresentationObject((server != null) ? server.Id : adcomputer.Id, (server != null) ? server.Fqdn : adcomputer.DnsHostName, ServerComponentStateManager.GetComponentId(serverComponent), (server != null) ? server.ComponentStates : adcomputer.ComponentStates));
						}
					}
					goto IL_1B2;
				}
			}
			base.WriteObject(new ServerComponentStatePresentationObject((server != null) ? server.Id : adcomputer.Id, (server != null) ? server.Fqdn : adcomputer.DnsHostName, this.Component, (server != null) ? server.ComponentStates : adcomputer.ComponentStates));
			IL_1B2:
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is IOException;
		}

		private ServerIdParameter serverId;

		private Fqdn domainController;
	}
}
