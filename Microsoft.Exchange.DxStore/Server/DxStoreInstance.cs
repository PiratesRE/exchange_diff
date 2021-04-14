using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using FUSE.Paxos;
using FUSE.Paxos.Esent;
using FUSE.Paxos.Network;
using FUSE.Weld.Base;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DxStore;
using Microsoft.Exchange.DxStore.Common;

namespace Microsoft.Exchange.DxStore.Server
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
	public class DxStoreInstance : IDxStoreInstance
	{
		public DxStoreInstance(InstanceGroupConfig groupConfig, IDxStoreEventLogger eventLogger)
		{
			this.Identity = groupConfig.Identity;
			this.IdentityHash = groupConfig.Identity.GetHashCode();
			this.EventLogger = eventLogger;
			this.GroupConfig = groupConfig;
			this.LocalProcessInfo = new ProcessBasicInfo(true);
			this.StopEvent = new ManualResetEvent(false);
			this.StopCompletedEvent = new ManualResetEvent(false);
			this.State = InstanceState.Initialized;
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.InstanceTracer;
			}
		}

		public string Identity { get; private set; }

		public int IdentityHash { get; private set; }

		public IDxStoreEventLogger EventLogger { get; set; }

		public ProcessBasicInfo LocalProcessInfo { get; private set; }

		public ManualResetEvent StopEvent { get; private set; }

		public ManualResetEvent StopCompletedEvent { get; private set; }

		public bool IsStopping
		{
			get
			{
				return this.State == InstanceState.Stopping;
			}
		}

		public WCF.ServiceHostCommon InstanceServiceHost { get; set; }

		public WCF.ServiceHostCommon InstanceDefaultGroupServiceHost { get; set; }

		public ServiceHost ClientAccessServiceHost { get; set; }

		public ServiceHost ClientAccessDefaultGroupServiceHost { get; set; }

		public InstanceGroupConfig GroupConfig { get; set; }

		public ILocalDataStore LocalDataStore { get; set; }

		public DxStoreAccess StoreAccess { get; set; }

		public DxStoreStateMachine StateMachine { get; set; }

		public SnapshotManager SnapshotManager { get; set; }

		public DxStoreHealthChecker HealthChecker { get; set; }

		public LocalCommitAcknowledger CommitAcknowledger { get; set; }

		public InstanceState State { get; set; }

		public InstanceClientFactory InstanceClientFactory { get; set; }

		public AccessClientFactory AccessClientFactory { get; set; }

		public bool IsStartupCompleted { get; set; }

		public Counters PerfCounters { get; set; }

		public static bool RemoveGroupStorage(IDxStoreEventLogger eventLogger, InstanceGroupConfig group)
		{
			bool flag;
			if (Directory.Exists(group.Settings.PaxosStorageDir))
			{
				flag = (Utils.RunOperation(group.Identity, "Removing Paxos directory", delegate
				{
					Directory.Delete(group.Settings.PaxosStorageDir, true);
				}, eventLogger, LogOptions.LogException | LogOptions.LogStart | LogOptions.LogSuccess | LogOptions.LogPeriodic | group.Settings.AdditionalLogOptions, true, null, null, null, null, null) == null);
			}
			else
			{
				flag = true;
			}
			bool flag2;
			if (Directory.Exists(group.Settings.SnapshotStorageDir))
			{
				flag2 = (Utils.RunOperation(group.Identity, "Removing Snapshot directory", delegate
				{
					Directory.Delete(group.Settings.SnapshotStorageDir, true);
				}, eventLogger, LogOptions.LogException | LogOptions.LogStart | LogOptions.LogSuccess | LogOptions.LogPeriodic | group.Settings.AdditionalLogOptions, true, null, null, null, null, null) == null);
			}
			else
			{
				flag2 = true;
			}
			return flag && flag2;
		}

		public void Start()
		{
			this.RunOperation("InstanceStart", new Action(this.StartInternal), LogOptions.LogAll, null, null, null, null);
		}

		public void RegisterStoreInstanceServiceHost()
		{
			if (this.InstanceServiceHost == null || this.InstanceServiceHost.State != CommunicationState.Opened || this.InstanceServiceHost.State != CommunicationState.Opening)
			{
				WCF.ServiceHostCommon serviceHostCommon = new WCF.ServiceHostCommon(this, new Uri[0]);
				ServiceEndpoint storeInstanceEndpoint = this.GroupConfig.GetStoreInstanceEndpoint(this.GroupConfig.Self, false, true, null);
				this.AddEndPoint("Instance", serviceHostCommon, storeInstanceEndpoint);
				this.RunOperation("OpenStoreInstanceServiceHost", new Action(serviceHostCommon.Open), LogOptions.LogAll, null, null, null, null);
				this.InstanceServiceHost = serviceHostCommon;
				if (this.GroupConfig.IsDefaultGroup)
				{
					WCF.ServiceHostCommon serviceHostCommon2 = new WCF.ServiceHostCommon(this, new Uri[0]);
					ServiceEndpoint storeInstanceEndpoint2 = this.GroupConfig.GetStoreInstanceEndpoint(this.GroupConfig.Self, true, true, null);
					this.AddEndPoint("Instance (default)", serviceHostCommon2, storeInstanceEndpoint2);
					this.RunOperation("OpenStoreInstanceServiceHost (default)", new Action(serviceHostCommon2.Open), LogOptions.LogAll, null, null, null, null);
					this.InstanceServiceHost = serviceHostCommon2;
				}
			}
		}

		public void RegisterStoreAccessServiceHost()
		{
			if (this.ClientAccessServiceHost == null || this.ClientAccessServiceHost.State != CommunicationState.Opened || this.ClientAccessServiceHost.State != CommunicationState.Opening)
			{
				ServiceHost serviceHost = new ServiceHost(this.StoreAccess, new Uri[0]);
				ServiceEndpoint storeAccessEndpoint = this.GroupConfig.GetStoreAccessEndpoint(this.GroupConfig.Self, false, true, null);
				this.AddEndPoint("Store Access", serviceHost, storeAccessEndpoint);
				this.RunOperation("OpenStoreAccessServiceHost", new Action(serviceHost.Open), LogOptions.LogAll, null, null, null, null);
				this.ClientAccessServiceHost = serviceHost;
				if (this.GroupConfig.IsDefaultGroup)
				{
					ServiceHost serviceHost2 = new ServiceHost(this.StoreAccess, new Uri[0]);
					ServiceEndpoint storeAccessEndpoint2 = this.GroupConfig.GetStoreAccessEndpoint(this.GroupConfig.Self, true, true, null);
					this.AddEndPoint("Store Access (default)", serviceHost2, storeAccessEndpoint2);
					this.RunOperation("OpenStoreAccessServiceHost (default)", new Action(serviceHost2.Open), LogOptions.LogAll, null, null, null, null);
					this.ClientAccessDefaultGroupServiceHost = serviceHost2;
				}
			}
		}

		public Exception RunBestEffortOperation(string actionLabel, Action action, LogOptions options = LogOptions.LogAll, TimeSpan? timeout = null, string periodicKey = null, TimeSpan? periodicDuration = null, Action<Exception> exitAction = null)
		{
			return Utils.RunOperation(this.Identity, actionLabel, action, this.EventLogger, options | this.GroupConfig.Settings.AdditionalLogOptions, true, timeout, new TimeSpan?(periodicDuration ?? this.GroupConfig.Settings.PeriodicTimeoutLoggingDuration), periodicKey, exitAction, null);
		}

		public void RunOperation(string actionLabel, Action action, LogOptions options = LogOptions.LogAll, TimeSpan? timeout = null, string periodicKey = null, TimeSpan? periodicDuration = null, Action<Exception> exitAction = null)
		{
			Utils.RunOperation(this.Identity, actionLabel, action, this.EventLogger, options | this.GroupConfig.Settings.AdditionalLogOptions, false, timeout, new TimeSpan?(periodicDuration ?? this.GroupConfig.Settings.PeriodicTimeoutLoggingDuration), periodicKey, exitAction, null);
		}

		public void Stop(bool isExitProcess = true)
		{
			DxStoreInstance.Tracer.TraceDebug<string>((long)this.IdentityHash, "{0}: Stopping instance", this.Identity);
			this.State = InstanceState.Stopping;
			this.SnapshotManager.ForceFlush();
			this.SnapshotManager.Stop();
			this.StopEvent.Set();
			bool flag = this.RunBestEffortOperation("StopStateMachine", new Action(this.StopStateMachine), LogOptions.LogAll, new TimeSpan?(this.GroupConfig.Settings.StateMachineStopTimeout), null, null, null) == null;
			this.StopCompletedEvent.Set();
			if (isExitProcess)
			{
				int exitCode = flag ? 99 : 0;
				DxStoreInstance.Tracer.TraceWarning<string, int>((long)this.IdentityHash, "{0}: Will be exiting process after {1}ms", this.Identity, 500);
				Utils.ExitProcessDelayed(TimeSpan.FromMilliseconds(500.0), exitCode);
			}
		}

		public void CloseServiceHosts()
		{
			if (this.ClientAccessServiceHost != null)
			{
				this.RunBestEffortOperation("StoreAccessServiceHostClose", new Action(this.ClientAccessServiceHost.Close), LogOptions.LogAll, new TimeSpan?(this.GroupConfig.Settings.ServiceHostCloseTimeout), null, null, null);
				this.ClientAccessServiceHost = null;
			}
			if (this.ClientAccessDefaultGroupServiceHost != null)
			{
				this.RunBestEffortOperation("StoreAccessServiceHostClose (default group)", new Action(this.ClientAccessDefaultGroupServiceHost.Close), LogOptions.LogAll, new TimeSpan?(this.GroupConfig.Settings.ServiceHostCloseTimeout), null, null, null);
				this.ClientAccessDefaultGroupServiceHost = null;
			}
			if (this.InstanceServiceHost != null)
			{
				this.RunBestEffortOperation("StoreInstanceServiceHostClose", new Action(this.InstanceServiceHost.Close), LogOptions.LogAll, new TimeSpan?(this.GroupConfig.Settings.ServiceHostCloseTimeout), null, null, null);
				this.InstanceServiceHost = null;
			}
			if (this.InstanceDefaultGroupServiceHost != null)
			{
				this.RunBestEffortOperation("StoreInstanceServiceHostClose (default group)", new Action(this.InstanceDefaultGroupServiceHost.Close), LogOptions.LogAll, new TimeSpan?(this.GroupConfig.Settings.ServiceHostCloseTimeout), null, null, null);
				this.InstanceDefaultGroupServiceHost = null;
			}
		}

		public void ApplySnapshot(InstanceSnapshotInfo snapshot, bool isForce = false)
		{
			this.EventLogger.Log(isForce ? DxEventSeverity.Warning : DxEventSeverity.Info, 0, "{0}: Applying snapshot (isForce={1})", new object[]
			{
				this.Identity,
				isForce
			});
			if (isForce)
			{
				this.LocalDataStore.ApplySnapshot(snapshot, null);
				return;
			}
			this.EnsureInstanceIsReady();
			snapshot.Compress();
			DxStoreCommand.ApplySnapshot command = new DxStoreCommand.ApplySnapshot
			{
				SnapshotInfo = snapshot
			};
			this.StateMachine.ReplicateCommand(command, null);
		}

		public InstanceSnapshotInfo AcquireSnapshot(string fullKeyName, bool isCompress)
		{
			this.EnsureInstanceIsReady();
			bool isStale = !this.HealthChecker.IsStoreReady();
			InstanceSnapshotInfo snapshot = this.LocalDataStore.GetSnapshot(fullKeyName, isCompress);
			if (snapshot != null)
			{
				snapshot.IsStale = isStale;
			}
			return snapshot;
		}

		public void TryBecomeLeader()
		{
			this.StateMachine.BecomeLeader(this.GroupConfig.Settings.LeaderPromotionTimeout);
		}

		public void Flush()
		{
			if (this.SnapshotManager != null)
			{
				this.SnapshotManager.ForceFlush();
			}
		}

		public void Reconfigure(InstanceGroupMemberConfig[] members)
		{
			this.EnsureInstanceIsReady();
			this.GroupConfig.Members = members;
			this.StateMachine.Reconfigure(members);
			this.GroupConfig.Members = members;
		}

		public InstanceStatusInfo GetStatus()
		{
			if (this.SnapshotManager == null || !this.SnapshotManager.IsInitialLoadAttempted)
			{
				throw new DxStoreInstanceNotReadyException(this.State.ToString());
			}
			InstanceStatusInfo instanceStatusInfo = new InstanceStatusInfo();
			instanceStatusInfo.State = this.State;
			instanceStatusInfo.Self = this.GroupConfig.Self;
			instanceStatusInfo.MemberConfigs = this.GroupConfig.Members;
			if (this.LocalDataStore != null)
			{
				instanceStatusInfo.LastInstanceExecuted = this.LocalDataStore.LastInstanceExecuted;
			}
			if (this.StateMachine != null)
			{
				instanceStatusInfo.PaxosInfo = this.StateMachine.GetPaxosInfo();
			}
			instanceStatusInfo.HostProcessInfo = this.LocalProcessInfo;
			if (this.CommitAcknowledger != null)
			{
				instanceStatusInfo.CommitAckOldestItemTime = this.CommitAcknowledger.OldestItemTime;
			}
			return instanceStatusInfo;
		}

		public void NotifyInitiator(Guid commandId, string sender, int instanceNumber, bool isSucceeded, string errorMessage)
		{
			this.CommitAcknowledger.HandleAcknowledge(commandId, sender);
		}

		public Task PaxosMessageAsync(string sender, Message message)
		{
			Task result;
			try
			{
				if (this.StateMachine != null && this.StateMachine.Mesh != null)
				{
					this.StateMachine.Mesh.Incoming.OnNext(Tuple.Create<string, Message>(sender, message));
				}
				else
				{
					DxStoreInstance.Tracer.TraceError<string>(0L, "{0}: PaxosMessageAsync skipped since state machine is not configured yet.", this.GroupConfig.Identity);
				}
				result = TaskFactoryExtensions.FromResult<object>(Task.Factory, null);
			}
			catch (Exception ex)
			{
				result = TaskFactoryExtensions.FromException(Task.Factory, ex);
			}
			return result;
		}

		public void EnsureInstanceIsReady()
		{
			if (!this.IsInstanceInReadyState())
			{
				throw new DxStoreInstanceNotReadyException(this.State.ToString());
			}
		}

		public bool IsInstanceInReadyState()
		{
			return this.State == InstanceState.Running;
		}

		private HttpReply HandleIncomingMessage(HttpRequest msg)
		{
			if (msg is HttpRequest.PaxosMessage)
			{
				this.HandleIncomingPaxosMessage(msg as HttpRequest.PaxosMessage);
				return null;
			}
			if (msg is HttpRequest.GetStatusRequest)
			{
				return new HttpReply.GetInstanceStatusReply(this.GetStatus());
			}
			if (msg is HttpRequest.DxStoreRequest)
			{
				return this.HandleStoreAccess(msg as HttpRequest.DxStoreRequest);
			}
			return new HttpReply.ExceptionReply(new Exception(string.Format("Unknown request: {0}", msg.GetType().FullName)));
		}

		private HttpReply HandleStoreAccess(HttpRequest.DxStoreRequest req)
		{
			if (req.Request is DxStoreAccessRequest.CheckKey)
			{
				return new HttpReply.DxStoreReply(this.StoreAccess.CheckKey(req.Request as DxStoreAccessRequest.CheckKey));
			}
			if (req.Request is DxStoreAccessRequest.DeleteKey)
			{
				return new HttpReply.DxStoreReply(this.StoreAccess.DeleteKey(req.Request as DxStoreAccessRequest.DeleteKey));
			}
			if (req.Request is DxStoreAccessRequest.SetProperty)
			{
				return new HttpReply.DxStoreReply(this.StoreAccess.SetProperty(req.Request as DxStoreAccessRequest.SetProperty));
			}
			if (req.Request is DxStoreAccessRequest.DeleteProperty)
			{
				return new HttpReply.DxStoreReply(this.StoreAccess.DeleteProperty(req.Request as DxStoreAccessRequest.DeleteProperty));
			}
			if (req.Request is DxStoreAccessRequest.ExecuteBatch)
			{
				return new HttpReply.DxStoreReply(this.StoreAccess.ExecuteBatch(req.Request as DxStoreAccessRequest.ExecuteBatch));
			}
			if (req.Request is DxStoreAccessRequest.GetProperty)
			{
				return new HttpReply.DxStoreReply(this.StoreAccess.GetProperty(req.Request as DxStoreAccessRequest.GetProperty));
			}
			if (req.Request is DxStoreAccessRequest.GetAllProperties)
			{
				return new HttpReply.DxStoreReply(this.StoreAccess.GetAllProperties(req.Request as DxStoreAccessRequest.GetAllProperties));
			}
			if (req.Request is DxStoreAccessRequest.GetPropertyNames)
			{
				return new HttpReply.DxStoreReply(this.StoreAccess.GetPropertyNames(req.Request as DxStoreAccessRequest.GetPropertyNames));
			}
			if (req.Request is DxStoreAccessRequest.GetSubkeyNames)
			{
				return new HttpReply.DxStoreReply(this.StoreAccess.GetSubkeyNames(req.Request as DxStoreAccessRequest.GetSubkeyNames));
			}
			return new HttpReply.ExceptionReply(new Exception(string.Format("Unknown StoreAccess request: {0}", req.Request.GetType().FullName)));
		}

		private void HandleIncomingPaxosMessage(HttpRequest.PaxosMessage msg)
		{
			try
			{
				if (this.StateMachine != null && this.StateMachine.Mesh != null)
				{
					this.StateMachine.Mesh.Incoming.OnNext(Tuple.Create<string, Message>(msg.Sender, msg.Message));
				}
				else
				{
					DxStoreInstance.Tracer.TraceError<string>(0L, "{0}: HandleIncomingMessage skipped since state machine is not configured yet.", this.GroupConfig.Identity);
				}
			}
			catch (Exception arg)
			{
				DxStoreInstance.Tracer.TraceError<string, Exception>(0L, "{0}: HandleIncomingMessage caught {1}", this.GroupConfig.Identity, arg);
			}
		}

		private void StartInternal()
		{
			this.State = InstanceState.Starting;
			if (this.GroupConfig.Settings.StartupDelay != TimeSpan.Zero)
			{
				this.EventLogger.Log(DxEventSeverity.Info, 0, "Delaying instance startup for {0}", new object[]
				{
					this.GroupConfig.Settings.StartupDelay
				});
				Thread.Sleep(this.GroupConfig.Settings.StartupDelay);
			}
			this.CreateClientFactories();
			this.LocalDataStore = new LocalMemoryStore(this.Identity);
			this.SnapshotManager = new SnapshotManager(this);
			this.SnapshotManager.InitializeDataStore();
			this.StoreAccess = new DxStoreAccess(this);
			this.RegisterStoreAccessServiceHost();
			Microsoft.Exchange.DxStore.Common.EventLogger.Instance = this.EventLogger;
			if (this.GroupConfig.Settings.IsUseHttpTransportForInstanceCommunication)
			{
				HttpConfiguration.Configure(this.GroupConfig);
				this.httpListener = new DxStoreHttpListener(new Func<HttpRequest, HttpReply>(this.HandleIncomingMessage));
				Exception ex;
				if (!this.httpListener.StartListening(this.GroupConfig.Self, this.GroupConfig.Name, "B1563499-EA40-4101-A9E6-59A8EB26FF1E", out ex))
				{
					string text = string.Format("DxStoreHttpListener startup fails: {0}", ex);
					DxStoreInstance.Tracer.TraceError(0L, text);
					Microsoft.Exchange.DxStore.Common.EventLogger.LogErr(text, new object[0]);
					throw ex;
				}
			}
			this.RegisterStoreInstanceServiceHost();
			this.CommitAcknowledger = new LocalCommitAcknowledger(this);
			this.HealthChecker = new DxStoreHealthChecker(this);
			this.majorityNotificationSubscription = ObservableExtensions.Subscribe<GroupStatusInfo>(this.HealthChecker.WhenMajority, delegate(GroupStatusInfo gsi)
			{
				this.WhenHealthCheckerSeeMajorityOfNodes(gsi);
			});
			this.HealthChecker.Start();
		}

		private void WhenHealthCheckerSeeMajorityOfNodes(GroupStatusInfo gsi)
		{
			if (this.IsStartupCompleted)
			{
				return;
			}
			bool flag = false;
			try
			{
				object obj;
				Monitor.Enter(obj = this.instanceLock, ref flag);
				bool isWaitForNextRound = false;
				bool flag2 = gsi.Lag > this.GroupConfig.Settings.MaxAllowedLagToCatchup;
				DxStoreInstance.Tracer.TraceDebug((long)this.IdentityHash, "{0}: Instance start - Majority replied (LocalInstance# {1}, Lag: {2}, CatchupLimit: {3}", new object[]
				{
					this.Identity,
					(gsi.LocalInstance != null) ? gsi.LocalInstance.InstanceNumber : -1,
					gsi.Lag,
					this.GroupConfig.Settings.MaxAllowedLagToCatchup
				});
				if (flag2)
				{
					isWaitForNextRound = true;
				}
				DxStoreInstanceClient client = this.InstanceClientFactory.GetClient(gsi.HighestInstance.NodeName);
				InstanceSnapshotInfo snapshotInfo = null;
				this.RunBestEffortOperation("GetSnapshot :" + gsi.HighestInstance.NodeName, delegate
				{
					snapshotInfo = client.AcquireSnapshot(null, true, null);
				}, LogOptions.LogAll, null, null, null, null);
				if (snapshotInfo != null && snapshotInfo.LastInstanceExecuted > gsi.LocalInstance.InstanceNumber)
				{
					this.RunBestEffortOperation("Apply local snapshot", delegate
					{
						this.SnapshotManager.ApplySnapshot(snapshotInfo, true);
						isWaitForNextRound = false;
					}, LogOptions.LogAll, null, null, null, null);
				}
				if (!isWaitForNextRound)
				{
					this.majorityNotificationSubscription.Dispose();
					this.majorityNotificationSubscription = null;
					this.HealthChecker.ChangeTimerDuration(this.GroupConfig.Settings.GroupHealthCheckDuration);
					Round<string>? leaderHint = null;
					if (gsi.IsLeaderExist)
					{
						leaderHint = new Round<string>?(gsi.LeaderHint);
					}
					this.StateMachine = this.CreateStateMachine(leaderHint, null);
					Task task = this.StartStateMachine();
					task.ContinueWith(delegate(Task t)
					{
						this.EventLogger.Log(DxEventSeverity.Info, 0, "Successfully started state machine", new object[0]);
						this.SnapshotManager.Start();
						this.State = InstanceState.Running;
						this.IsStartupCompleted = true;
					});
				}
				else
				{
					DxStoreInstance.Tracer.TraceWarning<string>((long)this.IdentityHash, "{0}: Instance start - waiting for the next round to start local instance", this.Identity);
				}
			}
			finally
			{
				if (flag)
				{
					object obj;
					Monitor.Exit(obj);
				}
			}
		}

		private Dictionary<string, ServiceEndpoint> GetMembersInstanceClientEndPoints(string[] members)
		{
			Dictionary<string, ServiceEndpoint> dictionary = new Dictionary<string, ServiceEndpoint>();
			foreach (string text in members)
			{
				ServiceEndpoint storeInstanceEndpoint = this.GroupConfig.GetStoreInstanceEndpoint(text, false, false, this.GroupConfig.Settings.StoreInstanceWcfTimeout);
				dictionary[text] = storeInstanceEndpoint;
			}
			return dictionary;
		}

		private DxStoreStateMachine CreateStateMachine(Round<string>? leaderHint, PaxosBasicInfo referencePaxos)
		{
			Policy policy = new Policy
			{
				DebugName = this.GroupConfig.Self
			};
			bool flag = false;
			string[] array = (from m in this.GroupConfig.Members
			select m.Name).ToArray<string>();
			if (referencePaxos != null && referencePaxos.IsMember(this.GroupConfig.Self))
			{
				array = referencePaxos.Members;
				flag = true;
			}
			IDxStoreEventLogger eventLogger = this.EventLogger;
			DxEventSeverity severity = DxEventSeverity.Info;
			int id = 0;
			string formatString = "{0}: Creating state machine with membership '{1}' (IsUsingReferencePaxos: {2}, IsReferencePaxosLeader: {3}, GroupConfig.Members: {4}";
			object[] array2 = new object[5];
			array2[0] = this.GroupConfig.Identity;
			array2[1] = array.JoinWithComma("<null>");
			array2[2] = flag;
			array2[3] = (flag ? referencePaxos.IsLeader.ToString() : "<unknown>");
			array2[4] = (from m in this.GroupConfig.Members
			select m.Name).JoinWithComma("<null>");
			eventLogger.Log(severity, id, formatString, array2);
			Dictionary<string, ServiceEndpoint> membersInstanceClientEndPoints = this.GetMembersInstanceClientEndPoints(array);
			NodeEndPointsBase<ServiceEndpoint> nodeEndPointsBase = new NodeEndPointsBase<ServiceEndpoint>(this.GroupConfig.Self, membersInstanceClientEndPoints);
			this.PerfCounters = new Counters(this.GroupConfig.Identity);
			Configuration<string> configuration = new Configuration<string>(nodeEndPointsBase.Nodes, nodeEndPointsBase.Nodes, nodeEndPointsBase.Nodes, null);
			GroupMembersMesh mesh = new GroupMembersMesh(this.Identity, nodeEndPointsBase, this.GroupConfig);
			EsentStorage<string, DxStoreCommand> esentStorage = new EsentStorage<string, DxStoreCommand>(this.GroupConfig.Settings.PaxosStorageDir, this.PerfCounters, null, null, true);
			esentStorage.TryInitialize(configuration);
			return new DxStoreStateMachine(policy, this, nodeEndPointsBase, esentStorage, mesh, this.PerfCounters, leaderHint);
		}

		private Task StartStateMachine()
		{
			string text = string.Join(",", (from m in this.GroupConfig.Members
			select m.Name).ToArray<string>());
			this.EventLogger.Log(DxEventSeverity.Info, 0, "Starting state machine with {0} (LastInstanceExecuted = {1})", new object[]
			{
				text,
				this.LocalDataStore.LastInstanceExecuted
			});
			int num = this.LocalDataStore.LastInstanceExecuted;
			if (num > 0)
			{
				num++;
			}
			return this.StateMachine.StartAsync(num);
		}

		private void StopStateMachine()
		{
			if (this.StateMachine != null)
			{
				this.StateMachine.Stop();
			}
		}

		private void CreateClientFactories()
		{
			this.InstanceClientFactory = new InstanceClientFactory(this.GroupConfig, null);
			this.AccessClientFactory = new AccessClientFactory(this.GroupConfig, null);
		}

		private void AddEndPoint(string name, ServiceHost svcHost, ServiceEndpoint endPoint)
		{
			this.EventLogger.Log(DxEventSeverity.Info, 0, "Added endpoint {0} to service host {1}", new object[]
			{
				endPoint.Address.Uri,
				name
			});
			svcHost.AddServiceEndpoint(endPoint);
		}

		private object instanceLock = new object();

		private IDisposable majorityNotificationSubscription;

		private DxStoreHttpListener httpListener;
	}
}
