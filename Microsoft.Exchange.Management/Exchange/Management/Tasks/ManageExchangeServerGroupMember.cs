using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	public class ManageExchangeServerGroupMember : SetupTaskBase
	{
		[Parameter(Mandatory = false)]
		public string ServerName
		{
			get
			{
				return (string)base.Fields["ServerName"];
			}
			set
			{
				base.Fields["ServerName"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (string.IsNullOrEmpty(this.ServerName))
			{
				this.server = ((ITopologyConfigurationSession)this.domainConfigurationSession).FindLocalComputer();
			}
			else
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 99, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\ManageExchangeServerGroupMember.cs");
				topologyConfigurationSession.UseConfigNC = false;
				topologyConfigurationSession.UseGlobalCatalog = true;
				this.server = topologyConfigurationSession.FindComputerByHostName(this.ServerName);
			}
			if (this.server != null)
			{
				base.LogReadObject(this.server);
				this.serverDomain = this.domainConfigurationSession.Read<ADDomain>(this.server.Id.DomainId);
				if (this.serverDomain != null)
				{
					base.LogReadObject(this.serverDomain);
					this.meso = ((ITopologyConfigurationSession)this.domainConfigurationSession).FindMesoContainer(this.serverDomain);
					if (this.meso != null)
					{
						base.LogReadObject(this.meso);
						this.recipientSession.DomainController = this.meso.OriginatingServer;
						this.e12ds = DirectoryCommon.FindE12DomainServersGroup(this.recipientSession, this.meso);
						if (this.e12ds != null)
						{
							base.LogReadObject(this.e12ds);
						}
					}
				}
			}
			this.exs = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.ExSWkGuid);
			this.ets = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.EtsWkGuid);
			TaskLogger.LogExit();
		}

		protected ADComputer server;

		protected ADGroup exs;

		protected ADGroup ets;

		protected ADGroup e12ds;

		protected ADDomain serverDomain;

		protected MesoContainer meso;
	}
}
