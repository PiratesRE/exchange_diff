using System;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MdbSystemMailboxPinger : DisposeTrackableBase, IMdbSystemMailboxPinger
	{
		public static Func<Guid, MdbSystemMailboxPinger, bool> OnTestPing { get; set; }

		private static TimeSpan ReadPingTimeoutFromConfig()
		{
			TimeSpanAppSettingsEntry timeSpanAppSettingsEntry = new TimeSpanAppSettingsEntry("PingTimeoutInSeconds", TimeSpanUnit.Seconds, TimeSpan.FromSeconds(120.0), ExTraceGlobals.DatabasePingerTracer);
			return timeSpanAppSettingsEntry.Value;
		}

		internal static Action<Guid, TimeSpan> OnPingTimeout { get; set; }

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MdbSystemMailboxPinger>(this);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				lock (this.instanceLock)
				{
					this.DisposeAccessInfo();
					if (this.registeredWaitHandle != null)
					{
						this.registeredWaitHandle.Unregister(this.remoteCallDoneEvent);
						this.registeredWaitHandle = null;
					}
					this.remoteCallDoneEvent.Close();
					this.remoteCallDoneEvent = null;
				}
			}
		}

		public MdbSystemMailboxPinger(Guid databaseGuid)
		{
			if (databaseGuid == Guid.Empty)
			{
				throw new ArgumentException("databaseGuid cannot be Guid.Empty", "databaseGuid");
			}
			this.waitOrTimerCallback = new WaitOrTimerCallback(this.TimeoutCallback);
			this.databaseGuid = databaseGuid;
			this.systemMailboxName = "SystemMailbox{" + this.databaseGuid + "}";
			ExTraceGlobals.DatabasePingerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Creating MdbSystemMailboxPinger for private mdb guid {0}", this.databaseGuid);
		}

		internal static TimeSpan PingTimeout
		{
			get
			{
				return MdbSystemMailboxPinger.pingTimeout;
			}
			set
			{
				MdbSystemMailboxPinger.pingTimeout = value;
			}
		}

		public DateTime LastSuccessfulPingUtc
		{
			get
			{
				base.CheckDisposed();
				return this.lastSuccessfulPingUtc;
			}
		}

		public bool Pinging
		{
			get
			{
				base.CheckDisposed();
				return this.pinging;
			}
			private set
			{
				this.pinging = value;
			}
		}

		public DateTime LastPingAttemptUtc
		{
			get
			{
				base.CheckDisposed();
				return this.lastPingAttemptUtc;
			}
		}

		public bool Ping()
		{
			base.CheckDisposed();
			bool flag = false;
			bool flag2 = false;
			this.lastPingAttemptUtc = TimeProvider.UtcNow;
			try
			{
				if (!this.Pinging)
				{
					lock (this.instanceLock)
					{
						if (!this.Pinging)
						{
							this.Pinging = true;
							flag2 = true;
						}
					}
				}
				if (flag2)
				{
					ExTraceGlobals.DatabasePingerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Attempting XSO ping against database {0}", this.databaseGuid);
					lock (this.instanceLock)
					{
						PerformanceContext performanceContext = default(PerformanceContext);
						bool flag5 = false;
						bool flag6 = ExTraceGlobals.DatabasePingerTracer.IsTraceEnabled(TraceType.DebugTrace);
						if (flag6)
						{
							flag5 = NativeMethods.GetTLSPerformanceContext(out performanceContext);
						}
						DateTime utcNow = TimeProvider.UtcNow;
						flag = this.OpenSession();
						if (flag)
						{
							PingerPerfCounterWrapper.PingSuccessful();
						}
						else
						{
							PingerPerfCounterWrapper.PingFailed();
						}
						TimeSpan timeSpan = TimeProvider.UtcNow - utcNow;
						PerformanceContext performanceContext2;
						if (flag6 && flag5 && NativeMethods.GetTLSPerformanceContext(out performanceContext2))
						{
							uint num = performanceContext2.rpcCount - performanceContext.rpcCount;
							ulong num2 = performanceContext2.rpcLatency - performanceContext.rpcLatency;
							ExTraceGlobals.DatabasePingerTracer.TraceDebug((long)this.GetHashCode(), "Ping Stats - Mdb: {0}, Rpc Count: {1}, Rpc Latency: {2}. Elapsed: {3}", new object[]
							{
								this.databaseGuid,
								num,
								num2,
								timeSpan
							});
						}
						goto IL_17F;
					}
				}
				ExTraceGlobals.DatabasePingerTracer.TraceError<Guid>((long)this.GetHashCode(), "Could not make ping call against mdb {0} because there is already an outstanding ping.", this.databaseGuid);
				IL_17F:;
			}
			catch (StoragePermanentException arg)
			{
				ExTraceGlobals.DatabasePingerTracer.TraceError<Guid, StoragePermanentException>((long)this.GetHashCode(), "Encountered permanent exception acquiring and pinging mdb {0}.  Exception: {1}", this.databaseGuid, arg);
				this.principal = null;
				this.DisposeAccessInfo();
				this.pingerState = MdbSystemMailboxPinger.PingerState.NeedReinitialization;
				PingerPerfCounterWrapper.PingFailed();
			}
			catch (StorageTransientException arg2)
			{
				PingerPerfCounterWrapper.PingFailed();
				ExTraceGlobals.DatabasePingerTracer.TraceError<Guid, StorageTransientException>((long)this.GetHashCode(), "Encountered transient exception acquiring and pinging mdb {0}.  Exception: {1}", this.databaseGuid, arg2);
			}
			catch (Exception)
			{
				PingerPerfCounterWrapper.PingFailed();
				throw;
			}
			finally
			{
				if (flag2)
				{
					lock (this.instanceLock)
					{
						this.Pinging = false;
					}
				}
			}
			return flag;
		}

		private MailboxSession GetMailboxSession()
		{
			if (MdbSystemMailboxPinger.OnTestPing != null)
			{
				return null;
			}
			return MailboxSession.ConfigurableOpen(this.principal, this.accessInfo, CultureInfo.InvariantCulture, "Client=ResourceHealth;Action=DatabasePing", LogonType.SystemService, MdbSystemMailboxPinger.LocalePropertyDefinition, MailboxSession.InitializationFlags.None, Array<DefaultFolderType>.Empty);
		}

		private bool OpenSession()
		{
			bool flag = false;
			if (this.AcquireADObjectsForPrivateMdb())
			{
				using (MailboxSession mailboxSession = this.GetMailboxSession())
				{
					bool flag2 = false;
					try
					{
						lock (this.instanceLock)
						{
							if (this.registeredWaitHandle == null && this.remoteCallDoneEvent != null)
							{
								this.remoteCallDoneEvent.Reset();
								this.registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(this.remoteCallDoneEvent, this.waitOrTimerCallback, null, MdbSystemMailboxPinger.PingTimeout, true);
								flag2 = true;
							}
						}
						if (MdbSystemMailboxPinger.OnTestPing != null)
						{
							flag = MdbSystemMailboxPinger.OnTestPing(this.databaseGuid, this);
						}
						else
						{
							mailboxSession.Mailbox.ForceReload(new PropertyDefinition[]
							{
								MailboxSchema.QuotaUsedExtended
							});
							flag = true;
						}
						if (flag)
						{
							this.pingerState = MdbSystemMailboxPinger.PingerState.Normal;
						}
					}
					finally
					{
						if (flag2)
						{
							this.UnregisterWaitHandle();
						}
					}
				}
			}
			if (flag)
			{
				this.lastSuccessfulPingUtc = TimeProvider.UtcNow;
				ExTraceGlobals.DatabasePingerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Successfully pinged database {0}", this.databaseGuid);
			}
			return flag;
		}

		private void TimeoutCallback(object state, bool timedOut)
		{
			this.UnregisterWaitHandle();
			TimeSpan timeSpan = TimeProvider.UtcNow - this.lastPingAttemptUtc;
			if (timeSpan < MdbSystemMailboxPinger.PingTimeout)
			{
				ExTraceGlobals.DatabasePingerTracer.TraceError<TimeSpan, TimeSpan, Guid>((long)this.GetHashCode(), "Timeout was called before PingTimeout reached.  Ignoring.  Elapsed: {0}, PingTimeout: {1}, Database: {2}", timeSpan, MdbSystemMailboxPinger.PingTimeout, this.databaseGuid);
				return;
			}
			if (timedOut)
			{
				PingerPerfCounterWrapper.PingTimedOut();
				if (MdbSystemMailboxPinger.OnPingTimeout != null)
				{
					MdbSystemMailboxPinger.OnPingTimeout(this.databaseGuid, MdbSystemMailboxPinger.PingTimeout);
				}
				ExTraceGlobals.DatabasePingerTracer.TraceError<Guid>((long)this.GetHashCode(), "Ping for Mdb '{0}' timed out.  This might suggest that the remote server is down.", this.databaseGuid);
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorDatabasePingTimedOut, string.Empty, new object[]
				{
					this.databaseGuid,
					timeSpan,
					(this.lastSuccessfulPingUtc == DateTime.MinValue) ? "[never]" : this.lastSuccessfulPingUtc.ToString()
				});
			}
		}

		private void UnregisterWaitHandle()
		{
			lock (this.instanceLock)
			{
				if (this.registeredWaitHandle != null && this.remoteCallDoneEvent != null)
				{
					this.registeredWaitHandle.Unregister(this.remoteCallDoneEvent);
					this.registeredWaitHandle = null;
				}
			}
		}

		private bool AcquireADObjectsForPrivateMdb()
		{
			if (MdbSystemMailboxPinger.OnTestPing != null)
			{
				return true;
			}
			bool flag = false;
			try
			{
				if (this.principal != null)
				{
					if (!this.VerifyLocalBoxCall(this.principal.MailboxInfo.Location.ServerFqdn))
					{
						return false;
					}
					flag = true;
					return true;
				}
				else
				{
					if ((this.pingerState == MdbSystemMailboxPinger.PingerState.NeedReinitialization || this.pingerState == MdbSystemMailboxPinger.PingerState.NotInitialized) && TimeProvider.UtcNow - this.lastSessionAttemptUtc < MdbSystemMailboxPinger.OpenSessionAttemptInterval)
					{
						ExTraceGlobals.DatabasePingerTracer.TraceDebug((long)this.GetHashCode(), "Need to acquire principal, but not enough time has passed between attempts.");
						return false;
					}
					this.lastSessionAttemptUtc = TimeProvider.UtcNow;
					ActiveManager noncachingActiveManagerInstance = ActiveManager.GetNoncachingActiveManagerInstance();
					DatabaseLocationInfo serverForDatabase = noncachingActiveManagerInstance.GetServerForDatabase(this.databaseGuid, true);
					if (!this.VerifyLocalBoxCall(serverForDatabase.ServerFqdn))
					{
						return false;
					}
					ADSessionSettings adsessionSettings = ADSessionSettings.FromRootOrgScopeSet();
					ADSystemMailbox adSystemMailbox = this.FindSystemMailbox(adsessionSettings);
					Server server = this.FindMdbServer(adsessionSettings, serverForDatabase.ServerFqdn);
					if (server == null)
					{
						ExTraceGlobals.DatabasePingerTracer.TraceError<string>((long)this.GetHashCode(), "[MdbSystemMailboxPinger.AcquireADObjectsForPrivateMdb] Failed to find server with FQDN: '{0}'", serverForDatabase.ServerFqdn);
						return false;
					}
					this.principal = ExchangePrincipal.FromADSystemMailbox(adsessionSettings, adSystemMailbox, server);
					this.accessInfo = new MailboxAccessInfo(new WindowsPrincipal(WindowsIdentity.GetCurrent()));
					flag = true;
				}
			}
			catch (StoragePermanentException arg)
			{
				ExTraceGlobals.DatabasePingerTracer.TraceError<StoragePermanentException>((long)this.GetHashCode(), "Encountered StoragePermanentException obtaining ExchangePrincipal.  Exception: {0}", arg);
			}
			catch (StorageTransientException arg2)
			{
				ExTraceGlobals.DatabasePingerTracer.TraceError<StorageTransientException>((long)this.GetHashCode(), "Encountered StorageTransientException obtaining ExchangePrincipal.  Exception: {0}", arg2);
			}
			catch (DataSourceOperationException arg3)
			{
				ExTraceGlobals.DatabasePingerTracer.TraceError<DataSourceOperationException>((long)this.GetHashCode(), "EncounteredDataSourceOperationException obtaining ExchangePrincipal.  Exception :{0}", arg3);
			}
			catch (DataSourceTransientException arg4)
			{
				ExTraceGlobals.DatabasePingerTracer.TraceError<DataSourceTransientException>((long)this.GetHashCode(), "Encountered DataSourceTransientException obtaining ExchangePrincipal.  Exception :{0}", arg4);
			}
			finally
			{
				if (!flag)
				{
					this.principal = null;
					this.DisposeAccessInfo();
				}
			}
			return flag;
		}

		private bool VerifyLocalBoxCall(string serverFqdn)
		{
			if (serverFqdn == null || !serverFqdn.StartsWith(Environment.MachineName + ".", StringComparison.OrdinalIgnoreCase))
			{
				ExTraceGlobals.DatabasePingerTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "[MdbSystemMailboxPinger.VerifyLocalBoxCall] Will not ping database '{0}' since it is on a different server: '{1}'", this.databaseGuid, serverFqdn);
				return false;
			}
			return true;
		}

		private void DisposeAccessInfo()
		{
			lock (this.instanceLock)
			{
				if (this.accessInfo != null)
				{
					if (this.accessInfo.AuthenticatedUserPrincipal != null)
					{
						IDisposable disposable = this.accessInfo.AuthenticatedUserPrincipal.Identity as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
					this.accessInfo = null;
				}
			}
		}

		private Server FindMdbServer(ADSessionSettings settings, string serverFQDN)
		{
			ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, settings, 764, "FindMdbServer", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\ResourceHealth\\MdbSystemMailboxPinger.cs");
			Server server = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				server = session.FindServerByFqdn(serverFQDN);
			});
			if (adoperationResult.Succeeded)
			{
				return server;
			}
			ExTraceGlobals.DatabasePingerTracer.TraceError<string, Exception>((long)this.GetHashCode(), "[MdbSystmeMailboxPinger.FindMdbServer] Encountered exception looking up server by Fqdn '{0}'.  Exception: {1}", serverFQDN, adoperationResult.Exception);
			return null;
		}

		private ADSystemMailbox FindSystemMailbox(ADSessionSettings settings)
		{
			IRootOrganizationRecipientSession session = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, settings, 796, "FindSystemMailbox", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\ResourceHealth\\MdbSystemMailboxPinger.cs");
			ADRecipient[] adRecipients = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				adRecipients = session.Find(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, this.systemMailboxName), null, 1);
			});
			if (!adoperationResult.Succeeded)
			{
				throw adoperationResult.Exception;
			}
			if (adRecipients.Length != 1 || !(adRecipients[0] is ADSystemMailbox))
			{
				throw new ObjectNotFoundException(ServerStrings.AdUserNotFoundException(string.Format("SystemMailbox {0} was not found", this.systemMailboxName)));
			}
			return (ADSystemMailbox)adRecipients[0];
		}

		private const int DefaultPingTimeoutSeconds = 120;

		private const string MapiClientIdAndAction = "Client=ResourceHealth;Action=DatabasePing";

		private static readonly TimeSpan OpenSessionAttemptInterval = TimeSpan.FromSeconds(30.0);

		private static readonly PropertyDefinition[] LocalePropertyDefinition = new PropertyDefinition[]
		{
			MailboxSchema.LocaleId
		};

		private static TimeSpan pingTimeout = MdbSystemMailboxPinger.ReadPingTimeoutFromConfig();

		private ManualResetEvent remoteCallDoneEvent = new ManualResetEvent(false);

		private RegisteredWaitHandle registeredWaitHandle;

		private bool pinging;

		private Guid databaseGuid;

		private object instanceLock = new object();

		private DateTime lastSuccessfulPingUtc = DateTime.MinValue;

		private DateTime lastPingAttemptUtc = DateTime.MinValue;

		private DateTime lastSessionAttemptUtc = TimeProvider.UtcNow.Add(-MdbSystemMailboxPinger.OpenSessionAttemptInterval);

		private MdbSystemMailboxPinger.PingerState pingerState;

		private ExchangePrincipal principal;

		private MailboxAccessInfo accessInfo;

		private string systemMailboxName;

		private WaitOrTimerCallback waitOrTimerCallback;

		private enum PingerState
		{
			NotInitialized,
			NeedReinitialization,
			Normal
		}
	}
}
