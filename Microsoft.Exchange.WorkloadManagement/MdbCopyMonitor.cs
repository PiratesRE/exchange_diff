using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.WorkloadManagement.EventLogs;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class MdbCopyMonitor
	{
		protected MdbCopyMonitor()
		{
			this.timer = new GuardedTimer(delegate(object state)
			{
				this.ReadDataFromAD();
			});
		}

		public static Hookable<MdbCopyMonitor> Instance
		{
			get
			{
				return MdbCopyMonitor.instance;
			}
		}

		public virtual List<Guid> GetMdbsForServer(string serverFqdn)
		{
			this.Initialize();
			List<Guid> result;
			try
			{
				this.lockObject.EnterReadLock();
				List<Guid> list = null;
				if (this.mdbByServerFqdn != null)
				{
					this.mdbByServerFqdn.TryGetValue(serverFqdn, out list);
				}
				result = list;
			}
			finally
			{
				try
				{
					this.lockObject.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return result;
		}

		public virtual List<string> GetServersForMdb(Guid mdbGuid)
		{
			this.Initialize();
			List<string> result;
			try
			{
				this.lockObject.EnterReadLock();
				List<string> list = null;
				if (this.serverFqdnByMdb != null)
				{
					this.serverFqdnByMdb.TryGetValue(mdbGuid, out list);
				}
				result = list;
			}
			finally
			{
				try
				{
					this.lockObject.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return result;
		}

		private static bool TryRunAdOperation(ADOperation adCall, out Exception exception)
		{
			exception = null;
			try
			{
				ADNotificationAdapter.RunADOperation(adCall);
			}
			catch (ADTransientException ex)
			{
				exception = ex;
				return false;
			}
			catch (ADOperationException ex2)
			{
				exception = ex2;
				return false;
			}
			return true;
		}

		private void Initialize()
		{
			if (!this.isTimerInitialized)
			{
				lock (this.timer)
				{
					if (!this.isTimerInitialized)
					{
						this.isTimerInitialized = true;
						this.ResetTimer();
					}
				}
			}
			this.RegisterADChangeNotification();
			bool condition = this.initializationComplete.WaitOne(TimeSpan.FromMinutes(15.0));
			ExAssert.RetailAssert(condition, "Waiting for MdbCopyMonitor initialization timed out.");
		}

		private void ResetTimer()
		{
			lock (this.timer)
			{
				this.timer.Change(CiHealthMonitorConfiguration.MdbCopyUpdateDelay, CiHealthMonitorConfiguration.MdbCopyUpdateInterval);
			}
		}

		private void RegisterADChangeNotification()
		{
			if (Interlocked.Exchange(ref this.isAdNotificationRegistered, 1) == 0)
			{
				Exception arg;
				if (!MdbCopyMonitor.TryRunAdOperation(delegate
				{
					ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 251, "RegisterADChangeNotification", "f:\\15.00.1497\\sources\\dev\\WorkloadManagement\\src\\WorkloadManagement\\ResourceMonitors\\ContentIndexing\\MdbCopyMonitor.cs");
					ADObjectId databasesContainerId = topologyConfigurationSession.GetDatabasesContainerId();
					ADNotificationAdapter.RegisterChangeNotification<DatabaseCopy>(databasesContainerId, delegate(ADNotificationEventArgs args)
					{
						this.ResetTimer();
					});
				}, out arg))
				{
					ExTraceGlobals.ResourceHealthManagerTracer.TraceError<Exception>((long)this.GetHashCode(), "[MdbCopyMonitor::RegisterADChangeNotification] Failed to sign up for AD notifications, exception: {0}", arg);
					Interlocked.Exchange(ref this.isAdNotificationRegistered, 0);
				}
			}
		}

		private void ReadDataFromAD()
		{
			try
			{
				Dictionary<ADObjectId, string> serverFqdnCache = new Dictionary<ADObjectId, string>();
				MailboxDatabase[] mailboxDatabases = null;
				Exception ex;
				bool flag = MdbCopyMonitor.TryRunAdOperation(delegate
				{
					serverFqdnCache.Clear();
					ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 288, "ReadDataFromAD", "f:\\15.00.1497\\sources\\dev\\WorkloadManagement\\src\\WorkloadManagement\\ResourceMonitors\\ContentIndexing\\MdbCopyMonitor.cs");
					Server localServer = LocalServerCache.LocalServer;
					if (localServer == null)
					{
						throw new LocalServerNotFoundException(string.Empty);
					}
					serverFqdnCache.Add(localServer.Id, localServer.Fqdn);
					mailboxDatabases = localServer.GetMailboxDatabases();
					foreach (MailboxDatabase mailboxDatabase2 in mailboxDatabases)
					{
						if (mailboxDatabase2.ActivationPreference != null)
						{
							foreach (KeyValuePair<ADObjectId, int> keyValuePair2 in mailboxDatabase2.ActivationPreference)
							{
								if (!serverFqdnCache.ContainsKey(keyValuePair2.Key))
								{
									Server server = topologyConfigurationSession.Read<Server>(keyValuePair2.Key);
									serverFqdnCache.Add(keyValuePair2.Key, server.Fqdn);
								}
							}
						}
					}
				}, out ex);
				if (flag)
				{
					Dictionary<string, List<Guid>> dictionary = new Dictionary<string, List<Guid>>(serverFqdnCache.Count);
					Dictionary<Guid, List<string>> dictionary2 = new Dictionary<Guid, List<string>>(mailboxDatabases.Length);
					foreach (MailboxDatabase mailboxDatabase in mailboxDatabases)
					{
						if (mailboxDatabase.ActivationPreference != null)
						{
							List<string> list = new List<string>(4);
							dictionary2.Add(mailboxDatabase.Guid, list);
							foreach (KeyValuePair<ADObjectId, int> keyValuePair in mailboxDatabase.ActivationPreference)
							{
								string text = serverFqdnCache[keyValuePair.Key];
								list.Add(text);
								List<Guid> list2;
								if (!dictionary.TryGetValue(text, out list2))
								{
									list2 = new List<Guid>(40);
									dictionary.Add(text, list2);
								}
								list2.Add(mailboxDatabase.Guid);
							}
						}
					}
					try
					{
						this.lockObject.EnterWriteLock();
						this.mdbByServerFqdn = dictionary;
						this.serverFqdnByMdb = dictionary2;
						goto IL_1AC;
					}
					finally
					{
						try
						{
							this.lockObject.ExitWriteLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
				}
				ExTraceGlobals.ResourceHealthManagerTracer.TraceError<Exception>((long)this.GetHashCode(), "[MdbCopyMonitor::ReadDataFromAD] Failed to read data from AD, exception: {0}", ex);
				WorkloadManagerEventLogger.LogEvent(WorkloadManagementEventLogConstants.Tuple_CiMdbCopyMonitorFailure, string.Empty, new object[]
				{
					ex.ToString()
				});
				try
				{
					this.lockObject.EnterWriteLock();
					this.mdbByServerFqdn = null;
					this.serverFqdnByMdb = null;
				}
				finally
				{
					try
					{
						this.lockObject.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				IL_1AC:;
			}
			finally
			{
				this.initializationComplete.Set();
			}
		}

		private const int DefaultCopiesPerMdb = 4;

		private const int DefaultMdbCopiesPerServer = 40;

		private static readonly Hookable<MdbCopyMonitor> instance = Hookable<MdbCopyMonitor>.Create(true, new MdbCopyMonitor());

		private readonly ManualResetEvent initializationComplete = new ManualResetEvent(false);

		private readonly ReaderWriterLockSlim lockObject = new ReaderWriterLockSlim();

		private readonly GuardedTimer timer;

		private Dictionary<string, List<Guid>> mdbByServerFqdn;

		private Dictionary<Guid, List<string>> serverFqdnByMdb;

		private int isAdNotificationRegistered;

		private bool isTimerInitialized;
	}
}
