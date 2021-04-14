using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmPerfCounterUpdater : TimerComponent
	{
		public AmPerfCounterUpdater() : base(new TimeSpan(0, 0, 0), TimeSpan.FromSeconds((double)RegistryParameters.AmPerfCounterUpdateIntervalInSec), "AmPerfCounterUpdater")
		{
			ExTraceGlobals.AmConfigManagerTracer.TraceDebug(0L, "AmPerfCounterUpdater created.");
		}

		public void Update(List<Guid> allDatabases)
		{
			List<Guid> allDatabasesCached = new List<Guid>(allDatabases);
			lock (this.m_cacheLock)
			{
				this.m_allDatabasesCached = allDatabasesCached;
			}
		}

		protected override void TimerCallbackInternal()
		{
			List<Guid> list = null;
			lock (this.m_cacheLock)
			{
				if (this.m_allDatabasesCached == null || this.m_allDatabasesCached.Count == 0)
				{
					ExTraceGlobals.AmConfigManagerTracer.TraceDebug(0L, "AmPerfCounterUpdater: No configurations have been discovered, so nothing to do! Exiting.");
					return;
				}
				list = new List<Guid>(this.m_allDatabasesCached);
			}
			Exception ex = null;
			MdbStatus[] array = null;
			if (AmStoreHelper.GetAllDatabaseStatuses(null, false, out array, out ex))
			{
				Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>(array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null && (array[i].Status & MdbStatusFlags.Online) == MdbStatusFlags.Online)
					{
						try
						{
							dictionary.Add(array[i].MdbGuid, array[i].MdbName);
						}
						catch (ArgumentException)
						{
							ExTraceGlobals.AmConfigManagerTracer.TraceDebug<Guid, string>(0L, "AmPerfCounterUpdater: Database {0} ({1}) was returned more than once from GetAllDatabasesStatuses.", array[i].MdbGuid, array[i].MdbName);
						}
					}
				}
				AmStoreHelper.UpdateNumberOfDatabasesCounter(list.Count);
				foreach (Guid guid in list)
				{
					string empty = string.Empty;
					if (dictionary.TryGetValue(guid, out empty))
					{
						AmStoreHelper.UpdateIsMountedCounter(guid, empty, true, false);
					}
					else
					{
						AmStoreHelper.UpdateIsMountedCounter(guid, null, false, false);
					}
					IADDatabase database = Dependencies.ADConfig.GetDatabase(guid);
					if (database != null)
					{
						MountStatus? mountStatus;
						string activeServerForDatabase = ReplicaInstance.GetActiveServerForDatabase(guid, database.Name, new AmServerName(database.Server.Name).Fqdn, out mountStatus);
						AmServerName amServerName = new AmServerName(activeServerForDatabase);
						AmStoreHelper.UpdateCopyRoleIsActivePerfCounter(guid, empty, amServerName.IsLocalComputerName);
					}
					else
					{
						ExTraceGlobals.AmConfigManagerTracer.TraceError<Guid>(0L, "AmPerfCounterUpdater: Failed to get the database for guid {0}", guid);
					}
				}
				return;
			}
			AmTrace.Error("AmPerfCounterUpdater: Failed to get mounted database information from store. Exception: {0}", new object[]
			{
				ex
			});
		}

		private List<Guid> m_allDatabasesCached;

		private object m_cacheLock = new object();
	}
}
