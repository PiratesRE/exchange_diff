using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GlobalDatabaseHandler
	{
		internal ICoreGlobalDatabaseHandler CoreGlobalDatabaseHandlerClass
		{
			set
			{
				this.coreGlobalDatabaseHandler = value;
			}
		}

		internal SortedDictionary<Guid, DatabaseManager> DatabasesDictionary
		{
			get
			{
				return this.databasesDictionary;
			}
		}

		internal static bool IsSessionReusableAfterException(Exception e)
		{
			return CacheExceptionUtilities.Instance.IsSessionReusableAfterException(e);
		}

		internal static CacheTransientException CreateCacheTransientException(Trace tracer, int objectHash, Guid databaseGuid, Guid mailboxGuid, LocalizedString exceptionInfo)
		{
			return CacheExceptionUtilities.Instance.CreateCacheTransientException(tracer, objectHash, databaseGuid, mailboxGuid, exceptionInfo);
		}

		internal static Exception ConvertToCacheException(Trace tracer, int objectHash, Guid databaseGuid, Guid mailboxGuid, Exception exception, out bool reuseSession)
		{
			Exception result = CacheExceptionUtilities.Instance.ConvertToCacheException(tracer, objectHash, databaseGuid, mailboxGuid, exception, out reuseSession);
			if (ExceptionUtilities.IndicatesDatabaseDismount(exception))
			{
				ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)1297UL, GlobalDatabaseHandler.Tracer, "Exception {0} encountered. Treating the database {1} as dismounted.", new object[]
				{
					exception.GetType(),
					databaseGuid
				});
				DataAccessLayer.DatabaseHandler.ApplyDatabaseStateChange(databaseGuid, false);
				DataAccessLayer.TriggerOnDatabaseDismountedEvent(databaseGuid);
			}
			return result;
		}

		internal Guid[] GetDatabases()
		{
			Guid[] result;
			lock (this.syncObject)
			{
				if (this.Initialized())
				{
					result = this.databaseGuidLookupTable.ToArray();
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		internal SubscriptionCacheManager GetCacheManager(Guid databaseGuid)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			DatabaseManager databaseManager = null;
			lock (this.syncObject)
			{
				this.databasesDictionary.TryGetValue(databaseGuid, out databaseManager);
				if (databaseManager != null)
				{
					return databaseManager.SubscriptionCacheManager;
				}
			}
			return null;
		}

		internal Guid GetDatabaseGuid(int databaseManagerIndex)
		{
			Guid result;
			lock (this.syncObject)
			{
				result = this.databaseGuidLookupTable[databaseManagerIndex];
			}
			return result;
		}

		internal DatabaseManager GetDatabaseManager(int databaseManagerIndex)
		{
			DatabaseManager result = null;
			lock (this.syncObject)
			{
				this.databasesDictionary.TryGetValue(this.databaseGuidLookupTable[databaseManagerIndex], out result);
			}
			return result;
		}

		internal DatabaseManager GetDatabaseManager(Guid databaseGuid)
		{
			DatabaseManager result = null;
			lock (this.syncObject)
			{
				this.databasesDictionary.TryGetValue(databaseGuid, out result);
			}
			return result;
		}

		internal int GetDatabaseCount()
		{
			int count;
			lock (this.syncObject)
			{
				count = this.databaseGuidLookupTable.Count;
			}
			return count;
		}

		internal void Initialize()
		{
			if (this.coreGlobalDatabaseHandler == null)
			{
				this.coreGlobalDatabaseHandler = new GlobalDatabaseHandler.CoreGlobalDatabaseHandler();
			}
			this.databasesDictionary = new SortedDictionary<Guid, DatabaseManager>();
			DatabaseManager.MailboxCrawlerInstance.StartCrawl();
			this.PollDatabases();
			this.findDatabasesTimer = new GuardedTimer(new TimerCallback(this.OnFindDatabasesTimerCallback), null, ContentAggregationConfig.DatabasePollingInterval, TimeSpan.FromMilliseconds(-1.0));
		}

		internal void OnFindDatabasesTimerCallback(object state)
		{
			lock (this.syncObject)
			{
				if (this.findDatabasesTimer == null)
				{
					return;
				}
			}
			this.PollDatabases();
			lock (this.syncObject)
			{
				if (this.findDatabasesTimer != null)
				{
					this.findDatabasesTimer.Change(ContentAggregationConfig.DatabasePollingInterval, TimeSpan.FromMilliseconds(-1.0));
				}
			}
		}

		internal void Shutdown()
		{
			List<DatabaseManager> list = new List<DatabaseManager>();
			lock (this.syncObject)
			{
				this.findDatabasesTimer.Dispose(false);
				this.findDatabasesTimer = null;
				foreach (KeyValuePair<Guid, DatabaseManager> keyValuePair in this.databasesDictionary)
				{
					if (keyValuePair.Value.Enabled)
					{
						list.Add(keyValuePair.Value);
					}
				}
				this.databasesDictionary.Clear();
			}
			DatabaseManager.MailboxCrawlerInstance.StopCrawl();
			foreach (DatabaseManager databaseManager in list)
			{
				databaseManager.Shutdown();
			}
		}

		internal SortedDictionary<Guid, bool> FindLocalDatabases(out string dbPollingSource)
		{
			SortedDictionary<Guid, bool> sortedDictionary = this.FindLocalDatabasesFromAdminRPC();
			if (sortedDictionary == null)
			{
				sortedDictionary = this.FindLocalDatabasesFromAD();
				dbPollingSource = "AD";
			}
			else
			{
				dbPollingSource = "AdminRPC";
			}
			return sortedDictionary;
		}

		internal void ApplyNewDatabaseTest(Guid databaseGuid, Guid systemMailboxGuid, bool enabled)
		{
			this.ApplyNewDatabase(databaseGuid, enabled);
			DatabaseManager databaseManager = this.GetDatabaseManager(databaseGuid);
			databaseManager.SystemMailboxGuid = systemMailboxGuid;
		}

		internal void ApplyDeletedDatabaseTest(Guid databaseGuid)
		{
			this.ApplyDeletedDatabase(databaseGuid);
		}

		internal void ApplyDatabaseStateChangeTest(Guid databaseGuid)
		{
			DatabaseManager databaseManager = this.GetDatabaseManager(databaseGuid);
			this.ApplyDatabaseStateChange(databaseGuid, !databaseManager.Enabled);
		}

		internal void OnDatabaseDismounted(object sender, OnDatabaseDismountedEventArgs databaseDismountedArgs)
		{
			Guid databaseGuid = databaseDismountedArgs.DatabaseGuid;
			this.ApplyDatabaseStateChange(databaseGuid, false);
		}

		internal XElement GetDiagnosticInfo(SyncDiagnosticMode mode)
		{
			XElement xelement = new XElement("GlobalDatabaseHandler");
			xelement.Add(new XElement("DatabaseCount", this.GetDatabaseCount()));
			xelement.Add(new XElement("LastDatabaseDiscoveryStartTime", (this.lastDatabaseDiscoveryStartTime != null) ? this.lastDatabaseDiscoveryStartTime.Value.ToString("o") : string.Empty));
			DatabaseManager.AddMailboxCrawlerDiagnosticInfoTo(xelement);
			XElement xelement2 = new XElement("Databases");
			lock (this.syncObject)
			{
				foreach (DatabaseManager databaseManager in this.databasesDictionary.Values)
				{
					xelement2.Add(databaseManager.GetDiagnosticInfo(mode));
				}
			}
			if (mode == SyncDiagnosticMode.Verbose)
			{
				XElement xelement3 = new XElement("MailboxCrawlQueue");
				DatabaseManager.AddMailboxCrawlerQueueEntriesDiagnosticInfoTo(xelement3);
				xelement.Add(xelement3);
			}
			xelement.Add(xelement2);
			return xelement;
		}

		private static string DBStateString(bool state)
		{
			if (!state)
			{
				return "Disabled";
			}
			return "Enabled";
		}

		private void PollDatabases()
		{
			try
			{
				string dbPollingSource = null;
				SortedDictionary<Guid, bool> databaseList = this.FindLocalDatabases(out dbPollingSource);
				this.ApplyLatestDatabaseStatus(databaseList, dbPollingSource);
			}
			catch (Exception exception)
			{
				DataAccessLayer.ReportWatson("Exception during DB Discovery", exception);
			}
		}

		private void ApplyNewDatabase(Guid databaseGuid, bool enabled)
		{
			ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)278UL, GlobalDatabaseHandler.Tracer, "ApplyNewDatabase: Found new database {0} in {1} state.", new object[]
			{
				databaseGuid,
				enabled ? "enabled" : "disabled"
			});
			DatabaseManager databaseManager = null;
			lock (this.syncObject)
			{
				databaseManager = this.NewDatabaseManager(databaseGuid);
			}
			if (databaseManager != null && enabled)
			{
				databaseManager.Initialize();
			}
		}

		private void ApplyDeletedDatabase(Guid databaseGuid)
		{
			DatabaseManager databaseManager = this.GetDatabaseManager(databaseGuid);
			if (databaseManager != null)
			{
				lock (this.syncObject)
				{
					bool enabled = databaseManager.Enabled;
					ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)279UL, GlobalDatabaseHandler.Tracer, "ApplyDeletedDatabase: Database {0} which was in {1} state does not exist anymore.", new object[]
					{
						databaseGuid,
						enabled
					});
					this.DeleteDatabaseManager(databaseManager);
				}
				if (databaseManager != null && databaseManager.Enabled)
				{
					databaseManager.Shutdown();
				}
			}
			DataAccessLayer.TriggerOnDatabaseDismountedEvent(databaseGuid);
		}

		private void ApplyDatabaseStateChange(Guid databaseGuid, bool enabled)
		{
			ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)280UL, GlobalDatabaseHandler.Tracer, "ApplyDatabaseStateChange: Database {0} changed status from {1} to {2}", new object[]
			{
				databaseGuid,
				enabled ? "disabled" : "enabled",
				enabled ? "enabled" : "disabled"
			});
			DatabaseManager databaseManager = null;
			lock (this.syncObject)
			{
				databaseManager = this.GetDatabaseManager(databaseGuid);
				if (databaseManager == null || databaseManager.Enabled == enabled)
				{
					return;
				}
			}
			if (!enabled)
			{
				databaseManager.Shutdown();
				return;
			}
			databaseManager.Initialize();
		}

		private void ApplyLatestDatabaseStatus(SortedDictionary<Guid, bool> databaseList, string dbPollingSource)
		{
			this.lastDatabaseDiscoveryStartTime = new ExDateTime?(ExDateTime.UtcNow);
			if (databaseList == null)
			{
				ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)281UL, GlobalDatabaseHandler.Tracer, "ApplyLatestDatabaseStatus skipped. Failed to read the local databases.", new object[0]);
				return;
			}
			ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)282UL, GlobalDatabaseHandler.Tracer, "ApplyLatestDatabaseStatus called with {0} databases in the database list.", new object[]
			{
				databaseList.Count
			});
			IEnumerator<KeyValuePair<Guid, bool>> enumerator = databaseList.GetEnumerator();
			bool flag = enumerator.MoveNext();
			int num = 0;
			Dictionary<Guid, bool> dictionary = new Dictionary<Guid, bool>();
			List<Guid> list = new List<Guid>();
			List<KeyValuePair<Guid, bool>> list2 = new List<KeyValuePair<Guid, bool>>();
			int num2 = 0;
			lock (this.syncObject)
			{
				using (SortedDictionary<Guid, DatabaseManager>.Enumerator enumerator2 = this.databasesDictionary.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						KeyValuePair<Guid, DatabaseManager> keyValuePair = enumerator2.Current;
						while (flag)
						{
							KeyValuePair<Guid, bool> keyValuePair2 = enumerator.Current;
							if (keyValuePair2.Key.CompareTo(keyValuePair.Key) >= 0)
							{
								break;
							}
							Dictionary<Guid, bool> dictionary2 = dictionary;
							KeyValuePair<Guid, bool> keyValuePair3 = enumerator.Current;
							Guid key = keyValuePair3.Key;
							KeyValuePair<Guid, bool> keyValuePair4 = enumerator.Current;
							dictionary2.Add(key, keyValuePair4.Value);
							KeyValuePair<Guid, bool> keyValuePair5 = enumerator.Current;
							if (keyValuePair5.Value)
							{
								num2++;
							}
							flag = enumerator.MoveNext();
						}
						if (flag)
						{
							KeyValuePair<Guid, bool> keyValuePair6 = enumerator.Current;
							if (keyValuePair6.Key == keyValuePair.Key)
							{
								KeyValuePair<Guid, bool> keyValuePair7 = enumerator.Current;
								if (keyValuePair7.Value != keyValuePair.Value.Enabled)
								{
									list2.Add(enumerator.Current);
								}
								else
								{
									num++;
								}
								KeyValuePair<Guid, bool> keyValuePair8 = enumerator.Current;
								if (keyValuePair8.Value)
								{
									num2++;
								}
								flag = enumerator.MoveNext();
								continue;
							}
						}
						list.Add(keyValuePair.Key);
					}
					goto IL_1F9;
				}
				IL_1CE:
				Dictionary<Guid, bool> dictionary3 = dictionary;
				KeyValuePair<Guid, bool> keyValuePair9 = enumerator.Current;
				Guid key2 = keyValuePair9.Key;
				KeyValuePair<Guid, bool> keyValuePair10 = enumerator.Current;
				dictionary3.Add(key2, keyValuePair10.Value);
				flag = enumerator.MoveNext();
				IL_1F9:
				if (flag)
				{
					goto IL_1CE;
				}
			}
			foreach (KeyValuePair<Guid, bool> keyValuePair11 in dictionary)
			{
				this.ApplyNewDatabase(keyValuePair11.Key, keyValuePair11.Value);
				SyncHealthLogManager.TryLogDatabaseDiscovery(this.lastDatabaseDiscoveryStartTime.Value, dbPollingSource, databaseList.Count, num2, keyValuePair11.Key.ToString(), "Add", GlobalDatabaseHandler.DBStateString(keyValuePair11.Value));
			}
			foreach (Guid databaseGuid in list)
			{
				this.ApplyDeletedDatabase(databaseGuid);
				SyncHealthLogManager.TryLogDatabaseDiscovery(this.lastDatabaseDiscoveryStartTime.Value, dbPollingSource, databaseList.Count, num2, databaseGuid.ToString(), "Delete", GlobalDatabaseHandler.DBStateString(false));
			}
			foreach (KeyValuePair<Guid, bool> keyValuePair12 in list2)
			{
				this.ApplyDatabaseStateChange(keyValuePair12.Key, keyValuePair12.Value);
				SyncHealthLogManager.TryLogDatabaseDiscovery(this.lastDatabaseDiscoveryStartTime.Value, dbPollingSource, databaseList.Count, num2, keyValuePair12.Key.ToString(), "StateChange", GlobalDatabaseHandler.DBStateString(keyValuePair12.Value));
			}
			ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)283UL, GlobalDatabaseHandler.Tracer, "ApplyLatestDatabaseStatus exiting. Processed {0} additions, {1} deletions, {2} state changes, {3} unchanged.", new object[]
			{
				dictionary.Count,
				list.Count,
				list2.Count,
				num
			});
		}

		private SortedDictionary<Guid, bool> FindLocalDatabasesFromAdminRPC()
		{
			return this.coreGlobalDatabaseHandler.FindLocalDatabasesFromAdminRPC();
		}

		private SortedDictionary<Guid, bool> FindLocalDatabasesFromAD()
		{
			return this.coreGlobalDatabaseHandler.FindLocalDatabasesFromAD();
		}

		private DatabaseManager NewDatabaseManager(Guid databaseGuid)
		{
			if (!this.databaseGuidLookupTable.Contains(databaseGuid))
			{
				this.databaseGuidLookupTable.Add(databaseGuid);
			}
			byte databaseLookupIndex = (byte)this.databaseGuidLookupTable.IndexOf(databaseGuid);
			DatabaseManager databaseManager = new DatabaseManager(databaseGuid, databaseLookupIndex);
			this.databasesDictionary.Add(databaseGuid, databaseManager);
			this.coreGlobalDatabaseHandler.OnNewDatabaseManager(databaseManager);
			return databaseManager;
		}

		private void DeleteDatabaseManager(DatabaseManager databaseManager)
		{
			this.databasesDictionary.Remove(databaseManager.DatabaseGuid);
		}

		private bool Initialized()
		{
			bool result;
			lock (this.syncObject)
			{
				result = (this.findDatabasesTimer != null);
			}
			return result;
		}

		private const string EnabledState = "Enabled";

		private const string DisabledState = "Disabled";

		private static readonly Trace Tracer = ExTraceGlobals.GlobalDatabaseHandlerTracer;

		private object syncObject = new object();

		private ICoreGlobalDatabaseHandler coreGlobalDatabaseHandler;

		private GuardedTimer findDatabasesTimer;

		private SortedDictionary<Guid, DatabaseManager> databasesDictionary;

		private List<Guid> databaseGuidLookupTable = new List<Guid>();

		private ExDateTime? lastDatabaseDiscoveryStartTime = null;

		private class CoreGlobalDatabaseHandler : ICoreGlobalDatabaseHandler
		{
			public SortedDictionary<Guid, bool> FindLocalDatabasesFromAD()
			{
				if (ContentAggregationConfig.LocalServer == null)
				{
					ContentAggregationConfig.SyncLogSession.LogError((TSLID)284UL, GlobalDatabaseHandler.Tracer, "Cannot read the local databases from AD. Failed to load the LocalServer info", new object[0]);
					return null;
				}
				MailboxDatabase[] databases = null;
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					databases = ContentAggregationConfig.LocalServer.GetMailboxDatabases();
				});
				if (adoperationResult.ErrorCode != ADOperationErrorCode.Success)
				{
					ContentAggregationConfig.SyncLogSession.LogError((TSLID)285UL, GlobalDatabaseHandler.Tracer, "{0} error when looking for local databases {1}", new object[]
					{
						(adoperationResult.ErrorCode == ADOperationErrorCode.RetryableError) ? "Retryable" : "Permanent",
						adoperationResult.Exception
					});
					return null;
				}
				if (databases.Length == 0)
				{
					ContentAggregationConfig.SyncLogSession.LogError((TSLID)286UL, GlobalDatabaseHandler.Tracer, "No databases found in AD for the local server", new object[0]);
					return null;
				}
				SortedDictionary<Guid, bool> sortedDictionary = new SortedDictionary<Guid, bool>();
				foreach (MailboxDatabase mailboxDatabase in databases)
				{
					bool flag = mailboxDatabase.IsValid && mailboxDatabase.Mounted != null && mailboxDatabase.Mounted.Value;
					ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)287UL, GlobalDatabaseHandler.Tracer, "AD DB discovery: Found DB {0} in {1} state", new object[]
					{
						mailboxDatabase.Guid,
						flag ? "enabled" : "disabled"
					});
					sortedDictionary.Add(mailboxDatabase.Guid, flag);
				}
				return sortedDictionary;
			}

			public SortedDictionary<Guid, bool> FindLocalDatabasesFromAdminRPC()
			{
				if (ContentAggregationConfig.LocalServer == null)
				{
					ContentAggregationConfig.SyncLogSession.LogError((TSLID)288UL, GlobalDatabaseHandler.Tracer, "Cannot read the local databases from Admin RPC. Failed to load the LocalServer info", new object[0]);
					return null;
				}
				SortedDictionary<Guid, bool> sortedDictionary = new SortedDictionary<Guid, bool>();
				try
				{
					using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=TransportSync", ContentAggregationConfig.LocalServer.Name, null, null, null))
					{
						MdbStatus[] array = exRpcAdmin.ListMdbStatus(true);
						if (array == null || array.Length == 0)
						{
							ContentAggregationConfig.SyncLogSession.LogError((TSLID)289UL, GlobalDatabaseHandler.Tracer, "No databases found in Admin RPC for the local server", new object[0]);
							return null;
						}
						foreach (MdbStatus mdbStatus in array)
						{
							bool flag = (mdbStatus.Status & MdbStatusFlags.Online) == MdbStatusFlags.Online;
							ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)290UL, GlobalDatabaseHandler.Tracer, "Admin RPC DB discovery: Found DB {0} in {1} state", new object[]
							{
								mdbStatus.MdbGuid,
								flag ? "enabled" : "disabled"
							});
							sortedDictionary.Add(mdbStatus.MdbGuid, flag);
						}
					}
				}
				catch (MapiPermanentException ex)
				{
					ContentAggregationConfig.SyncLogSession.LogError((TSLID)291UL, GlobalDatabaseHandler.Tracer, "Permanent error when looking for local databases {0}", new object[]
					{
						ex
					});
					return null;
				}
				catch (MapiRetryableException ex2)
				{
					ContentAggregationConfig.SyncLogSession.LogError((TSLID)292UL, GlobalDatabaseHandler.Tracer, "Retryable error when looking for local databases {0}", new object[]
					{
						ex2
					});
					return null;
				}
				return sortedDictionary;
			}

			public void OnNewDatabaseManager(DatabaseManager databaseManager)
			{
			}
		}
	}
}
