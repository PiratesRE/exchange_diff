using System;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "DatabaseAvailabilityGroupNetwork")]
	public sealed class GetDatabaseAvailabilityGroupNetwork : GetTenantADObjectWithIdentityTaskBase<DatabaseAvailabilityGroupNetworkIdParameter, DatabaseAvailabilityGroupNetwork>
	{
		[Parameter]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return DagTaskHelper.IsKnownException(this, e) || base.IsKnownException(e);
		}

		private IConfigurationSession SetupAdSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 63, "SetupAdSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\GetDatabaseAvailabilityGroupNetwork.cs");
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession adSession = this.SetupAdSession();
			return new DagNetworkConfigDataProvider(adSession, this.m_targetServerName, this.m_dag);
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.Server != null)
			{
				ITopologyConfigurationSession globalConfigSession = base.GlobalConfigSession;
				Server server = (Server)base.GetDataObject<Server>(this.Server, globalConfigSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
				if (base.HasErrors || server == null)
				{
					return;
				}
				if (!server.IsE14OrLater)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorServerNotE14OrLater(this.Server.ToString())), ErrorCategory.InvalidOperation, this.Server);
				}
				if (!server.IsMailboxServer)
				{
					base.WriteError(server.GetServerRoleError(ServerRole.Mailbox), ErrorCategory.InvalidOperation, this.Server);
					return;
				}
				this.m_targetServerName = server.Fqdn;
				this.m_dag = null;
				ADObjectId databaseAvailabilityGroup = server.DatabaseAvailabilityGroup;
				if (databaseAvailabilityGroup == null)
				{
					base.WriteError(new ServerNotInDagException(server.Fqdn), ErrorCategory.InvalidData, null);
				}
				this.m_dag = globalConfigSession.Read<DatabaseAvailabilityGroup>(databaseAvailabilityGroup);
				if (this.m_dag == null)
				{
					base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorManagementObjectNotFound(databaseAvailabilityGroup.ToString())), ErrorCategory.InvalidData, databaseAvailabilityGroup);
				}
			}
		}

		private string m_targetServerName;

		private DatabaseAvailabilityGroup m_dag;
	}
}
