using System;
using System.Threading;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.Cluster;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal static class DatabaseTasks
	{
		internal static string LocalMachineFqdn
		{
			get
			{
				if (string.IsNullOrEmpty(DatabaseTasks.s_localMachineFqdn))
				{
					try
					{
						DatabaseTasks.s_localMachineFqdn = Dependencies.ManagementClassHelper.LocalComputerFqdn;
					}
					catch (CannotGetComputerNameException ex)
					{
						DatabaseTasks.Trace("GetLocalComputerFqdn() threw exception: {0}", new object[]
						{
							ex
						});
						DatabaseTasks.s_localMachineFqdn = Environment.MachineName;
					}
				}
				return DatabaseTasks.s_localMachineFqdn;
			}
		}

		internal static void InitiateMoveForCatalog(IADDatabase database, string from)
		{
			AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Automatic, AmDbActionReason.CatalogFailureItem, AmDbActionCategory.Move);
			DatabaseTasks.Move(database, from, actionCode, DatabaseMountDialOverride.None);
		}

		internal static void Move(IADDatabase database, string from)
		{
			AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Automatic, AmDbActionReason.FailureItem, AmDbActionCategory.Move);
			DatabaseTasks.Move(database, from, actionCode, DatabaseMountDialOverride.None);
		}

		internal static void Move(IADDatabase database, string from, AmDbActionCode actionCode, DatabaseMountDialOverride mountDialOverride)
		{
			Exception ex = null;
			AmDatabaseMoveResult amDatabaseMoveResult = null;
			DatabaseTasks.Trace("Database '{0}' moved from '{1}'", new object[]
			{
				database.Name,
				from
			});
			try
			{
				string text;
				AmRpcClientHelper.MoveDatabaseEx(database, 0, 16, (int)mountDialOverride, from, null, true, 0, actionCode, null, out text, ref amDatabaseMoveResult);
			}
			catch (AmServerException ex2)
			{
				DatabaseTasks.Trace("MoveDatabase() failed with {0}; Exception: {1}", new object[]
				{
					amDatabaseMoveResult,
					ex2
				});
				ex = ex2;
			}
			catch (AmServerTransientException ex3)
			{
				DatabaseTasks.Trace("MoveDatabase() failed with {0}; Exception: {1}", new object[]
				{
					amDatabaseMoveResult,
					ex3
				});
				ex = ex3;
			}
			if (ex != null)
			{
				throw new DatabaseFailoverFailedException(database.Name, ex.ToString(), ex);
			}
		}

		internal static void HandleSourceLogCorruption(IADDatabase database, string from)
		{
			Exception ex = null;
			AmDatabaseMoveResult amDatabaseMoveResult = null;
			DatabaseTasks.Trace("HandleSourceLogCorruption for Database '{0}' from '{1}'", new object[]
			{
				database.Name,
				from
			});
			ReplayEventLogConstants.Tuple_CorruptLogRecoveryIsAttempted.LogEvent(null, new object[]
			{
				database.Name
			});
			try
			{
				AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Automatic, AmDbActionReason.FailureItem, AmDbActionCategory.Move);
				string text;
				AmRpcClientHelper.MoveDatabaseEx(database, 0, 0, RegistryParameters.MaxAutoDatabaseMountDial, from, null, true, 0, actionCode, "HandleSourceLogCorruption", out text, ref amDatabaseMoveResult);
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
				DatabaseTasks.Tracer.TraceError<Exception>(0L, "HandleSourceLogCorruption: Move failed: {0}", ex);
			}
			bool flag = false;
			Exception ex4 = null;
			try
			{
				string text2;
				ActiveManagerCore.GetDatabaseMountStatus(database.Guid, out text2);
				if (!string.IsNullOrEmpty(text2) && !Cluster.StringIEquals(text2, Dependencies.ManagementClassHelper.LocalMachineName))
				{
					flag = true;
				}
			}
			catch (ClusterException ex5)
			{
				ex4 = ex5;
			}
			catch (AmServerException ex6)
			{
				ex4 = ex6;
			}
			if (ex4 != null)
			{
				DatabaseTasks.Tracer.TraceError<Exception>(0L, "HandleSourceLogCorruption: Determine active failed: {0}", ex4);
				if (ex == null)
				{
					ex = ex4;
				}
			}
			if (flag)
			{
				Exception ex7 = DatabaseTasks.ResumeLocalDatabaseCopy(database);
				if (ex7 != null)
				{
					DatabaseTasks.Tracer.TraceError<Exception>(0L, "HandleSourceLogCorruption: Resume failed: {0}", ex7);
					ReplayEventLogConstants.Tuple_ResumeFailedDuringFailureItemProcessing.LogEvent(database.Name, new object[]
					{
						database.Name,
						ex7.Message
					});
				}
			}
			if (ex != null)
			{
				throw new DatabaseLogCorruptRecoveryException(database.Name, ex.ToString(), ex);
			}
		}

		internal static Exception TryToDismountClean(IADDatabase database)
		{
			Exception result = null;
			try
			{
				AmRpcClientHelper.DismountDatabase(database, 0);
			}
			catch (AmServerException ex)
			{
				result = ex;
			}
			catch (AmServerTransientException ex2)
			{
				result = ex2;
			}
			return result;
		}

		internal static void Remount(IADDatabase database, string from)
		{
			Exception ex = null;
			DatabaseTasks.Trace("Database '{0}' is attempting to remount on '{1}'", new object[]
			{
				database.Name,
				from
			});
			try
			{
				DatabaseTasks.WaitUntilDatabaseIsNotMounted(database, 5);
				AmRpcClientHelper.RemountDatabase(database, 0, -1, from);
			}
			catch (AmServerException ex2)
			{
				DatabaseTasks.Trace("RemountDatabase() failed with {0}", new object[]
				{
					ex2
				});
				ex = ex2;
			}
			catch (AmServerTransientException ex3)
			{
				DatabaseTasks.Trace("RemountDatabase() failed with {0}", new object[]
				{
					ex3
				});
				ex = ex3;
			}
			if (ex != null)
			{
				throw new DatabaseRemountFailedException(database.Name, ex.ToString(), ex);
			}
		}

		internal static void WaitUntilDatabaseIsNotMounted(IADDatabase database, int seconds)
		{
			bool flag = true;
			DatabaseTasks.Trace("Waiting for database {0} to reach dismounted/dismounting state for {1} seconds", new object[]
			{
				database.Name,
				seconds
			});
			for (int i = 0; i < seconds; i++)
			{
				if (!AmStoreHelper.IsMounted(null, database.Guid))
				{
					flag = false;
					break;
				}
				Thread.Sleep(1000);
			}
			if (flag)
			{
				DatabaseTasks.Trace("Database is still mounted {0} after {1} seconds", new object[]
				{
					database.Name,
					seconds
				});
			}
		}

		internal static void SuspendLocalDatabaseCopy(IADDatabase database, string suspendMsg)
		{
			string serverNameFqdn = DatabaseTasks.LocalMachineFqdn;
			Exception ex = null;
			try
			{
				Action invokableAction = delegate()
				{
					DatabaseCopyActionFlags flags = DatabaseCopyActionFlags.Replication | DatabaseCopyActionFlags.Activation;
					Dependencies.ReplayRpcClientWrapper.RequestSuspend3(serverNameFqdn, database.Guid, suspendMsg, (uint)flags, 1U);
				};
				InvokeWithTimeout.Invoke(invokableAction, TimeSpan.FromSeconds((double)RegistryParameters.FailureItemLocalDatabaseOperationTimeoutInSec));
			}
			catch (ReplayServiceSuspendCommentException ex2)
			{
				DatabaseTasks.Trace("SuspendLocalDatabaseCopy(): Catching and ignoring exception: {0}", new object[]
				{
					ex2
				});
			}
			catch (TimeoutException ex3)
			{
				DatabaseTasks.Trace("SuspendLocalDatabaseCopy(): Failed on timeout: {0}", new object[]
				{
					ex3
				});
				ex = ex3;
			}
			catch (TaskServerException ex4)
			{
				ex = ex4;
			}
			catch (TaskServerTransientException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				DatabaseTasks.Trace("SuspendLocalDatabaseCopy(): Failed with exception: {0}", new object[]
				{
					ex
				});
				throw new DatabaseCopySuspendException(database.Name, Environment.MachineName, ex.ToString(), ex);
			}
		}

		internal static Exception ResumeLocalDatabaseCopy(IADDatabase database)
		{
			string localMachineFqdn = DatabaseTasks.LocalMachineFqdn;
			Exception ex = null;
			try
			{
				DatabaseCopyActionFlags flags = DatabaseCopyActionFlags.Replication | DatabaseCopyActionFlags.Activation;
				Dependencies.ReplayRpcClientWrapper.RequestResume2(localMachineFqdn, database.Guid, (uint)flags);
			}
			catch (TaskServerException ex2)
			{
				ex = ex2;
			}
			catch (TaskServerTransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				DatabaseTasks.Trace("ResumeLocalDatabaseCopy(): Failed with exception: {0}", new object[]
				{
					ex
				});
				return ex;
			}
			return null;
		}

		internal static Exception SuspendAndFailLocalDatabaseCopy(IADDatabase database, string suspendMsg, string errorMsg, uint errorEventId, bool blockResume, bool blockReseed, bool blockInPlaceReseed)
		{
			Exception ex = null;
			try
			{
				Action invokableAction = delegate()
				{
					ReplicaInstanceManager replicaInstanceManager = Dependencies.ReplayCoreManager.ReplicaInstanceManager;
					replicaInstanceManager.RequestSuspendAndFail(database.Guid, errorEventId, errorMsg, suspendMsg, true, blockResume, blockReseed, blockInPlaceReseed);
				};
				InvokeWithTimeout.Invoke(invokableAction, TimeSpan.FromSeconds((double)RegistryParameters.FailureItemLocalDatabaseOperationTimeoutInSec));
			}
			catch (TimeoutException ex2)
			{
				DatabaseTasks.Trace("SuspendLocalDatabaseCopy(): Failed on timeout: {0}", new object[]
				{
					ex2
				});
				ex = ex2;
			}
			catch (TaskServerException ex3)
			{
				ex = ex3;
			}
			catch (TaskServerTransientException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				DatabaseTasks.Trace("SuspendAndFailLocalDatabaseCopy failed to suspend: {0}", new object[]
				{
					ex
				});
				return new DatabaseCopySuspendException(database.Name, Environment.MachineName, ex.ToString(), ex);
			}
			return null;
		}

		private static void Trace(string formatString, params object[] args)
		{
			if (ExTraceGlobals.FailureItemTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string formatString2 = "[FIMTASK] " + formatString;
				ExTraceGlobals.FailureItemTracer.TraceDebug(0L, formatString2, args);
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.FailureItemTracer;

		private static string s_localMachineFqdn;
	}
}
