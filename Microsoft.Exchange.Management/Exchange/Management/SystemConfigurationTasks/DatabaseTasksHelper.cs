using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class DatabaseTasksHelper
	{
		public static void ValidateDatabaseCopyActionTask(DatabaseCopy databaseCopy, bool allowInvalidDbCopy, bool fCheckDbReplicated, IConfigDataProvider session, ObjectId rootId, Task.TaskErrorLoggingDelegate writeError, LocalizedString mdbNotUniqueError, LocalizedString? singleDbCopyError, out Server server)
		{
			string databaseName = databaseCopy.DatabaseName;
			server = null;
			Database database = databaseCopy.GetDatabase<Database>();
			DatabaseTasksHelper.CheckDatabaseForCopyTaskImpl(database, fCheckDbReplicated, writeError, singleDbCopyError);
			if (database.Recovery)
			{
				writeError(new InvalidOperationException(Strings.ErrorInvalidOperationOnRecoveryMailboxDatabase(database.Name)), ErrorCategory.InvalidOperation, database.Identity);
			}
			if (databaseCopy.HostServer != null)
			{
				MailboxServerIdParameter serverIdParam = new MailboxServerIdParameter(databaseCopy.HostServer);
				server = databaseCopy.Session.Read<Server>(databaseCopy.HostServer);
				DatabaseTasksHelper.CheckServerObjectForCopyTask(serverIdParam, writeError, server);
				return;
			}
			if (!allowInvalidDbCopy && !databaseCopy.IsHostServerValid)
			{
				writeError(new InvalidOperationException(Strings.ErrorDbCopyHostServerInvalid(databaseCopy.Name)), ErrorCategory.InvalidData, databaseCopy);
			}
		}

		public static void CheckDatabaseCopyForCopyTask(Database database, Task.TaskErrorLoggingDelegate writeError, Server server, out DatabaseCopy databaseCopy, out DatabaseCopy[] databaseCopies)
		{
			databaseCopy = null;
			databaseCopies = database.GetDatabaseCopies();
			if (databaseCopies == null || databaseCopies.Length == 0)
			{
				writeError(new CopyConfigurationErrorException(Strings.ErrorCouldNotReadDatabaseCopy(database.Name)), ErrorCategory.ReadError, database.Identity);
			}
			else
			{
				foreach (DatabaseCopy databaseCopy2 in databaseCopies)
				{
					if (databaseCopy2.HostServer.ObjectGuid == server.Guid)
					{
						databaseCopy = databaseCopy2;
						break;
					}
				}
			}
			if (databaseCopy == null && database.ReplicationType == ReplicationType.Remote)
			{
				writeError(new InvalidOperationException(Strings.ErrorDbCopyNotHostedOnServer(database.Identity.ToString(), server.Identity.ToString())), ErrorCategory.InvalidData, database.Identity);
			}
		}

		public static void CheckServerObjectForCopyTask(IIdentityParameter serverIdParam, Task.TaskErrorLoggingDelegate writeError, Server server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			if (!server.IsMailboxServer)
			{
				writeError(server.GetServerRoleError(ServerRole.Mailbox), ErrorCategory.InvalidOperation, serverIdParam);
			}
			if (!server.IsE14OrLater)
			{
				writeError(new InvalidOperationException(Strings.ErrorServerNotE14OrLater(server.Name)), ErrorCategory.InvalidOperation, serverIdParam);
			}
		}

		public static void CheckDatabaseForCopyTask(Database database, Task.TaskErrorLoggingDelegate writeError, LocalizedString singleDbCopyError)
		{
			DatabaseTasksHelper.CheckDatabaseForCopyTaskImpl(database, true, writeError, new LocalizedString?(singleDbCopyError));
		}

		public static void RunConfigurationUpdaterRpc(string serverFqdn, Database database, ServerVersion serverVersion, ReplayConfigChangeHints changeHint, Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskWarningLoggingDelegate writeWarning)
		{
			string name = database.Name;
			if (ReplayRpcVersionControl.IsRunConfigUpdaterRpcSupported(serverVersion))
			{
				writeVerbose(Strings.RunConfigUpdaterRpcVerbose(serverFqdn, name));
				try
				{
					ReplayRpcClientHelper.NotifyChangedReplayConfiguration(serverFqdn, database.Guid, serverVersion, true, false, changeHint);
				}
				catch (TaskServerTransientException ex)
				{
					writeWarning(Strings.RunConfigUpdaterRpcFailedWarning(serverFqdn, name, ex.Message));
				}
				catch (TaskServerException ex2)
				{
					writeWarning(Strings.RunConfigUpdaterRpcFailedWarning(serverFqdn, name, ex2.Message));
				}
			}
		}

		public static void RunConfigurationUpdaterRpcAsync(string serverFqdn, Database database, ReplayConfigChangeHints changeHint, Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskWarningLoggingDelegate writeWarning)
		{
			try
			{
				ReplayRpcClientHelper.NotifyChangedReplayConfigurationAsync(serverFqdn, database.Guid, changeHint);
			}
			catch (TaskServerTransientException ex)
			{
				writeWarning(Strings.RunConfigUpdaterRpcFailedWarning(serverFqdn, database.Name, ex.Message));
			}
			catch (TaskServerException ex2)
			{
				writeWarning(Strings.RunConfigUpdaterRpcFailedWarning(serverFqdn, database.Name, ex2.Message));
			}
		}

		private static void CheckDatabaseForCopyTaskImpl(Database database, bool fCheckDbReplicated, Task.TaskErrorLoggingDelegate writeError, LocalizedString? singleDbCopyError)
		{
			if (!database.IsExchange2009OrLater)
			{
				writeError(new ErrorDatabaseWrongVersion(database.Name), ErrorCategory.InvalidOperation, database.Identity);
			}
			if (fCheckDbReplicated && database.ReplicationType == ReplicationType.None)
			{
				LocalizedString? localizedString = singleDbCopyError;
				writeError(new InvalidOperationException((localizedString != null) ? localizedString.GetValueOrDefault() : null), ErrorCategory.InvalidOperation, database.Identity);
			}
		}

		public static void CheckReplayServiceRunningOnNode(AmServerName nodeName, Task.TaskErrorLoggingDelegate writeError)
		{
			if (!DatabaseTasksHelper.IsServiceRunningOnNode("msexchangerepl", nodeName))
			{
				writeError(new ReplServiceNotRunningOnNodeException(nodeName.NetbiosName), ErrorCategory.InvalidOperation, null);
			}
		}

		private static bool IsServiceRunningOnNode(string serviceName, AmServerName nodeName)
		{
			Exception ex;
			bool result = ServiceOperations.IsServiceRunningOnNode(serviceName, nodeName.Fqdn, out ex);
			if (ex != null)
			{
				ExTraceGlobals.CmdletsTracer.TraceError<string, AmServerName, Exception>(0L, "IsServiceRunningOnNode( {0}, {1} ): Caught exception {2}", serviceName, nodeName, ex);
			}
			return result;
		}

		private static int GetNumberOfDatacenters(ITopologyConfigurationSession taskSession, Database database)
		{
			HashSet<ADObjectId> hashSet = new HashSet<ADObjectId>();
			DatabaseCopy[] databaseCopies = database.GetDatabaseCopies();
			foreach (DatabaseCopy databaseCopy in databaseCopies)
			{
				MiniServer miniServer = taskSession.ReadMiniServer(databaseCopy.HostServer, DatabaseTasksHelper.s_propertiesNeededFromServer);
				if (miniServer.ServerSite != null)
				{
					hashSet.Add(miniServer.ServerSite);
				}
			}
			return hashSet.Count;
		}

		public static void DataMoveReplicationConstraintFallBack(ITopologyConfigurationSession taskSession, Database database, DataMoveReplicationConstraintParameter constraint, out DataMoveReplicationConstraintParameter suggestedConstraint)
		{
			suggestedConstraint = constraint;
			switch (constraint)
			{
			case DataMoveReplicationConstraintParameter.None:
			case (DataMoveReplicationConstraintParameter)2:
				break;
			case DataMoveReplicationConstraintParameter.SecondCopy:
			case DataMoveReplicationConstraintParameter.AllCopies:
				if (database.ReplicationType == ReplicationType.None)
				{
					suggestedConstraint = DataMoveReplicationConstraintParameter.None;
					return;
				}
				break;
			case DataMoveReplicationConstraintParameter.SecondDatacenter:
			case DataMoveReplicationConstraintParameter.AllDatacenters:
				if (database.ReplicationType == ReplicationType.None)
				{
					suggestedConstraint = DataMoveReplicationConstraintParameter.None;
					return;
				}
				if (DatabaseTasksHelper.GetNumberOfDatacenters(taskSession, database) < 2)
				{
					suggestedConstraint = DataMoveReplicationConstraintParameter.SecondCopy;
					return;
				}
				break;
			default:
				switch (constraint)
				{
				case DataMoveReplicationConstraintParameter.CINoReplication:
				case (DataMoveReplicationConstraintParameter)65538:
					break;
				case DataMoveReplicationConstraintParameter.CISecondCopy:
				case DataMoveReplicationConstraintParameter.CIAllCopies:
					if (database.ReplicationType == ReplicationType.None)
					{
						suggestedConstraint = DataMoveReplicationConstraintParameter.CINoReplication;
						return;
					}
					break;
				case DataMoveReplicationConstraintParameter.CISecondDatacenter:
				case DataMoveReplicationConstraintParameter.CIAllDatacenters:
					if (database.ReplicationType == ReplicationType.None)
					{
						suggestedConstraint = DataMoveReplicationConstraintParameter.CINoReplication;
						return;
					}
					if (DatabaseTasksHelper.GetNumberOfDatacenters(taskSession, database) < 2)
					{
						suggestedConstraint = DataMoveReplicationConstraintParameter.CISecondCopy;
					}
					break;
				default:
					return;
				}
				break;
			}
		}

		public static void UpdateDataGuaranteeConstraint(ITopologyConfigurationSession taskSession, Database database, Task.TaskWarningLoggingDelegate writeWarning)
		{
			DataMoveReplicationConstraintParameter dataMoveReplicationConstraint = database.DataMoveReplicationConstraint;
			DatabaseTasksHelper.DataMoveReplicationConstraintFallBack(taskSession, database, database.DataMoveReplicationConstraint, out dataMoveReplicationConstraint);
			if (dataMoveReplicationConstraint != database.DataMoveReplicationConstraint)
			{
				writeWarning(Strings.ConstraintFallback(database.DataMoveReplicationConstraint, dataMoveReplicationConstraint, database.Identity.ToString()));
			}
			database.DataMoveReplicationConstraint = dataMoveReplicationConstraint;
			taskSession.Save(database);
		}

		internal static Server GetServerObject(ServerIdParameter serverIdParameter, IConfigurationSession dataSession, ObjectId rootId, DataAccessHelper.GetDataObjectDelegate getDataObjectServer)
		{
			return (Server)getDataObjectServer(serverIdParameter, dataSession, rootId, null, new LocalizedString?(Strings.ErrorServerNotFound(serverIdParameter.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter.ToString())));
		}

		public static bool IsMailboxDatabaseExcludedFromMonitoring(MailboxDatabase database)
		{
			return database.Recovery || (Datacenter.IsMicrosoftHostedOnly(true) && !ExEnvironment.IsTest && database.IsExcludedFromProvisioning && database.ReplicationType != ReplicationType.Remote);
		}

		public static IClusterDB OpenClusterDatabase(ITopologyConfigurationSession taskSession, Task.TaskVerboseLoggingDelegate writeLog, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskErrorLoggingDelegate writeError, Database database, bool allowNoOwningServer, out DatabaseAvailabilityGroup dag)
		{
			string machineName = database.Server.ToString();
			try
			{
				ADObjectId server = database.Server;
				MiniServer miniServer = taskSession.ReadMiniServer(server, DatabaseTasksHelper.MiniserverProperties);
				if (miniServer == null)
				{
					writeWarning(Strings.ErrorDBOwningServerNotFound(database.Identity.ToString()));
					if (allowNoOwningServer)
					{
						dag = null;
						return null;
					}
					writeError(new InvalidOperationException(Strings.ErrorDBOwningServerNotFound(database.Name)), ErrorCategory.InvalidOperation, database);
				}
				machineName = miniServer.Fqdn;
				ADObjectId databaseAvailabilityGroup = miniServer.DatabaseAvailabilityGroup;
				if (databaseAvailabilityGroup != null)
				{
					dag = DagTaskHelper.ReadDag(databaseAvailabilityGroup, taskSession);
					if (dag != null)
					{
						AmServerName amServerName = new AmServerName(miniServer);
						writeLog(Strings.OpeningClusterDatabase(amServerName.ToString()));
						return ClusterDB.Open(amServerName);
					}
				}
			}
			catch (ClusterException ex)
			{
				writeWarning(Strings.CouldNotConnectToClusterError(machineName, ex.Message));
			}
			catch (DataSourceTransientException)
			{
				writeWarning(Strings.CouldNotConnectToCluster(machineName));
			}
			catch (DataSourceOperationException)
			{
				writeWarning(Strings.CouldNotConnectToCluster(machineName));
			}
			dag = null;
			return null;
		}

		public static int GetMaximumSupportedDatabaseSchemaVersion(ITopologyConfigurationSession taskSession, Task.TaskVerboseLoggingDelegate writeLog, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskErrorLoggingDelegate writeError, Database database)
		{
			int num;
			int result;
			int num2;
			DatabaseTasksHelper.GetSupporableDatabaseSchemaVersionRange(taskSession, writeLog, writeWarning, writeError, database, out num, out result, out num2);
			return result;
		}

		public static void GetSupporableDatabaseSchemaVersionRange(ITopologyConfigurationSession taskSession, Task.TaskVerboseLoggingDelegate writeLog, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskErrorLoggingDelegate writeError, Database database, out int minVersion, out int maxVersion, out int currentRequestedVersion)
		{
			currentRequestedVersion = 0;
			int num = int.MaxValue;
			int num2 = 0;
			DatabaseAvailabilityGroup databaseAvailabilityGroup;
			using (IClusterDB clusterDB = DatabaseTasksHelper.OpenClusterDatabase(taskSession, writeLog, writeWarning, writeError, database, false, out databaseAvailabilityGroup))
			{
				if (clusterDB != null && clusterDB.IsInitialized)
				{
					ClusterDBHelpers.ReadRequestedDatabaseSchemaVersion(clusterDB, database.Guid, 0, out currentRequestedVersion);
					foreach (ADObjectId adobjectId in databaseAvailabilityGroup.Servers)
					{
						int num3;
						int num4;
						ClusterDBHelpers.ReadServerDatabaseSchemaVersionRange(clusterDB, adobjectId.ObjectGuid, 121, 121, out num3, out num4);
						writeLog(Strings.ServerSchemaVersionRange(adobjectId.ObjectGuid.ToString(), num3, num4));
						num2 = Math.Max(num3, num2);
						num = Math.Min(num4, num);
					}
				}
			}
			if (num2 > num)
			{
				writeError(new IncompatibleDatabaseSchemaVersionsInDAGException(), ErrorCategory.MetadataError, database);
			}
			minVersion = num2;
			maxVersion = num;
		}

		public static void SetRequestedDatabaseSchemaVersion(ITopologyConfigurationSession taskSession, Task.TaskVerboseLoggingDelegate writeLog, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskErrorLoggingDelegate writeError, Database database, int version)
		{
			try
			{
				DatabaseAvailabilityGroup databaseAvailabilityGroup;
				using (IClusterDB clusterDB = DatabaseTasksHelper.OpenClusterDatabase(taskSession, writeLog, writeWarning, writeError, database, false, out databaseAvailabilityGroup))
				{
					if (clusterDB != null && clusterDB.IsInitialized)
					{
						writeLog(Strings.SetDatabaseRequestedSchemaVersion(database.Name, version));
						ClusterDBHelpers.WriteRequestedDatabaseSchemaVersion(clusterDB, database.Guid, version);
					}
				}
			}
			catch (ClusterException)
			{
				if (writeError != null)
				{
					writeError(new FailedToSetRequestedDatabaseSchemaVersionException(database.Name), ErrorCategory.WriteError, database);
				}
			}
		}

		public static void RemoveDatabaseFromClusterDB(ITopologyConfigurationSession taskSession, Task.TaskVerboseLoggingDelegate writeLog, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskErrorLoggingDelegate writeError, Database database)
		{
			try
			{
				DatabaseAvailabilityGroup databaseAvailabilityGroup;
				using (IClusterDB clusterDB = DatabaseTasksHelper.OpenClusterDatabase(taskSession, writeLog, writeWarning, writeError, database, true, out databaseAvailabilityGroup))
				{
					if (clusterDB != null && clusterDB.IsInitialized)
					{
						writeLog(Strings.DeleteClusterDBKey(database.Name));
						ClusterDBHelpers.RemoveDatabaseRequestedSchemaVersion(clusterDB, database.Guid);
					}
				}
			}
			catch (ClusterException)
			{
				writeWarning(Strings.FailedToRemoveDatabaseSection(database.Name));
			}
		}

		public const string MailboxLocationAttributeName = "Location";

		private static readonly PropertyDefinition[] s_propertiesNeededFromServer = new PropertyDefinition[]
		{
			ServerSchema.ServerSite
		};

		private static readonly PropertyDefinition[] MiniserverProperties = new PropertyDefinition[]
		{
			ServerSchema.Fqdn,
			ServerSchema.DatabaseAvailabilityGroup
		};
	}
}
