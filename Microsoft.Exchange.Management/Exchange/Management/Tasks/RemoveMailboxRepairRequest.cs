using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Remove", "MailboxRepairRequest", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveMailboxRepairRequest : SetObjectWithIdentityTaskBase<StoreIntegrityCheckJobIdParameter, StoreIntegrityCheckJob, StoreIntegrityCheckJob>
	{
		[Parameter(Mandatory = false)]
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveMailboxRepairRequestDatabase(this.Identity.ToString());
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || (exception is ServerUnavailableException || exception is RpcException || exception is InvalidIntegrityCheckJobIdentity);
		}

		protected override IConfigDataProvider CreateSession()
		{
			TaskLogger.LogEnter();
			ADObjectId rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerId(this.DomainController, null);
			ADSessionSettings sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, rootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			sessionSettings = ADSessionSettings.RescopeToSubtree(sessionSettings);
			this.readOnlyConfigSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 121, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\OnlineIsInteg\\RemoveMailboxRepairRequest.cs");
			TaskLogger.LogExit();
			return this.readOnlyConfigSession;
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			StoreIntegrityCheckJobIdentity storeIntegrityCheckJobIdentity = new StoreIntegrityCheckJobIdentity(this.Identity.RawIdentity);
			DatabaseIdParameter databaseIdParameter = new DatabaseIdParameter(new ADObjectId(storeIntegrityCheckJobIdentity.DatabaseGuid));
			this.jobGuid = storeIntegrityCheckJobIdentity.JobGuid;
			this.database = (Database)base.GetDataObject<Database>(databaseIdParameter, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(databaseIdParameter.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(databaseIdParameter.ToString())));
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			return new StoreIntegrityCheckJob();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			StoreIntegrityCheckAdminRpc.RemoveStoreIntegrityCheckJob(this.database, this.jobGuid, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			TaskLogger.LogExit();
		}

		private const string ParameterDatabase = "Database";

		private const string ParameterRequest = "Request";

		private IConfigurationSession readOnlyConfigSession;

		private Database database;

		private Guid jobGuid;
	}
}
