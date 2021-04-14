using System;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class DatabaseCopyActionTask : SystemConfigurationObjectActionTask<DatabaseCopyIdParameter, DatabaseCopy>
	{
		protected DatabaseCopy DatabaseCopy
		{
			get
			{
				return this.m_databaseCopy;
			}
		}

		protected string DatabaseName
		{
			get
			{
				if (this.DataObject != null)
				{
					return this.DataObject.DatabaseName;
				}
				if (this.Identity != null)
				{
					return this.Identity.DatabaseName;
				}
				return null;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				TaskLogger.LogExit();
				return;
			}
			this.m_databaseCopy = this.DataObject;
			DatabaseTasksHelper.ValidateDatabaseCopyActionTask(this.m_databaseCopy, false, true, base.DataSession, this.RootId, new Task.TaskErrorLoggingDelegate(base.WriteError), Strings.ErrorMailboxDatabaseNotUnique(this.Identity.ToString()), new LocalizedString?(Strings.ErrorSingleDatabaseCopy(this.Identity.ToString())), out this.m_server);
			Database database = this.DataObject.GetDatabase<Database>();
			if (database != null)
			{
				MapiTaskHelper.VerifyDatabaseIsWithinScope(base.SessionSettings, database, new Task.ErrorLoggerDelegate(base.WriteError));
			}
		}

		internal ReplayConfiguration ConstructReplayConfiguration(Database database)
		{
			IADDatabaseAvailabilityGroup dag = null;
			if (this.m_server.DatabaseAvailabilityGroup != null)
			{
				DatabaseAvailabilityGroup dag2 = this.ConfigurationSession.Read<DatabaseAvailabilityGroup>(this.m_server.DatabaseAvailabilityGroup);
				dag = ADObjectWrapperFactory.CreateWrapper(dag2);
			}
			return RemoteReplayConfiguration.TaskGetReplayConfig(dag, ADObjectWrapperFactory.CreateWrapper(database), ADObjectWrapperFactory.CreateWrapper(this.m_server));
		}

		private DatabaseCopy m_databaseCopy;

		protected Server m_server;
	}
}
