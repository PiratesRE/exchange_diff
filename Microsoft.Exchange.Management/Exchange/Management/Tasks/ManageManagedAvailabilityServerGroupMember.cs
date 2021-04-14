using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	public class ManageManagedAvailabilityServerGroupMember : SetupTaskBase
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
			this.gcSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 69, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\ManageManagedAvailabilityServerGroupMember.cs");
			this.gcSession.UseConfigNC = false;
			this.gcSession.UseGlobalCatalog = true;
			if (string.IsNullOrEmpty(this.ServerName))
			{
				this.ServerName = Environment.MachineName;
			}
			this.server = this.gcSession.FindComputerByHostName(this.ServerName);
			if (this.server != null)
			{
				base.LogReadObject(this.server);
			}
			this.mas = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.MaSWkGuid);
			TaskLogger.LogExit();
		}

		protected ADComputer server;

		protected ADGroup mas;

		internal ITopologyConfigurationSession gcSession;
	}
}
