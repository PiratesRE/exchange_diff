using System;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.ThirdPartyReplication;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal static class AmHelper
	{
		public static Exception RunAmClusterOperation(Action codeToRun)
		{
			Exception result = null;
			try
			{
				codeToRun();
			}
			catch (ClusterException ex)
			{
				result = ex;
			}
			catch (AmServerException ex2)
			{
				result = ex2;
			}
			catch (AmServerTransientException ex3)
			{
				result = ex3;
			}
			return result;
		}

		internal static bool SleepUntilShutdown(TimeSpan ts)
		{
			return AmSystemManager.Instance.ShutdownEvent.WaitOne(ts, false);
		}

		internal static bool SleepUntilShutdown(TimeSpan ts, bool throwIfShuttingdown)
		{
			bool flag = AmHelper.SleepUntilShutdown(ts);
			if (flag && throwIfShuttingdown)
			{
				throw new AmServiceShuttingDownException();
			}
			return flag;
		}

		internal static void ThrowDbActionWrapperExceptionIfNecessary(Exception exception)
		{
			if (exception == null)
			{
				return;
			}
			string dbActionError = string.Empty;
			IHaRpcServerBaseException ex = exception as IHaRpcServerBaseException;
			if (ex != null)
			{
				dbActionError = ex.ErrorMessage;
			}
			else
			{
				dbActionError = exception.Message;
			}
			if (exception is TransientException)
			{
				throw new AmDbActionWrapperTransientException(dbActionError, exception);
			}
			throw new AmDbActionWrapperException(dbActionError, exception);
		}

		internal static Exception HandleKnownExceptions(EventHandler ev)
		{
			Exception result = null;
			try
			{
				ev(null, null);
			}
			catch (ThirdPartyReplicationException ex)
			{
				result = ex;
			}
			catch (AmServiceShuttingDownException ex2)
			{
				result = ex2;
			}
			catch (MapiPermanentException ex3)
			{
				result = ex3;
			}
			catch (MapiRetryableException ex4)
			{
				result = ex4;
			}
			catch (DataSourceTransientException ex5)
			{
				result = ex5;
			}
			catch (DataSourceOperationException ex6)
			{
				result = ex6;
			}
			catch (AmCommonException ex7)
			{
				result = ex7;
			}
			catch (AmServerException ex8)
			{
				result = ex8;
			}
			catch (MonitoringADConfigException ex9)
			{
				result = ex9;
			}
			catch (ClusterException ex10)
			{
				result = ex10;
			}
			catch (OperationAbortedException ex11)
			{
				result = ex11;
			}
			catch (TransientException ex12)
			{
				result = ex12;
			}
			catch (Win32Exception ex13)
			{
				result = ex13;
			}
			catch (IOException ex14)
			{
				result = ex14;
			}
			catch (TimeoutException ex15)
			{
				result = ex15;
			}
			return result;
		}

		internal static ServerVersion GetServerVersion(AmServerName serverName)
		{
			Exception ex;
			IADServer miniServer = AmBestCopySelectionHelper.GetMiniServer(serverName, out ex);
			if (miniServer == null)
			{
				throw ex;
			}
			return miniServer.AdminDisplayVersion;
		}

		internal static IADDatabase FindDatabaseByGuid(Guid dbGuid)
		{
			return AmHelper.FindDatabaseByGuid(dbGuid, true);
		}

		internal static IADDatabase FindDatabaseByGuid(Guid dbGuid, bool throwOnError)
		{
			Exception ex;
			return AmHelper.FindDatabaseByGuid(dbGuid, throwOnError, out ex);
		}

		internal static IADDatabase FindDatabaseByGuid(Guid dbGuid, bool throwOnError, out Exception exception)
		{
			IADDatabase iaddatabase = null;
			exception = null;
			try
			{
				iaddatabase = Dependencies.ReplayAdObjectLookup.DatabaseLookup.FindAdObjectByGuidEx(dbGuid, AdObjectLookupFlags.ReadThrough);
			}
			catch (ADTransientException ex)
			{
				AmTrace.Error("FindDatabaseByGuid(): ADTransientException occurred for {0} (error={1})", new object[]
				{
					dbGuid,
					ex
				});
				exception = ex;
			}
			catch (ADExternalException ex2)
			{
				AmTrace.Error("FindDatabaseByGuid(): ADExternalException occurred for {0} (error={1})", new object[]
				{
					dbGuid,
					ex2
				});
				exception = ex2;
			}
			catch (ADOperationException ex3)
			{
				AmTrace.Error("FindDatabaseByGuid(): ADOperationException occurred for {0} (error={1})", new object[]
				{
					dbGuid,
					ex3
				});
				exception = ex3;
			}
			if (iaddatabase == null)
			{
				AmTrace.Error("Database object not found in AD {0}", new object[]
				{
					dbGuid
				});
				if (throwOnError)
				{
					throw new AmDatabaseNotFoundException(dbGuid, exception);
				}
			}
			return iaddatabase;
		}

		internal static IADDatabase FindDatabaseByName(string databaseName)
		{
			return AmHelper.FindDatabaseByName(databaseName, true);
		}

		internal static IADDatabase FindDatabaseByName(string databaseName, bool throwOnError)
		{
			Database database = null;
			Exception innerException = null;
			try
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 347, "FindDatabaseByName", "f:\\15.00.1497\\sources\\dev\\cluster\\src\\Replay\\ActiveManager\\Util\\AmHelperMethods.cs");
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DatabaseSchema.Name, databaseName);
				Database[] array = tenantOrTopologyConfigurationSession.Find<Database>(null, QueryScope.SubTree, filter, null, 1);
				if (array != null && array.Length > 0)
				{
					database = array[0];
				}
			}
			catch (ADTransientException ex)
			{
				AmTrace.Error("FindDatabaseByName(): ADTransientException occurred for {0} (error={1})", new object[]
				{
					databaseName,
					ex
				});
				innerException = ex;
			}
			catch (ADExternalException ex2)
			{
				AmTrace.Error("FindDatabaseByName(): ADExternalException occurred for {0} (error={1})", new object[]
				{
					databaseName,
					ex2
				});
				innerException = ex2;
			}
			catch (ADOperationException ex3)
			{
				AmTrace.Error("FindDatabaseByName(): ADOperationException occurred for {0} (error={1})", new object[]
				{
					databaseName,
					ex3
				});
				innerException = ex3;
			}
			if (database == null)
			{
				AmTrace.Error("Database object not found in AD {0}", new object[]
				{
					databaseName
				});
				if (throwOnError)
				{
					throw new AmDatabaseNameNotFoundException(databaseName, innerException);
				}
			}
			return ADObjectWrapperFactory.CreateWrapper(database);
		}

		internal static bool IsDatabaseRcrEnabled(Guid mdbGuid)
		{
			IADDatabase iaddatabase = AmHelper.FindDatabaseByGuid(mdbGuid);
			return iaddatabase.ReplicationType == ReplicationType.Remote;
		}

		internal static bool IsReplayRunning(AmServerName serverName)
		{
			bool result = false;
			try
			{
				result = Dependencies.AmRpcClientWrapper.IsReplayRunning(serverName.Fqdn);
			}
			catch (AmServerException ex)
			{
				AmTrace.Error("IsReplayRunning() failed with {0}", new object[]
				{
					ex
				});
			}
			catch (AmServerTransientException ex2)
			{
				AmTrace.Error("IsReplayRunning() failed with {0}", new object[]
				{
					ex2
				});
			}
			return result;
		}

		internal static bool IsReplayRunning(string serverFqdn)
		{
			return AmHelper.IsReplayRunning(new AmServerName(serverFqdn));
		}

		internal static string ServerLegacyDnFromDatabaseLegacyDn(string databaseLegDn)
		{
			Match match = AmHelper.s_regexServerLegacyDnFromDatabaseLegacyDn.Match(databaseLegDn);
			if (match.Groups.Count < 1)
			{
				return null;
			}
			return match.Groups[1].Value;
		}

		internal static void GenerateStoreHardFailureItem(Guid dbGuid)
		{
			new DatabaseFailureItem(FailureNameSpace.Store, FailureTag.Hard, dbGuid)
			{
				InstanceName = dbGuid.ToString()
			}.Publish(true);
		}

		internal static EventRecord FindEvent(string logName, int eventId, long occuredBeforeMs)
		{
			string query = string.Concat(new string[]
			{
				"*[System[(EventID = ",
				eventId.ToString(),
				") and (TimeCreated[timediff(@SystemTime) <= ",
				occuredBeforeMs.ToString(),
				"])]]"
			});
			EventLogQuery eventQuery = new EventLogQuery(logName, PathType.LogName, query);
			EventRecord eventRecord = null;
			try
			{
				using (EventLogReader eventLogReader = new EventLogReader(eventQuery))
				{
					eventRecord = eventLogReader.ReadEvent();
					if (eventRecord == null)
					{
						AmTrace.Warning("Failed to find an event. (logname={0}, eventId={1}, occuredBefore={2})", new object[]
						{
							logName,
							eventId,
							occuredBeforeMs
						});
					}
					else
					{
						AmTrace.Debug("Found event (logname={0}, eventId={1}, occuredBefore={2})", new object[]
						{
							logName,
							eventId,
							occuredBeforeMs
						});
					}
				}
			}
			catch (EventLogException ex)
			{
				AmTrace.Error("FindEvent caught {0}", new object[]
				{
					ex
				});
			}
			return eventRecord;
		}

		internal static DateTime GetLocalBootTime()
		{
			DateTime bootTimeWithWmi;
			using (ManagementClass managementClass = new ManagementClass("Win32_OperatingSystem"))
			{
				bootTimeWithWmi = AmHelper.GetBootTimeWithWmi(managementClass, AmServerName.LocalComputerName);
			}
			return bootTimeWithWmi;
		}

		internal static DateTime GetBootTime(AmServerName machineName)
		{
			System.Management.ManagementScope managementScope = AmHelper.GetManagementScope(machineName);
			ManagementPath path = new ManagementPath("Win32_OperatingSystem");
			ObjectGetOptions options = null;
			DateTime bootTimeWithWmi;
			using (ManagementClass managementClass = new ManagementClass(managementScope, path, options))
			{
				bootTimeWithWmi = AmHelper.GetBootTimeWithWmi(managementClass, machineName);
			}
			return bootTimeWithWmi;
		}

		private static DateTime GetBootTimeWithWmi(ManagementClass mgmtClass, AmServerName machineName)
		{
			DateTime dateTime = ExDateTime.Now.UniversalTime;
			Exception ex = null;
			try
			{
				using (ManagementObjectCollection instances = mgmtClass.GetInstances())
				{
					if (instances != null)
					{
						using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ManagementBaseObject managementBaseObject = enumerator.Current;
								ManagementObject managementObject = (ManagementObject)managementBaseObject;
								using (managementObject)
								{
									string dmtfDate = (string)managementObject["LastBootupTime"];
									dateTime = ManagementDateTimeConverter.ToDateTime(dmtfDate).ToUniversalTime();
									AmTrace.Debug("GetBootTimeWithWmi: WMI says that the boot time for {0} is {1}.", new object[]
									{
										machineName,
										dateTime
									});
								}
							}
							goto IL_102;
						}
					}
					AmTrace.Error("GetBootTimeWithWmi: WMI could not query the boot time on server {0}: No instances found for management path {1}.", new object[]
					{
						machineName,
						mgmtClass.ClassPath.Path
					});
					ReplayEventLogConstants.Tuple_GetBootTimeWithWmiFailure.LogEvent(string.Empty, new object[]
					{
						machineName,
						ReplayStrings.NoInstancesFoundForManagementPath(mgmtClass.ClassPath.Path)
					});
					IL_102:;
				}
			}
			catch (COMException ex2)
			{
				ex = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ex = ex3;
			}
			catch (ManagementException ex4)
			{
				ex = ex4;
			}
			catch (OutOfMemoryException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				AmTrace.Error("GetBootTimeWithWmi: WMI could not query the boot time on server {0}: {1}", new object[]
				{
					machineName,
					ex
				});
				ReplayEventLogConstants.Tuple_GetBootTimeWithWmiFailure.LogEvent(string.Empty, new object[]
				{
					machineName,
					ex.Message
				});
			}
			return dateTime;
		}

		private static System.Management.ManagementScope GetManagementScope(AmServerName machineName)
		{
			ManagementPath path = new ManagementPath(string.Format("\\\\{0}\\root\\cimv2", machineName.Fqdn));
			AmServerName amServerName = new AmServerName(Environment.MachineName);
			ConnectionOptions connectionOptions = new ConnectionOptions();
			if (!amServerName.Equals(machineName))
			{
				connectionOptions.Authority = string.Format("Kerberos:host/{0}", machineName.Fqdn);
			}
			return new System.Management.ManagementScope(path, connectionOptions);
		}

		private static readonly PropertyDefinition[] s_propertiesNeededFromServer = new PropertyDefinition[]
		{
			ServerSchema.ExchangeLegacyDN,
			ServerSchema.IsMailboxServer,
			ServerSchema.IsClientAccessServer,
			ServerSchema.ServerSite
		};

		private static Regex s_regexServerLegacyDnFromDatabaseLegacyDn = new Regex("^(.*)/cn=Microsoft (public|private) MDB$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
	}
}
