using System;
using System.IO;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "MailboxDatabaseCopy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveMailboxDatabaseCopy : RemoveSystemConfigurationObjectTask<DatabaseCopyIdParameter, DatabaseCopy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveMailboxDatabaseCopy(this.m_database.Name, this.m_serverName);
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return AmExceptionHelper.IsKnownClusterException(this, e) || base.IsKnownException(e);
		}

		protected override void InternalStateReset()
		{
			this.m_database = null;
			this.m_server = null;
			this.m_serverName = null;
			this.m_databaseCopy = null;
			base.InternalStateReset();
		}

		private void ValidateDatabaseCopy()
		{
			this.m_databaseCopy = base.DataObject;
			this.m_database = this.m_databaseCopy.GetDatabase<Database>();
			DatabaseTasksHelper.ValidateDatabaseCopyActionTask(this.m_databaseCopy, true, true, base.DataSession, this.RootId, new Task.TaskErrorLoggingDelegate(base.WriteError), Strings.ErrorMailboxDatabaseNotUnique(this.m_database.Identity.ToString()), new LocalizedString?(Strings.ErrorSingleDatabaseCopyRemove(this.m_database.Identity.ToString(), this.m_databaseCopy.HostServerName)), out this.m_server);
			if (this.m_server == null)
			{
				this.m_serverName = this.m_databaseCopy.Name;
			}
			else
			{
				this.m_serverName = this.m_server.Name;
			}
			DatabaseCopy[] allDatabaseCopies = this.m_database.AllDatabaseCopies;
			this.m_originalCopiesLength = allDatabaseCopies.Length;
			if (this.m_originalCopiesLength == 2 && this.m_database.CircularLoggingEnabled)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorInvalidRemoveOperationOnDBCopyForCircularLoggingEnabledDB(this.m_database.Name)), ErrorCategory.InvalidOperation, base.DataObject.Identity);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (this.Identity != null)
			{
				this.Identity.AllowInvalid = true;
			}
			base.InternalValidate();
			if (base.HasErrors)
			{
				TaskLogger.LogExit();
				return;
			}
			this.ValidateDatabaseCopy();
			if (RemoteReplayConfiguration.IsServerRcrSource(ADObjectWrapperFactory.CreateWrapper(this.m_database), this.m_serverName, (ITopologyConfigurationSession)this.ConfigurationSession, out this.m_dbLocationInfo))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorDbMountedOnServer(this.m_database.Identity.ToString(), this.m_serverName)), ErrorCategory.InvalidOperation, this.m_database.Identity);
			}
			if (this.m_server != null)
			{
				base.VerifyIsWithinScopes((IConfigurationSession)base.DataSession, this.m_server, true, new DataAccessTask<DatabaseCopy>.ADObjectOutOfScopeString(Strings.ErrorServerOutOfScope));
			}
			if (this.m_database != null)
			{
				MapiTaskHelper.VerifyDatabaseIsWithinScope(base.SessionSettings, this.m_database, new Task.ErrorLoggerDelegate(base.WriteError));
				if (this.m_database.Servers != null && this.m_database.Servers.Length > 0)
				{
					MapiTaskHelper.VerifyServerIsWithinScope(this.m_database, new Task.ErrorLoggerDelegate(base.WriteError), (ITopologyConfigurationSession)this.ConfigurationSession);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			DatabaseCopy dataObject = base.DataObject;
			base.InternalProcessRecord();
			if (base.HasErrors)
			{
				TaskLogger.LogExit();
				return;
			}
			this.m_database.InvalidDatabaseCopiesAllowed = true;
			this.RunConfigurationUpdaterRpc();
			try
			{
				ReplayState.DeleteState((this.m_server != null) ? this.m_server.Fqdn : this.m_serverName, this.m_database);
			}
			catch (SecurityException ex)
			{
				this.WriteWarning(Strings.ErrorCannotDeleteReplayState(this.m_database.Name, this.m_serverName, ex.Message));
			}
			catch (UnauthorizedAccessException ex2)
			{
				this.WriteWarning(Strings.ErrorCannotDeleteReplayState(this.m_database.Name, this.m_serverName, ex2.Message));
			}
			catch (IOException ex3)
			{
				this.WriteWarning(Strings.ErrorCannotDeleteReplayState(this.m_database.Name, this.m_serverName, ex3.Message));
			}
			catch (RemoteRegistryTimedOutException ex4)
			{
				this.WriteWarning(Strings.ErrorCannotDeleteReplayState(this.m_database.Name, this.m_serverName, ex4.Message));
			}
			string pathName = this.m_database.EdbFilePath.PathName;
			if (string.IsNullOrEmpty(pathName))
			{
				this.WriteWarning(Strings.NeedRemoveCopyLogFileManuallyAfterCopyDisabledRcr(this.m_database.Name, this.m_database.LogFolderPath.PathName, this.m_serverName));
			}
			else
			{
				this.WriteWarning(Strings.NeedRemoveCopyFileManuallyAfterCopyDisabledRcr(this.m_database.Name, this.m_database.LogFolderPath.PathName, pathName, this.m_serverName));
			}
			DatabaseTasksHelper.UpdateDataGuaranteeConstraint((ITopologyConfigurationSession)base.DataSession, this.m_database, new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			TaskLogger.LogExit();
		}

		private void RunConfigurationUpdaterRpc()
		{
			if (this.m_server != null)
			{
				string fqdn = this.m_server.Fqdn;
				DatabaseTasksHelper.RunConfigurationUpdaterRpc(fqdn, this.m_database, this.m_server.AdminDisplayVersion, ReplayConfigChangeHints.DbCopyRemoved, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
			string serverFqdn = this.m_dbLocationInfo.ServerFqdn;
			DatabaseTasksHelper.RunConfigurationUpdaterRpc(serverFqdn, this.m_database, new ServerVersion(this.m_dbLocationInfo.ServerVersion), ReplayConfigChangeHints.DbCopyRemoved, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
		}

		private Server m_server;

		private string m_serverName;

		private Database m_database;

		private DatabaseCopy m_databaseCopy;

		private DatabaseLocationInfo m_dbLocationInfo;

		private int m_originalCopiesLength;
	}
}
