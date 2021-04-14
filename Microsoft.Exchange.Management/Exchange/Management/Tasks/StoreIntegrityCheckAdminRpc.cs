using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.Tasks
{
	public class StoreIntegrityCheckAdminRpc
	{
		internal static StoreIntegrityCheckJob CreateStoreIntegrityCheckJob(Database database, Guid mailboxGuid, StoreIntegrityCheckRequestFlags flags, MailboxCorruptionType[] taskIds, Task.TaskErrorLoggingDelegate writeError, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskVerboseLoggingDelegate writeVerbose)
		{
			List<StoreIntegrityCheckJob> result = null;
			try
			{
				StoreIntegrityCheckAdminRpc.ExecuteAdminRpc(database.Guid, delegate(ExRpcAdmin rpcAdmin, string serverFqdn)
				{
					uint[] array = new uint[taskIds.Length];
					for (int i = 0; i < taskIds.Length; i++)
					{
						array[i] = (uint)taskIds[i];
					}
					int num;
					int num2;
					rpcAdmin.GetAdminVersion(out num, out num2);
					if (num < 7 || (num == 7 && num2 < 15))
					{
						string input;
						rpcAdmin.ISIntegCheck(database.Guid, mailboxGuid, (uint)flags, array.Length, array, out input);
						JobFlags jobFlags = JobFlags.None;
						if ((flags & StoreIntegrityCheckRequestFlags.DetectOnly) == StoreIntegrityCheckRequestFlags.DetectOnly)
						{
							jobFlags |= JobFlags.DetectOnly;
						}
						if ((flags & (StoreIntegrityCheckRequestFlags)2147483648U) == (StoreIntegrityCheckRequestFlags)2147483648U)
						{
							jobFlags |= (JobFlags)2147483648U;
						}
						if ((flags & StoreIntegrityCheckRequestFlags.Force) == StoreIntegrityCheckRequestFlags.None)
						{
							jobFlags |= JobFlags.Background;
						}
						StoreIntegrityCheckJob item = new StoreIntegrityCheckJob(new DatabaseId(null, serverFqdn, database.Name, database.Guid), Guid.Parse(input), jobFlags, (from x in array
						select (MailboxCorruptionType)x).ToArray<MailboxCorruptionType>());
						result = new List<StoreIntegrityCheckJob>();
						result.Add(item);
						return;
					}
					PropValue[][] array2 = rpcAdmin.StoreIntegrityCheckEx(database.Guid, mailboxGuid, Guid.Empty, 1U, (uint)flags, array, StoreIntegrityCheckAdminRpc.JobPropTags);
					result = new List<StoreIntegrityCheckJob>(array2.Length);
					foreach (PropValue[] propValues in array2)
					{
						StoreIntegrityCheckJob item2 = new StoreIntegrityCheckJob(new DatabaseId(null, serverFqdn, database.Name, database.Guid), propValues);
						result.Add(item2);
					}
				}, writeError, writeWarning, writeVerbose);
			}
			catch (MapiExceptionNetworkError innerException)
			{
				writeError(new OnlineIsIntegException(database.Identity.ToString(), Strings.ServiceUnavaiable, innerException), ErrorCategory.ResourceUnavailable, database.Identity);
			}
			catch (MapiExceptionMdbOffline innerException2)
			{
				writeError(new OnlineIsIntegException(database.Identity.ToString(), Strings.DatabaseOffline, innerException2), ErrorCategory.InvalidOperation, database.Identity);
			}
			catch (MapiExceptionISIntegMdbTaskExceeded innerException3)
			{
				writeError(new OnlineIsIntegException(database.Identity.ToString(), Strings.DatabaseWideTasksExceeded, innerException3), ErrorCategory.InvalidOperation, database.Identity);
			}
			catch (MapiExceptionISIntegQueueFull innerException4)
			{
				writeError(new OnlineIsIntegException(database.Identity.ToString(), Strings.IsIntegQueueFull, innerException4), ErrorCategory.InvalidOperation, database.Identity);
			}
			catch (MapiExceptionUnknownUser)
			{
				writeWarning(Strings.WarningMailboxNeverBeenLoggedOn(mailboxGuid.ToString(), database.Identity.ToString()));
			}
			catch (MapiPermanentException ex)
			{
				writeError(new OnlineIsIntegException(database.Identity.ToString(), Strings.UnexpectedError(ex.ToString()), ex), ErrorCategory.InvalidOperation, database.Identity);
			}
			catch (MapiRetryableException ex2)
			{
				writeError(new OnlineIsIntegException(database.Identity.ToString(), Strings.UnexpectedError(ex2.ToString()), ex2), ErrorCategory.InvalidOperation, database.Identity);
			}
			return StoreIntegrityCheckJob.Aggregate(result);
		}

		internal static List<StoreIntegrityCheckJob> GetStoreIntegrityCheckJob(Database database, Guid mailboxGuid, Guid requestGuid, IntegrityCheckQueryFlags flags, bool details, Task.TaskErrorLoggingDelegate writeError, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskVerboseLoggingDelegate writeVerbose)
		{
			List<StoreIntegrityCheckJob> jobs = new List<StoreIntegrityCheckJob>();
			try
			{
				StoreIntegrityCheckAdminRpc.ExecuteAdminRpc(database.Guid, delegate(ExRpcAdmin rpcAdmin, string serverFqdn)
				{
					int num;
					int num2;
					rpcAdmin.GetAdminVersion(out num, out num2);
					if (num < 7 || (num == 7 && num2 < 15))
					{
						throw new NotSupportedException();
					}
					PropValue[][] array = rpcAdmin.StoreIntegrityCheckEx(database.Guid, mailboxGuid, requestGuid, 2U, (uint)flags, null, StoreIntegrityCheckAdminRpc.JobPropTags);
					foreach (PropValue[] propValues in array)
					{
						jobs.Add(new StoreIntegrityCheckJob(new DatabaseId(null, serverFqdn, database.Name, database.Guid), propValues));
					}
				}, writeError, writeWarning, writeVerbose);
			}
			catch (MapiExceptionNetworkError innerException)
			{
				writeError(new OnlineIsIntegQueryJobException(database.Identity.ToString(), Strings.ServiceUnavaiable, innerException), ErrorCategory.ResourceUnavailable, database.Identity);
			}
			catch (MapiExceptionMdbOffline innerException2)
			{
				writeError(new OnlineIsIntegQueryJobException(database.Identity.ToString(), Strings.DatabaseOffline, innerException2), ErrorCategory.InvalidOperation, database.Identity);
			}
			catch (MapiPermanentException ex)
			{
				writeError(new OnlineIsIntegQueryJobException(database.Identity.ToString(), Strings.UnexpectedError(ex.ToString()), ex), ErrorCategory.InvalidOperation, database.Identity);
			}
			catch (MapiRetryableException ex2)
			{
				writeError(new OnlineIsIntegQueryJobException(database.Identity.ToString(), Strings.UnexpectedError(ex2.ToString()), ex2), ErrorCategory.InvalidOperation, database.Identity);
			}
			if (details)
			{
				return jobs;
			}
			return new List<StoreIntegrityCheckJob>
			{
				StoreIntegrityCheckJob.Aggregate(jobs)
			};
		}

		internal static void RemoveStoreIntegrityCheckJob(Database database, Guid jobGuid, Task.TaskErrorLoggingDelegate writeError, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskVerboseLoggingDelegate writeVerbose)
		{
			try
			{
				StoreIntegrityCheckAdminRpc.ExecuteAdminRpc(database.Guid, delegate(ExRpcAdmin rpcAdmin, string serverFqdn)
				{
					int num;
					int num2;
					rpcAdmin.GetAdminVersion(out num, out num2);
					if (num < 7 || (num == 7 && num2 < 15))
					{
						throw new NotSupportedException();
					}
					rpcAdmin.StoreIntegrityCheckEx(database.Guid, Guid.Empty, jobGuid, 3U, 0U, null, null);
				}, writeError, writeWarning, writeVerbose);
			}
			catch (MapiExceptionNetworkError innerException)
			{
				writeError(new OnlineIsIntegRemoveJobException(database.Identity.ToString(), jobGuid.ToString(), Strings.ServiceUnavaiable, innerException), ErrorCategory.ResourceUnavailable, database.Identity);
			}
			catch (MapiExceptionMdbOffline innerException2)
			{
				writeError(new OnlineIsIntegRemoveJobException(database.Identity.ToString(), jobGuid.ToString(), Strings.DatabaseOffline, innerException2), ErrorCategory.InvalidOperation, database.Identity);
			}
			catch (MapiPermanentException ex)
			{
				writeError(new OnlineIsIntegRemoveJobException(database.Identity.ToString(), jobGuid.ToString(), Strings.UnexpectedError(ex.ToString()), ex), ErrorCategory.InvalidOperation, database.Identity);
			}
			catch (MapiRetryableException ex2)
			{
				writeError(new OnlineIsIntegRemoveJobException(database.Identity.ToString(), jobGuid.ToString(), Strings.UnexpectedError(ex2.ToString()), ex2), ErrorCategory.InvalidOperation, database.Identity);
			}
		}

		private static void ExecuteAdminRpc(Guid databaseGuid, Action<ExRpcAdmin, string> action, Task.TaskErrorLoggingDelegate writeError, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskVerboseLoggingDelegate writeVerbose)
		{
			ActiveManager activeManagerInstance = ActiveManager.GetActiveManagerInstance();
			DatabaseLocationInfo serverForDatabase = activeManagerInstance.GetServerForDatabase(databaseGuid);
			string serverFqdn = serverForDatabase.ServerFqdn;
			using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=Management", serverFqdn, null, null, null))
			{
				action(exRpcAdmin, serverFqdn);
			}
		}

		private static readonly PropTag[] JobPropTags = new PropTag[]
		{
			PropTag.IsIntegJobRequestGuid,
			PropTag.IsIntegJobGuid,
			PropTag.IsIntegJobMailboxGuid,
			PropTag.IsIntegJobFlags,
			PropTag.IsIntegJobTask,
			PropTag.IsIntegJobCreationTime,
			PropTag.IsIntegJobState,
			PropTag.IsIntegJobProgress,
			PropTag.IsIntegJobSource,
			PropTag.IsIntegJobPriority,
			PropTag.IsIntegJobTimeInServer,
			PropTag.IsIntegJobFinishTime,
			PropTag.IsIntegJobLastExecutionTime,
			PropTag.IsIntegJobCorruptionsDetected,
			PropTag.IsIntegJobCorruptionsFixed,
			PropTag.IsIntegJobCorruptions,
			PropTag.RtfSyncTrailingCount
		};
	}
}
