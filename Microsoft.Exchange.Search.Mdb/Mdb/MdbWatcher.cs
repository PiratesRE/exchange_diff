using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Mdb
{
	internal sealed class MdbWatcher : IDisposeTrackable, IMdbWatcher, IDisposable
	{
		public MdbWatcher()
		{
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession(base.GetType().Name, ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.MdbWatcherTracer, (long)this.GetHashCode());
			this.localServer = AdDataProvider.Create(this.diagnosticsSession).GetLocalServer();
			this.dbCache = DatabaseCache.Create(this.diagnosticsSession);
			this.RegisterDatabaseChangeNotification(new ADNotificationCallback(this.ADCallback));
			this.diagnosticsSession.TraceDebug<string>("Successfully found mailbox server object: {0}", this.localServer.Fqdn);
			this.disposeTracker = this.GetDisposeTracker();
		}

		public event EventHandler Changed;

		public IMdbCollection GetDatabases()
		{
			return new MdbWatcher.MdbCollection(this.localServer, this.diagnosticsSession);
		}

		public bool Exists(Guid mdbGuid)
		{
			return this.dbCache.DatabaseExists(mdbGuid);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MdbWatcher>(this);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				this.UnregisterDatabaseChangeNotification();
			}
		}

		private void OnChanged(EventArgs e)
		{
			EventHandler eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.Changed, null, null);
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		private void RegisterDatabaseChangeNotification(ADNotificationCallback notifyDatabasesChanged)
		{
			Util.ThrowOnNullArgument(notifyDatabasesChanged, "notifyDatabasesChanged");
			try
			{
				AdDataProvider adDataProvider = AdDataProvider.Create(this.diagnosticsSession);
				this.cookie = adDataProvider.RegisterChangeNotification(notifyDatabasesChanged);
			}
			catch (ComponentException arg)
			{
				this.diagnosticsSession.TraceError<ComponentException>("Failed to register database change notification. Exception={0}", arg);
			}
		}

		private void UnregisterDatabaseChangeNotification()
		{
			try
			{
				AdDataProvider adDataProvider = AdDataProvider.Create(this.diagnosticsSession);
				adDataProvider.UnRegisterChangeNotification(this.cookie);
			}
			catch (ComponentException arg)
			{
				this.diagnosticsSession.TraceError<ComponentException>("Failed to unregister database change notification. Exception={0}", arg);
			}
		}

		private void ADCallback(ADNotificationEventArgs args)
		{
			this.OnChanged(EventArgs.Empty);
		}

		private readonly Server localServer;

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly DatabaseCache dbCache;

		private DisposeTracker disposeTracker;

		private ADNotificationRequestCookie cookie;

		internal class MdbCollection : IMdbCollection
		{
			internal MdbCollection(Server localServer, IDiagnosticsSession diagnosticsSession)
			{
				this.localServer = localServer;
				this.diagnosticsSession = diagnosticsSession;
			}

			public IEnumerable<MdbInfo> Databases
			{
				get
				{
					return this.MdbInfos;
				}
			}

			private MailboxDatabase[] MailboxDatabases
			{
				get
				{
					if (this.mailboxDatabases == null)
					{
						AdDataProvider adDataProvider = AdDataProvider.Create(this.diagnosticsSession);
						this.mailboxDatabases = adDataProvider.GetLocalMailboxDatabases(this.localServer);
					}
					return this.mailboxDatabases;
				}
			}

			private MdbInfo[] MdbInfos
			{
				get
				{
					if (this.mdbInfos == null)
					{
						this.mdbInfos = new MdbInfo[this.MailboxDatabases.Length];
						for (int i = 0; i < this.MailboxDatabases.Length; i++)
						{
							this.mdbInfos[i] = new MdbInfo(this.MailboxDatabases[i]);
						}
					}
					return this.mdbInfos;
				}
			}

			public void UpdateDatabasesIndexStatusInfo(int numberOfCopiesToIndexPerDatabase)
			{
				ServerSchemaVersionSource serverSchemaVersionSource = new ServerSchemaVersionSource(this.localServer.Guid, this.diagnosticsSession);
				HashSet<Guid> hashSet = new HashSet<Guid>();
				foreach (MailboxDatabase mailboxDatabase in this.MailboxDatabases)
				{
					foreach (KeyValuePair<ADObjectId, int> keyValuePair in mailboxDatabase.ActivationPreference)
					{
						if (keyValuePair.Key.ObjectGuid != this.localServer.Guid)
						{
							hashSet.Add(keyValuePair.Key.ObjectGuid);
						}
					}
				}
				serverSchemaVersionSource.LoadVersions(hashSet);
				for (int k = 0; k < this.MailboxDatabases.Length; k++)
				{
					MailboxDatabase mailboxDatabase2 = this.MailboxDatabases[k];
					MdbInfo mdbInfo = this.MdbInfos[k];
					this.diagnosticsSession.Assert(object.Equals(mdbInfo.Guid, mailboxDatabase2.Guid), "MdbGuid must match", new object[0]);
					mdbInfo.ActivationPreference = this.GetOrdinalOfDatabaseCopyOnLocalServer(mailboxDatabase2);
					mdbInfo.PreferredActiveCopy = (mdbInfo.ActivationPreference == 1);
					mdbInfo.DatabaseCopies = MdbWatcher.MdbCollection.GetDatabaseCopies(serverSchemaVersionSource, mailboxDatabase2.ActivationPreference);
					mdbInfo.MaxSupportedVersion = MdbWatcher.MdbCollection.GetMaxFeedingVersion(mdbInfo.DatabaseCopies);
					if (mdbInfo.ActivationPreference > numberOfCopiesToIndexPerDatabase)
					{
						this.diagnosticsSession.TraceDebug<MailboxDatabase, int, int>("The ordinal of '{0}' copy on this server is {1} (> Threshold {2})", mailboxDatabase2, mdbInfo.ActivationPreference, numberOfCopiesToIndexPerDatabase);
						mdbInfo.NotIndexed = IndexStatusErrorCode.ActivationPreferenceSkipped;
					}
					else if (mailboxDatabase2.Recovery)
					{
						this.diagnosticsSession.TraceDebug<MailboxDatabase>("'{0}' is a recovery mailbox database", mailboxDatabase2);
						mdbInfo.NotIndexed = IndexStatusErrorCode.RecoveryDatabaseSkipped;
					}
					else if (!mailboxDatabase2.IndexEnabled)
					{
						this.diagnosticsSession.TraceDebug<MailboxDatabase>("'{0}' is not index-enabled", mailboxDatabase2);
						mdbInfo.NotIndexed = IndexStatusErrorCode.IndexNotEnabled;
					}
					else
					{
						mdbInfo.NotIndexed = IndexStatusErrorCode.Unknown;
					}
				}
			}

			public void UpdateDatabasesCopyStatusInfo()
			{
				Guid[] dbGuids = new Guid[this.MdbInfos.Length];
				for (int i = 0; i < this.MdbInfos.Length; i++)
				{
					dbGuids[i] = this.MdbInfos[i].Guid;
				}
				RpcDatabaseCopyStatus2[] statusResults = null;
				RpcErrorExceptionInfo errorInfo = null;
				TasksRpcExceptionWrapper.Instance.ClientRetryableOperation(this.localServer.Fqdn, delegate
				{
					using (ReplayRpcClient replayRpcClient = new ReplayRpcClient(this.localServer.Fqdn))
					{
						this.diagnosticsSession.TraceDebug<string>("Now making RpccGetCopyStatusBasic() RPC to server {0}.", this.localServer.Fqdn);
						errorInfo = replayRpcClient.RpccGetCopyStatusEx4(RpcGetDatabaseCopyStatusFlags2.None, dbGuids, ref statusResults);
					}
				});
				TasksRpcExceptionWrapper.Instance.ClientRethrowIfFailed(null, this.localServer.Fqdn, errorInfo);
				if (statusResults == null || statusResults.Length == 0)
				{
					this.diagnosticsSession.TraceDebug<string>("No CopyStatus returned for server {0}.", this.localServer.Fqdn);
					return;
				}
				Dictionary<Guid, RpcDatabaseCopyStatus2> dictionary = new Dictionary<Guid, RpcDatabaseCopyStatus2>(statusResults.Length);
				for (int j = 0; j < statusResults.Length; j++)
				{
					dictionary[statusResults[j].DBGuid] = statusResults[j];
				}
				for (int k = 0; k < this.MdbInfos.Length; k++)
				{
					MdbInfo mdbInfo = this.MdbInfos[k];
					RpcDatabaseCopyStatus2 rpcDatabaseCopyStatus;
					if (dictionary.TryGetValue(mdbInfo.Guid, out rpcDatabaseCopyStatus))
					{
						this.diagnosticsSession.TraceDebug<CopyStatusEnum, MdbInfo>("Get CopyStatus '{0}' for mdb {1}", rpcDatabaseCopyStatus.CopyStatus, mdbInfo);
						switch (rpcDatabaseCopyStatus.CopyStatus)
						{
						case CopyStatusEnum.FailedAndSuspended:
						case CopyStatusEnum.Suspended:
							mdbInfo.IsSuspended = true;
							break;
						case CopyStatusEnum.Seeding:
							goto IL_18B;
						case CopyStatusEnum.Mounting:
						case CopyStatusEnum.Mounted:
							mdbInfo.MountedOnLocalServer = true;
							break;
						default:
							goto IL_18B;
						}
						IL_19B:
						mdbInfo.IsLagCopy = (rpcDatabaseCopyStatus.ConfiguredReplayLagTime > TimeSpan.Zero);
						goto IL_1C7;
						IL_18B:
						mdbInfo.IsSuspended = false;
						mdbInfo.MountedOnLocalServer = false;
						goto IL_19B;
					}
					this.diagnosticsSession.TraceDebug<MdbInfo>("GetCopyStatus() didn't find replica instance for mdb {0}", mdbInfo);
					IL_1C7:;
				}
			}

			internal static List<MdbCopy> GetDatabaseCopies(ServerSchemaVersionSource serverSchemaVersionSource, KeyValuePair<ADObjectId, int>[] servers)
			{
				List<MdbCopy> list = new List<MdbCopy>(servers.Length);
				foreach (KeyValuePair<ADObjectId, int> keyValuePair in servers)
				{
					int serverVersion = serverSchemaVersionSource.GetServerVersion(keyValuePair.Key.ObjectGuid);
					list.Add(new MdbCopy(keyValuePair.Key.Name, keyValuePair.Value, serverVersion));
				}
				return list;
			}

			internal static int GetMaxFeedingVersion(ICollection<MdbCopy> copies)
			{
				VersionInfo latest = VersionInfo.Latest;
				int num = latest.FeedingVersion;
				foreach (MdbCopy mdbCopy in copies)
				{
					num = Math.Min(num, mdbCopy.SchemaVersion);
				}
				return num;
			}

			private int GetOrdinalOfDatabaseCopyOnLocalServer(MailboxDatabase mailboxDatabase)
			{
				int num = -1;
				foreach (KeyValuePair<ADObjectId, int> keyValuePair in mailboxDatabase.ActivationPreference)
				{
					ADObjectId key = keyValuePair.Key;
					if (key.Equals(this.localServer.Identity))
					{
						num = keyValuePair.Value;
						break;
					}
				}
				if (num < 0)
				{
					throw new InvalidOperationException(string.Format("The database '{0}' has no ActivationPreference on server '{1}'", mailboxDatabase, this.localServer.Fqdn));
				}
				int num2 = 1;
				foreach (KeyValuePair<ADObjectId, int> keyValuePair2 in mailboxDatabase.ActivationPreference)
				{
					if (keyValuePair2.Value < num)
					{
						num2++;
					}
				}
				return num2;
			}

			private readonly Server localServer;

			private readonly IDiagnosticsSession diagnosticsSession;

			private MailboxDatabase[] mailboxDatabases;

			private MdbInfo[] mdbInfos;
		}
	}
}
