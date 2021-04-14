using System;
using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Replay.Monitoring;
using Microsoft.Exchange.Cluster.Replay.Monitoring.Client;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class GetRedundancyTaskBase<TIdentity, TDataObject> : GetSystemConfigurationObjectTask<TIdentity, TDataObject> where TIdentity : IIdentityParameter where TDataObject : ADObject, new()
	{
		[Alias(new string[]
		{
			"Dag"
		})]
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
		public DatabaseAvailabilityGroupIdParameter DatabaseAvailabilityGroup
		{
			get
			{
				return (DatabaseAvailabilityGroupIdParameter)base.Fields["Dag"];
			}
			set
			{
				base.Fields["Dag"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public ServerIdParameter ServerToContact
		{
			get
			{
				return (ServerIdParameter)base.Fields["ServerToContact"];
			}
			set
			{
				base.Fields["ServerToContact"] = value;
			}
		}

		[ValidateRange(0, 2147483)]
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public int TimeoutInSeconds
		{
			get
			{
				return (int)(base.Fields["TimeoutInSeconds"] ?? 10);
			}
			set
			{
				base.Fields["TimeoutInSeconds"] = value;
			}
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.Identity == null && this.DatabaseAvailabilityGroup == null)
			{
				base.WriteError(new InvalidParamSpecifyIdentityOrDagException(), ErrorCategory.InvalidArgument, null);
			}
			if (this.Identity != null)
			{
				TIdentity identity = this.Identity;
				if (identity.RawIdentity.Contains("*"))
				{
					base.WriteError(new InvalidParamIdentityHasWildcardException(), ErrorCategory.InvalidArgument, this.Identity);
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			DatabaseAvailabilityGroup databaseAvailabilityGroup = null;
			ITopologyConfigurationSession globalConfigSession = base.GlobalConfigSession;
			if (this.DatabaseAvailabilityGroup != null)
			{
				databaseAvailabilityGroup = this.LookupDag(this.DatabaseAvailabilityGroup);
			}
			else if (this.Identity != null)
			{
				ADObjectId adobjectId = this.LookupIdentityObjectAndGetDagId();
				databaseAvailabilityGroup = globalConfigSession.Read<DatabaseAvailabilityGroup>(adobjectId);
				if (databaseAvailabilityGroup == null)
				{
					base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorDagNotFound(adobjectId.Name)), ErrorCategory.InvalidData, adobjectId);
				}
			}
			string serverToQueryFqdn = this.DiscoverServerToQuery(this.ServerToContact, databaseAvailabilityGroup);
			HealthInfoPersisted dagHealthInfo = this.GetDagHealthInfo(serverToQueryFqdn);
			this.WriteResultsFromHealthInfo(dagHealthInfo, serverToQueryFqdn);
		}

		protected abstract ADObjectId LookupIdentityObjectAndGetDagId();

		protected abstract void WriteResultsFromHealthInfo(HealthInfoPersisted hip, string serverToQueryFqdn);

		protected HealthInfoPersisted GetDagHealthInfo(string serverToQueryFqdn)
		{
			HealthInfoPersisted hip = null;
			Exception ex = MonitoringServiceClient.HandleException(delegate
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds((double)this.TimeoutInSeconds);
				using (MonitoringServiceClient monitoringServiceClient = MonitoringServiceClient.Open(serverToQueryFqdn, timeSpan, timeSpan, timeSpan, MonitoringServiceClient.ReceiveTimeout))
				{
					Task<HealthInfoPersisted> dagHealthInfoAsync = monitoringServiceClient.GetDagHealthInfoAsync();
					if (!dagHealthInfoAsync.Wait(timeSpan))
					{
						throw new TimeoutException(Strings.GetDagHealthInfoRequestTimedOut(this.TimeoutInSeconds));
					}
					hip = dagHealthInfoAsync.Result;
				}
			});
			if (ex != null)
			{
				base.WriteError(new GetDagHealthInfoRequestException(serverToQueryFqdn, ex.Message, ex), ErrorCategory.InvalidResult, serverToQueryFqdn);
				return null;
			}
			hip.ToString();
			return hip;
		}

		protected DatabaseAvailabilityGroup LookupDag(DatabaseAvailabilityGroupIdParameter dagParam)
		{
			ADObjectId id = new DatabaseAvailabilityGroupContainer().Id;
			return (DatabaseAvailabilityGroup)base.GetDataObject<DatabaseAvailabilityGroup>(dagParam, base.GlobalConfigSession, id, new LocalizedString?(Strings.ErrorDagNotFound(dagParam.ToString())), new LocalizedString?(Strings.ErrorDagNotUnique(dagParam.ToString())));
		}

		protected Server LookupServer(ServerIdParameter serverParam)
		{
			ADObjectId id = new ServersContainer().Id;
			Server server = (Server)base.GetDataObject<Server>(serverParam, base.GlobalConfigSession, id, new LocalizedString?(Strings.ErrorMailboxServerNotFound(serverParam.ToString())), new LocalizedString?(Strings.ErrorMailboxServerNotUnique(serverParam.ToString())));
			if (!server.IsMailboxServer)
			{
				base.WriteError(server.GetServerRoleError(ServerRole.Mailbox), ErrorCategory.InvalidOperation, serverParam);
				return null;
			}
			if (!server.IsE14OrLater)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorServerNotE14OrLater(server.Name)), ErrorCategory.InvalidOperation, serverParam);
				return null;
			}
			return server;
		}

		protected string DiscoverServerToQuery(ServerIdParameter serverToContactParam, DatabaseAvailabilityGroup dag)
		{
			string fqdn;
			if (serverToContactParam == null)
			{
				AmServerName primaryActiveManager = this.GetPrimaryActiveManager(dag);
				fqdn = primaryActiveManager.Fqdn;
			}
			else
			{
				Server server = this.LookupServer(serverToContactParam);
				ADObjectId databaseAvailabilityGroup = server.DatabaseAvailabilityGroup;
				if (databaseAvailabilityGroup == null)
				{
					base.WriteError(new ServerMustBeInDagException(server.Fqdn), ErrorCategory.InvalidData, serverToContactParam);
					return null;
				}
				if (!databaseAvailabilityGroup.Equals(dag.Id))
				{
					base.WriteError(new ServerToContactMustBeInSameDagException(server.Name, dag.Name, databaseAvailabilityGroup.Name), ErrorCategory.InvalidData, serverToContactParam);
					return null;
				}
				fqdn = server.Fqdn;
			}
			return fqdn;
		}

		private AmServerName GetPrimaryActiveManager(DatabaseAvailabilityGroup dag)
		{
			Exception ex = null;
			AmServerName result = null;
			try
			{
				result = DagTaskHelper.GetPrimaryActiveManagerNode(dag);
			}
			catch (AmFailedToDeterminePAM amFailedToDeterminePAM)
			{
				ex = amFailedToDeterminePAM;
			}
			catch (AmServerException ex2)
			{
				ex = ex2;
			}
			catch (AmServerTransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				base.WriteError(new PAMCouldNotBeDeterminedException(dag.Name, ex.Message, ex), ErrorCategory.ConnectionError, dag);
				return null;
			}
			return result;
		}

		protected const int DefaultTimeoutSecs = 10;
	}
}
