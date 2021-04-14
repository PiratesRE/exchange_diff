using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.RpcProxy;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.AdminRpc;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.DirectoryServices;
using Microsoft.Exchange.Server.Storage.WorkerManager;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Server.Storage.RpcProxy
{
	internal class RpcInstanceManager : IRpcInstanceManager
	{
		public RpcInstanceManager(IWorkerManager workerManager, int nonRecoveryDatabasesMax, int recoveryDatabasesMax, int activeDatabasesMax)
		{
			this.instances = PersistentAvlTree<Guid, RpcInstanceManager.RpcInstance>.Empty;
			this.workerManager = workerManager;
			this.workerComplete = new Action<IWorkerProcess>(this.OnWorkerComplete);
			this.notificationQueue = new Queue<RpcInstanceManager.NotificationLoop>(16);
			this.eventsProcessing = false;
			this.instanceLimitChecker = new RpcInstanceManager.InstancesLimitsChecker(nonRecoveryDatabasesMax, recoveryDatabasesMax, activeDatabasesMax, this.workerManager);
		}

		public event OnPoolNotificationsReceivedCallback NotificationsReceived;

		public event OnRpcInstanceClosedCallback RpcInstanceClosed;

		public virtual void StopAcceptingCalls()
		{
			PersistentAvlTree<Guid, RpcInstanceManager.RpcInstance> persistentAvlTree = null;
			using (LockManager.Lock(this.syncRoot))
			{
				persistentAvlTree = this.GetInstanceMap();
				Interlocked.Exchange<PersistentAvlTree<Guid, RpcInstanceManager.RpcInstance>>(ref this.instances, null);
			}
			if (persistentAvlTree != null)
			{
				foreach (RpcInstanceManager.RpcInstance rpcInstance in persistentAvlTree.GetValuesLmr())
				{
					using (LockManager.Lock(this.syncRoot))
					{
						rpcInstance.InstanceNotificationLoop.Cancelled = true;
						this.notificationQueue.Enqueue(rpcInstance.InstanceNotificationLoop);
					}
					if (rpcInstance.AdminClient != null)
					{
						rpcInstance.AdminClient.Close();
					}
					this.OnPoolNotificationReceived();
					if (rpcInstance.PoolClient != null)
					{
						rpcInstance.PoolClient.Close();
					}
				}
			}
		}

		public virtual ErrorCode StartInstance(Guid instanceId, uint flags, ref bool isNewInstanceStarted, CancellationToken cancellationToken)
		{
			IWorkerProcess workerProcess = null;
			ErrorCode errorCode = ErrorCode.NoError;
			bool flag = false;
			RpcInstanceManager.NotificationLoop notificationLoop = null;
			bool flag2 = false;
			DatabaseType databaseType = DatabaseType.None;
			isNewInstanceStarted = false;
			try
			{
				if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
				{
					ExTraceGlobals.FaultInjectionTracer.TraceTest<Guid>(2462461245U, ref instanceId);
				}
				DatabaseType databaseType2 = DatabaseType.None;
				((IRpcProxyDirectory)Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory).RefreshDatabaseInfo(NullExecutionContext.Instance, instanceId);
				DatabaseInfo databaseInfo = ((IRpcProxyDirectory)Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory).GetDatabaseInfo(NullExecutionContext.Instance, instanceId);
				string mdbName = databaseInfo.MdbName;
				bool flag3 = (flags & 8U) == 8U;
				databaseType2 = (databaseInfo.IsRecoveryDatabase ? DatabaseType.Recovery : (flag3 ? DatabaseType.Passive : DatabaseType.Active));
				using (LockManager.Lock(this.syncRoot))
				{
					if (this.IsInstanceStarted(instanceId))
					{
						flag2 = true;
						errorCode = this.workerManager.GetWorker(instanceId, out workerProcess);
						if (ErrorCode.NoError != errorCode)
						{
							return errorCode.Propagate((LID)48416U);
						}
						databaseType = workerProcess.InstanceDBType;
						workerProcess = null;
						errorCode = this.UpdateDatabaseTypeOnWorker(instanceId, mdbName, databaseType2);
						if (ErrorCode.NoError != errorCode)
						{
							return errorCode.Propagate((LID)35408U);
						}
					}
				}
				if (!flag3 && (!flag2 || (flag2 && databaseType == DatabaseType.Passive)))
				{
					NotificationItem notificationItem = new EventNotificationItem(ExchangeComponent.Store.Name, "DatabaseMountingTrigger", mdbName, ResultSeverityLevel.Error);
					notificationItem.Publish(false);
				}
				if (flag2)
				{
					return ErrorCode.NoError;
				}
				errorCode = this.instanceLimitChecker.EcCheckDatabasesLimits(instanceId, mdbName, databaseType2, false);
				if (ErrorCode.NoError != errorCode)
				{
					return errorCode.Propagate((LID)35388U);
				}
				DiagnosticContext.TraceDword((LID)33856U, (uint)Environment.TickCount);
				errorCode = this.workerManager.StartWorker(RpcInstanceManager.GetWorkerProcessPathName(RpcInstanceManager.workerProcessName), instanceId, databaseInfo.DagOrServerGuid, mdbName, this.workerComplete, cancellationToken, out workerProcess);
				DiagnosticContext.TraceDword((LID)50240U, (uint)Environment.TickCount);
				if (ErrorCode.NoError == errorCode)
				{
					using (LockManager.Lock(this.syncRoot))
					{
						errorCode = this.EcCheckDatabasesLimits(instanceId, mdbName, databaseType2, false);
						if (ErrorCode.NoError == errorCode)
						{
							workerProcess.InstanceDBType = databaseType2;
							if (ErrorCode.NoError == errorCode)
							{
								notificationLoop = this.RegisterInstance(instanceId, workerProcess);
							}
							flag = true;
						}
						else
						{
							if (ExTraceGlobals.ProxyAdminTracer.IsTraceEnabled(TraceType.ErrorTrace))
							{
								ExTraceGlobals.ProxyAdminTracer.TraceError(0L, "Failed to pass databases limits check");
							}
							DatabaseFailureItem databaseFailureItem = new DatabaseFailureItem(FailureNameSpace.Store, FailureTag.GenericMountFailure, instanceId)
							{
								ComponentName = "RpcProxy",
								InstanceName = mdbName
							};
							databaseFailureItem.Publish();
						}
					}
					if (notificationLoop != null)
					{
						notificationLoop.StateMachine.MoveNext();
					}
				}
			}
			catch (DatabaseNotFoundException ex)
			{
				NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(ex);
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_NoDatabase, new object[]
				{
					ex
				});
				errorCode = ErrorCode.CreateNotFound((LID)55536U);
			}
			catch (DirectoryTransientErrorException ex2)
			{
				NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(ex2);
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_TransientADError, new object[]
				{
					ex2
				});
				errorCode = ErrorCode.CreateAdUnavailable((LID)51440U);
			}
			catch (DirectoryPermanentErrorException ex3)
			{
				NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(ex3);
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_PermanentADError, new object[]
				{
					ex3
				});
				errorCode = ErrorCode.CreateADPropertyError((LID)45296U);
			}
			catch (StoreException ex4)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex4);
				throw new FailRpcException(ex4.Message, (int)ex4.Error);
			}
			finally
			{
				if (!flag && workerProcess != null)
				{
					this.workerManager.StopWorker(workerProcess.InstanceId, cancellationToken.IsCancellationRequested);
				}
			}
			isNewInstanceStarted = flag;
			return errorCode;
		}

		public virtual void StopInstance(Guid instanceId, bool terminate)
		{
			RpcInstanceManager.RpcClient<AdminRpcClient> rpcClient = null;
			RpcInstanceManager.RpcClient<RpcInstancePool> rpcClient2 = null;
			try
			{
				using (LockManager.Lock(this.syncRoot))
				{
					PersistentAvlTree<Guid, RpcInstanceManager.RpcInstance> persistentAvlTree = this.GetInstanceMap();
					RpcInstanceManager.RpcInstance rpcInstance;
					if (persistentAvlTree != null && persistentAvlTree.TryGetValue(instanceId, out rpcInstance))
					{
						persistentAvlTree = persistentAvlTree.Remove(instanceId);
						Interlocked.Exchange<PersistentAvlTree<Guid, RpcInstanceManager.RpcInstance>>(ref this.instances, persistentAvlTree);
						rpcClient = rpcInstance.AdminClient;
						rpcClient2 = rpcInstance.PoolClient;
						rpcInstance.InstanceNotificationLoop.Cancelled = true;
						this.notificationQueue.Enqueue(rpcInstance.InstanceNotificationLoop);
					}
				}
				DiagnosticContext.TraceDword((LID)47168U, (uint)Environment.TickCount);
				this.workerManager.StopWorker(instanceId, terminate);
				DiagnosticContext.TraceDword((LID)63552U, (uint)Environment.TickCount);
			}
			finally
			{
				if (rpcClient != null)
				{
					rpcClient.Close();
				}
				this.OnPoolNotificationReceived();
				if (rpcClient2 != null)
				{
					rpcClient2.Close();
				}
			}
		}

		public virtual bool IsInstanceStarted(Guid instanceId)
		{
			IWorkerProcess workerProcess;
			ErrorCode worker = this.workerManager.GetWorker(instanceId, out workerProcess);
			if (ErrorCode.NoError == worker)
			{
				PersistentAvlTree<Guid, RpcInstanceManager.RpcInstance> instanceMap = this.GetInstanceMap();
				return instanceMap != null && instanceMap.Contains(instanceId);
			}
			return false;
		}

		public virtual string GetInstanceDisplayName(Guid instanceId)
		{
			IWorkerProcess workerProcess;
			ErrorCode worker = this.workerManager.GetWorker(instanceId, out workerProcess);
			if (ErrorCode.NoError == worker)
			{
				return workerProcess.InstanceName;
			}
			return instanceId.ToString();
		}

		public virtual RpcInstanceManager.AdminCallGuard GetAdminRpcClient(Guid instanceId, string functionName, out AdminRpcClient adminRpc)
		{
			adminRpc = null;
			RpcInstanceManager.RpcClient<AdminRpcClient> client = null;
			int workerPid = -1;
			IWorkerProcess workerProcess;
			ErrorCode worker = this.workerManager.GetWorker(instanceId, out workerProcess);
			if (ErrorCode.NoError == worker)
			{
				workerPid = workerProcess.ProcessId;
				PersistentAvlTree<Guid, RpcInstanceManager.RpcInstance> instanceMap = this.GetInstanceMap();
				RpcInstanceManager.RpcInstance rpcInstance;
				if (instanceMap != null && instanceMap.TryGetValue(instanceId, out rpcInstance))
				{
					client = rpcInstance.AdminClient.BeginCall(out adminRpc);
				}
			}
			return new RpcInstanceManager.AdminCallGuard(client, functionName, instanceId, workerPid);
		}

		public virtual RpcInstanceManager.RpcClient<RpcInstancePool> GetPoolRpcClient(Guid instanceId, ref int generation, out RpcInstancePool rpcClient)
		{
			rpcClient = null;
			IWorkerProcess workerProcess;
			ErrorCode worker = this.workerManager.GetWorker(instanceId, out workerProcess);
			if (ErrorCode.NoError != worker)
			{
				return null;
			}
			if (generation != 0 && generation != workerProcess.Generation)
			{
				return null;
			}
			PersistentAvlTree<Guid, RpcInstanceManager.RpcInstance> instanceMap = this.GetInstanceMap();
			RpcInstanceManager.RpcInstance rpcInstance;
			if (instanceMap == null || !instanceMap.TryGetValue(instanceId, out rpcInstance))
			{
				return null;
			}
			if (generation != 0 && rpcInstance.Generation != workerProcess.Generation)
			{
				return null;
			}
			generation = workerProcess.Generation;
			return rpcInstance.PoolClient.BeginCall(out rpcClient);
		}

		public virtual IEnumerable<Guid> GetActiveInstances()
		{
			PersistentAvlTree<Guid, RpcInstanceManager.RpcInstance> map = this.GetInstanceMap();
			if (map != null)
			{
				foreach (Guid id in map.GetKeysLmr())
				{
					yield return id;
				}
			}
			yield break;
		}

		protected virtual RpcInstanceManager.NotificationLoop RegisterInstance(Guid instanceId, IWorkerProcess worker)
		{
			RpcInstanceManager.NotificationLoop notificationLoop = null;
			PersistentAvlTree<Guid, RpcInstanceManager.RpcInstance> persistentAvlTree = this.GetInstanceMap();
			if (persistentAvlTree != null && !persistentAvlTree.Contains(instanceId))
			{
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					AdminRpcClient adminRpcClient = new AdminRpcClient("localhost", instanceId);
					disposeGuard.Add<AdminRpcClient>(adminRpcClient);
					RpcInstancePool rpcInstancePool = new RpcInstancePool(instanceId, worker.Generation);
					disposeGuard.Add<RpcInstancePool>(rpcInstancePool);
					notificationLoop = new RpcInstanceManager.NotificationLoop();
					notificationLoop.StateMachine = this.InstanceNotificationLoop(instanceId, worker.Generation, notificationLoop);
					persistentAvlTree = persistentAvlTree.Add(instanceId, new RpcInstanceManager.RpcInstance(worker.Generation, adminRpcClient, rpcInstancePool, notificationLoop));
					Interlocked.Exchange<PersistentAvlTree<Guid, RpcInstanceManager.RpcInstance>>(ref this.instances, persistentAvlTree);
					disposeGuard.Success();
				}
			}
			return notificationLoop;
		}

		private static string GetWorkerProcessPathName(string imageFileName)
		{
			string fileName = Process.GetCurrentProcess().MainModule.FileName;
			string directoryName = Path.GetDirectoryName(fileName);
			return directoryName + "\\" + imageFileName;
		}

		private ErrorCode EcCheckDatabasesLimits(Guid mdbGuid, string databaseName, DatabaseType databaseType, bool databaseStateTransition)
		{
			return this.instanceLimitChecker.EcCheckDatabasesLimits(mdbGuid, databaseName, databaseType, databaseStateTransition);
		}

		private ErrorCode UpdateDatabaseTypeOnWorker(Guid instanceId, string workerName, DatabaseType databaseTypeFinal)
		{
			ErrorCode second = ErrorCode.NoError;
			IWorkerProcess workerProcess;
			second = this.workerManager.GetWorker(instanceId, out workerProcess);
			if (ErrorCode.NoError != second)
			{
				return second.Propagate((LID)51792U);
			}
			if (DatabaseType.Passive == workerProcess.InstanceDBType && DatabaseType.Active == databaseTypeFinal)
			{
				second = this.instanceLimitChecker.EcCheckDatabasesLimits(instanceId, workerName, databaseTypeFinal, true);
				if (ErrorCode.NoError != second)
				{
					return second.Propagate((LID)48432U);
				}
				workerProcess.InstanceDBType = databaseTypeFinal;
			}
			else if (ExTraceGlobals.ProxyAdminTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				ExTraceGlobals.ProxyAdminTracer.TraceFunction(0L, string.Format("Database state transition: {0} -> {1}.", workerProcess.InstanceDBType, databaseTypeFinal));
			}
			return ErrorCode.NoError;
		}

		private PersistentAvlTree<Guid, RpcInstanceManager.RpcInstance> GetInstanceMap()
		{
			return Interlocked.CompareExchange<PersistentAvlTree<Guid, RpcInstanceManager.RpcInstance>>(ref this.instances, null, null);
		}

		private void OnWorkerComplete(IWorkerProcess worker)
		{
			RpcInstanceManager.RpcClient<AdminRpcClient> rpcClient = null;
			RpcInstanceManager.RpcClient<RpcInstancePool> rpcClient2 = null;
			try
			{
				using (LockManager.Lock(this.syncRoot))
				{
					PersistentAvlTree<Guid, RpcInstanceManager.RpcInstance> persistentAvlTree = this.GetInstanceMap();
					RpcInstanceManager.RpcInstance rpcInstance;
					if (persistentAvlTree != null && persistentAvlTree.TryGetValue(worker.InstanceId, out rpcInstance) && worker.Generation == rpcInstance.Generation)
					{
						persistentAvlTree = persistentAvlTree.Remove(worker.InstanceId);
						Interlocked.Exchange<PersistentAvlTree<Guid, RpcInstanceManager.RpcInstance>>(ref this.instances, persistentAvlTree);
						rpcClient = rpcInstance.AdminClient;
						rpcClient2 = rpcInstance.PoolClient;
					}
				}
			}
			finally
			{
				if (rpcClient != null)
				{
					rpcClient.Close();
				}
				if (rpcClient2 != null)
				{
					rpcClient2.Close();
				}
				OnRpcInstanceClosedCallback rpcInstanceClosed = this.RpcInstanceClosed;
				if (rpcInstanceClosed != null)
				{
					rpcInstanceClosed(worker.InstanceId, worker.Generation);
				}
			}
		}

		private IEnumerator<ErrorCode> InstanceNotificationLoop(Guid workerInstance, int generation, RpcInstanceManager.NotificationLoop loop)
		{
			RpcInstanceManager.<InstanceNotificationLoop>d__a <InstanceNotificationLoop>d__a = new RpcInstanceManager.<InstanceNotificationLoop>d__a(0);
			<InstanceNotificationLoop>d__a.<>4__this = this;
			<InstanceNotificationLoop>d__a.workerInstance = workerInstance;
			<InstanceNotificationLoop>d__a.generation = generation;
			<InstanceNotificationLoop>d__a.loop = loop;
			return <InstanceNotificationLoop>d__a;
		}

		private void OnPoolNotificationReceived()
		{
			RpcInstanceManager.NotificationLoop notificationLoop = null;
			using (LockManager.Lock(this.syncRoot))
			{
				if (this.eventsProcessing)
				{
					return;
				}
				if (this.notificationQueue.Count == 0)
				{
					return;
				}
				notificationLoop = this.notificationQueue.Dequeue();
				this.eventsProcessing = true;
			}
			bool flag = false;
			do
			{
				try
				{
					notificationLoop.StateMachine.MoveNext();
				}
				finally
				{
					using (LockManager.Lock(this.syncRoot))
					{
						if (this.notificationQueue.Count > 0)
						{
							notificationLoop = this.notificationQueue.Dequeue();
						}
						else
						{
							this.eventsProcessing = false;
							flag = true;
						}
					}
				}
			}
			while (!flag);
		}

		private static string workerProcessName = "Microsoft.Exchange.Store.Worker.exe";

		private PersistentAvlTree<Guid, RpcInstanceManager.RpcInstance> instances;

		private object syncRoot = new object();

		private Action<IWorkerProcess> workerComplete;

		private IWorkerManager workerManager;

		private Queue<RpcInstanceManager.NotificationLoop> notificationQueue;

		private bool eventsProcessing;

		private RpcInstanceManager.InstancesLimitsChecker instanceLimitChecker;

		internal struct AdminCallGuard : IDisposable
		{
			internal AdminCallGuard(RpcInstanceManager.RpcClient<AdminRpcClient> client, string rpcName, Guid workerInstance, int workerPid)
			{
				this.client = client;
				this.rpcName = rpcName;
				this.workerInstance = workerInstance;
				this.workerPid = workerPid;
				if (client != null && ExTraceGlobals.ProxyAdminTracer.IsTraceEnabled(TraceType.FunctionTrace))
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					stringBuilder.Append("ENTER CALL PROXY [ADMIN][");
					stringBuilder.Append(rpcName);
					stringBuilder.Append("] pid:[");
					stringBuilder.Append(workerPid);
					stringBuilder.Append("] database:[");
					stringBuilder.Append(workerInstance.ToString());
					stringBuilder.Append("]");
					ExTraceGlobals.ProxyAdminTracer.TraceFunction(0L, stringBuilder.ToString());
				}
			}

			public void Dispose()
			{
				if (this.client != null)
				{
					this.client.EndCall();
					if (ExTraceGlobals.ProxyAdminTracer.IsTraceEnabled(TraceType.FunctionTrace))
					{
						StringBuilder stringBuilder = new StringBuilder(100);
						stringBuilder.Append("EXIT CALL PROXY [ADMIN][");
						stringBuilder.Append(this.rpcName);
						stringBuilder.Append(this.rpcName);
						stringBuilder.Append("] pid:[");
						stringBuilder.Append(this.workerPid);
						stringBuilder.Append("] database:[");
						stringBuilder.Append(this.workerInstance.ToString());
						stringBuilder.Append("]");
						ExTraceGlobals.ProxyAdminTracer.TraceFunction(0L, stringBuilder.ToString());
					}
				}
			}

			private RpcInstanceManager.RpcClient<AdminRpcClient> client;

			private string rpcName;

			private Guid workerInstance;

			private int workerPid;
		}

		private struct RpcInstance
		{
			public RpcInstance(int generation, AdminRpcClient adminClient, RpcInstancePool poolClient, RpcInstanceManager.NotificationLoop notificationLoop)
			{
				this.Generation = generation;
				this.AdminClient = new RpcInstanceManager.RpcClient<AdminRpcClient>(adminClient);
				this.PoolClient = new RpcInstanceManager.RpcClient<RpcInstancePool>(poolClient);
				this.InstanceNotificationLoop = notificationLoop;
			}

			public RpcInstanceManager.RpcClient<AdminRpcClient> AdminClient;

			public RpcInstanceManager.RpcClient<RpcInstancePool> PoolClient;

			public int Generation;

			public RpcInstanceManager.NotificationLoop InstanceNotificationLoop;
		}

		internal class RpcClient<TClient> where TClient : class, IDisposable
		{
			public RpcClient(TClient client)
			{
				this.activeCalls = 0L;
				this.client = client;
				this.closed = false;
			}

			public RpcInstanceManager.RpcClient<TClient> BeginCall(out TClient client)
			{
				client = default(TClient);
				using (LockManager.Lock(this))
				{
					if (!this.closed)
					{
						client = this.client;
						this.activeCalls += 1L;
						return this;
					}
				}
				return null;
			}

			public void EndCall()
			{
				using (LockManager.Lock(this))
				{
					this.activeCalls -= 1L;
					if (this.closed && this.client != null && this.activeCalls == 0L)
					{
						this.client.Dispose();
						this.client = default(TClient);
					}
				}
			}

			public void Close()
			{
				using (LockManager.Lock(this))
				{
					if (!this.closed)
					{
						this.closed = true;
						if (this.activeCalls == 0L)
						{
							this.client.Dispose();
							this.client = default(TClient);
						}
					}
				}
			}

			private long activeCalls;

			private TClient client;

			private bool closed;
		}

		internal class NotificationLoop
		{
			public IEnumerator<ErrorCode> StateMachine { get; set; }

			public RpcException LastRpcException { get; set; }

			public bool Cancelled
			{
				get
				{
					return this.cancellation.IsCancellationRequested;
				}
				set
				{
					this.cancellation.Cancel();
				}
			}

			public CancellationToken Token
			{
				get
				{
					return this.cancellation.Token;
				}
			}

			private CancellationTokenSource cancellation = new CancellationTokenSource();
		}

		private class InstancesLimitsChecker
		{
			public InstancesLimitsChecker(int nonRecoveryDatabasesMax, int recoveryDatabasesMax, int activeDatabasesMax, IWorkerManager workermanager)
			{
				this.nonRecoveryDatabasesMax = nonRecoveryDatabasesMax;
				this.recoveryDatabasesMax = recoveryDatabasesMax;
				this.activeDatabasesMax = activeDatabasesMax;
				this.workerManager = workermanager;
			}

			public ErrorCode EcCheckDatabasesLimits(Guid instanceId, string databaseName, DatabaseType databaseType, bool databaseStateTransition)
			{
				ErrorCode errorCode = ErrorCode.NoError;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				foreach (IWorkerProcess workerProcess in this.workerManager.GetActiveWorkers())
				{
					if (!databaseStateTransition || !(workerProcess.InstanceId == instanceId))
					{
						switch (workerProcess.InstanceDBType)
						{
						case DatabaseType.Active:
							num2++;
							break;
						case DatabaseType.Passive:
							num3++;
							break;
						case DatabaseType.Recovery:
							num++;
							break;
						}
					}
				}
				int num4 = num2 + num3;
				ErrorHelper.AssertRetail(num >= 0 && num <= this.recoveryDatabasesMax, "Invalid number of recovey databases.");
				ErrorHelper.AssertRetail(num2 >= 0 && num2 <= this.activeDatabasesMax, "Invalid number of active databases.");
				ErrorHelper.AssertRetail(num4 >= 0 && num4 <= this.nonRecoveryDatabasesMax, "Invalid number of non-recovery databases.");
				if (databaseType == DatabaseType.Recovery)
				{
					errorCode = this.EcCheckRecoveryDatabasesCount(instanceId, databaseName, num);
					if (ErrorCode.NoError != errorCode)
					{
						return errorCode.Propagate((LID)35056U);
					}
				}
				else
				{
					if (DatabaseType.Active == databaseType)
					{
						errorCode = this.EcCheckActiveDatabasesCount(instanceId, databaseName, num2);
						if (ErrorCode.NoError != errorCode)
						{
							return errorCode.Propagate((LID)59632U);
						}
					}
					errorCode = this.EcCheckNonRecoveryDatabasesCount(instanceId, databaseName, num4);
					if (ErrorCode.NoError != errorCode)
					{
						return errorCode.Propagate((LID)43248U);
					}
				}
				return errorCode;
			}

			private ErrorCode EcCheckActiveDatabasesCount(Guid mdbGuid, string databaseName, int activeDatabasesCount)
			{
				ErrorCode result = ErrorCode.NoError;
				if (activeDatabasesCount >= this.activeDatabasesMax)
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_TooManyActiveMDBsMounted, new object[]
					{
						this.activeDatabasesMax
					});
					this.PublishFailureItem(mdbGuid, databaseName, FailureTag.ExceededMaxActiveDatabases);
					result = ErrorCode.CreateWithLid((LID)42480U, ErrorCodeValue.TooManyMountedDatabases);
				}
				return result;
			}

			private ErrorCode EcCheckNonRecoveryDatabasesCount(Guid mdbGuid, string databaseName, int nonRecoveryDatabasesCount)
			{
				ErrorCode result = ErrorCode.NoError;
				if (nonRecoveryDatabasesCount >= this.nonRecoveryDatabasesMax)
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_TooManyMDBsMounted, new object[]
					{
						this.nonRecoveryDatabasesMax
					});
					this.PublishFailureItem(mdbGuid, databaseName, FailureTag.ExceededMaxDatabases);
					result = ErrorCode.CreateWithLid((LID)58864U, ErrorCodeValue.TooManyMountedDatabases);
				}
				return result;
			}

			private ErrorCode EcCheckRecoveryDatabasesCount(Guid mdbGuid, string databaseName, int recoveryDatabasesCount)
			{
				ErrorCode result = ErrorCode.NoError;
				if (recoveryDatabasesCount >= this.recoveryDatabasesMax)
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_TooManyRecoveryMDBsMounted, new object[]
					{
						this.recoveryDatabasesMax
					});
					this.PublishFailureItem(mdbGuid, databaseName, FailureTag.ExceededMaxDatabases);
					result = ErrorCode.CreateWithLid((LID)54768U, ErrorCodeValue.TooManyMountedDatabases);
				}
				return result;
			}

			private void PublishFailureItem(Guid databaseGuid, string databaseName, FailureTag failureTag)
			{
				DatabaseFailureItem databaseFailureItem = new DatabaseFailureItem(FailureNameSpace.Store, failureTag, databaseGuid)
				{
					ComponentName = "RpcProxy",
					InstanceName = databaseName
				};
				databaseFailureItem.Publish();
			}

			private readonly int nonRecoveryDatabasesMax;

			private readonly int recoveryDatabasesMax;

			private readonly int activeDatabasesMax;

			private IWorkerManager workerManager;
		}
	}
}
