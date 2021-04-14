using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Exchange.DxStore.HA.Events;

namespace Microsoft.Exchange.Cluster.Shared
{
	public class DistributedStore
	{
		public DistributedStore(string componentName)
		{
			this.IsRestartProcessOnDxStoreModeChange = true;
			this.StoreSettings = DataStoreSettings.GetStoreConfig();
			this.PerfTracker = new PerformanceTracker();
			if (this.StoreSettings.Primary == StoreKind.DxStore || this.StoreSettings.Shadow == StoreKind.DxStore)
			{
				this.DxStoreKeyFactoryInstance = new DxStoreKeyFactory(componentName, new Func<Exception, Exception>(this.ConstructClusterApiException), null, null, null, false);
			}
			this.BaseKeyGenerator = new Func<DxStoreKeyAccessMode, DistributedStore.Context, StoreKind, IDistributedStoreKey>(this.GetBaseKeyByStoreKind);
		}

		public static DistributedStore Instance
		{
			get
			{
				if (DistributedStore.instance == null)
				{
					lock (DistributedStore.instanceLock)
					{
						if (DistributedStore.instance == null)
						{
							DistributedStore.instance = new DistributedStore("ExchangeHA");
							DistributedStore.instance.PerfTracker.Start();
						}
					}
				}
				return DistributedStore.instance;
			}
			set
			{
				DistributedStore.instance = value;
			}
		}

		public bool IsRestartProcessOnDxStoreModeChange { get; set; }

		public Exception ConstructClusterApiException(Exception ex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (ex != null)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append(ex.Message);
				ex = ex.InnerException;
			}
			return new ClusterApiException(stringBuilder.ToString());
		}

		public Func<DxStoreKeyAccessMode, DistributedStore.Context, StoreKind, IDistributedStoreKey> BaseKeyGenerator { get; set; }

		public bool IsStopping { get; set; }

		public PerformanceTracker PerfTracker { get; set; }

		public DataStoreSettings StoreSettings { get; set; }

		public DxStoreKeyFactory DxStoreKeyFactoryInstance { get; set; }

		public DistributedStore.Context ConstructContext(string nodeName)
		{
			return new DistributedStore.Context
			{
				ChannelFactory = null,
				ClusterHandle = ClusapiMethods.OpenCluster(nodeName),
				NodeName = nodeName
			};
		}

		public void ExecuteRequest(DistributedStoreKey key, OperationCategory operationCategory, OperationType operationType, string dbgInfo, Action<IDistributedStoreKey, bool, StoreKind> action)
		{
			this.ExecuteRequest<int>(key, operationCategory, operationType, dbgInfo, delegate(IDistributedStoreKey storeKey, bool isPrimary, StoreKind storeKind)
			{
				action(storeKey, isPrimary, storeKind);
				return 0;
			});
		}

		public T ExecuteRequest<T>(DistributedStoreKey key, OperationCategory operationCategory, OperationType operationType, string dbgInfo, Func<IDistributedStoreKey, T> func)
		{
			return this.ExecuteRequest<T>(key, operationCategory, operationType, dbgInfo, (IDistributedStoreKey storeKey, bool isPrimary, StoreKind storeKind) => func(storeKey));
		}

		public T ExecuteRequest<T>(DistributedStoreKey key, OperationCategory operationCategory, OperationType operationType, string dbgInfo, Func<IDistributedStoreKey, bool, StoreKind, T> func)
		{
			RequestInfo req = new RequestInfo(operationCategory, operationType, dbgInfo);
			this.EnqueueShadowAction<T>(key, req, func);
			return this.PerformAction<T>(key, req, true, func);
		}

		public void TriggerDistributedStoreManagerRefreshBestEffort(string reason, bool isForceRefreshCache = true)
		{
			if (this.StoreSettings.Primary == StoreKind.DxStore || this.StoreSettings.Shadow == StoreKind.DxStore)
			{
				Exception ex = Utils.RunBestEffort(delegate
				{
					this.TriggerDistributedStoreManagerRefresh(reason, isForceRefreshCache);
				});
				if (ex != null)
				{
					DxStoreHACrimsonEvents.TriggerRefreshFailed.Log<string>(ex.Message);
				}
			}
		}

		private void TriggerDistributedStoreManagerRefresh(string reason, bool isForceRefreshCache)
		{
			if (this.DxStoreKeyFactoryInstance != null)
			{
				if (this.managerClientFactory == null)
				{
					this.managerClientFactory = new ManagerClientFactory(this.DxStoreKeyFactoryInstance.ConfigProvider.ManagerConfig, null);
				}
				if (this.managerClientFactory != null)
				{
					DxStoreManagerClient localClient = this.managerClientFactory.LocalClient;
					localClient.TriggerRefresh(reason, isForceRefreshCache, null);
				}
			}
		}

		internal bool IsKeyExist(string keyName, AmCluster cluster = null)
		{
			bool result = false;
			bool flag = false;
			if (cluster == null)
			{
				cluster = AmCluster.Open();
				flag = true;
			}
			try
			{
				using (IDistributedStoreKey clusterKey = this.GetClusterKey(cluster.Handle, null, null, DxStoreKeyAccessMode.Read, true))
				{
					if (clusterKey != null)
					{
						using (IDistributedStoreKey distributedStoreKey = clusterKey.OpenKey(keyName, DxStoreKeyAccessMode.Read, true, null))
						{
							result = (distributedStoreKey != null);
						}
					}
				}
			}
			finally
			{
				if (flag && cluster != null)
				{
					cluster.Dispose();
				}
			}
			return result;
		}

		public void StartProcessRestartTimer()
		{
			if (this.restartTimer == null)
			{
				lock (this.restartTimerLock)
				{
					if (this.restartTimer == null)
					{
						DistributedStore.ProcessRestartTimer processRestartTimer = new DistributedStore.ProcessRestartTimer();
						processRestartTimer.Start();
						this.restartTimer = processRestartTimer;
					}
				}
			}
		}

		public void StopProcessRestartTimer()
		{
			lock (this.restartTimerLock)
			{
				if (this.restartTimer != null)
				{
					this.restartTimer.Stop();
					this.restartTimer = null;
				}
			}
		}

		public IDistributedStoreKey GetBaseKey(DxStoreKeyAccessMode mode, DistributedStore.Context context)
		{
			if (this.IsRestartProcessOnDxStoreModeChange)
			{
				this.StartProcessRestartTimer();
			}
			DistributedStoreKey compositeKey = new DistributedStoreKey(string.Empty, string.Empty, mode, context);
			compositeKey.IsBaseKey = true;
			try
			{
				this.ExecuteRequest(compositeKey, OperationCategory.GetBaseKey, OperationType.Read, string.Empty, delegate(IDistributedStoreKey key, bool isPrimary, StoreKind storeKind)
				{
					this.SetKeyByRole(compositeKey, isPrimary, this.BaseKeyGenerator(mode, context, storeKind));
				});
			}
			finally
			{
				if (compositeKey.PrimaryStoreKey == null)
				{
					compositeKey.CloseKey();
					compositeKey = null;
				}
			}
			return compositeKey;
		}

		public void SetKeyByRole(DistributedStoreKey compositeKey, bool isPrimary, IDistributedStoreKey key)
		{
			if (isPrimary)
			{
				compositeKey.PrimaryStoreKey = key;
				return;
			}
			compositeKey.ShadowStoreKey = key;
		}

		public IDistributedStoreKey GetBaseKeyByStoreKind(DxStoreKeyAccessMode mode, DistributedStore.Context context, StoreKind storeKind)
		{
			IDistributedStoreKey result;
			switch (storeKind)
			{
			case StoreKind.Clusdb:
				result = ClusterDbKey.GetBaseKey(context.ClusterHandle, mode);
				break;
			case StoreKind.DxStore:
				result = this.DxStoreKeyFactoryInstance.GetBaseKey(mode, context.ChannelFactory, context.NodeName, false);
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		internal IDistributedStoreKey GetClusterKey(AmClusterHandle clusterHandle, CachedChannelFactory<IDxStoreAccess> channelFactory = null, string nodeName = null, DxStoreKeyAccessMode mode = DxStoreKeyAccessMode.Read, bool isBestEffort = false)
		{
			IDistributedStoreKey result = null;
			try
			{
				if (this.StoreSettings.IsCompositeModeEnabled || RegistryParameters.DistributedStoreIsLogPerformanceForSingleStore)
				{
					result = this.GetClusterCompositeKey(clusterHandle, channelFactory, nodeName, mode);
				}
				else
				{
					DistributedStore.Context context = new DistributedStore.Context
					{
						ClusterHandle = clusterHandle,
						ChannelFactory = channelFactory,
						NodeName = nodeName
					};
					result = this.GetBaseKeyByStoreKind(mode, context, this.StoreSettings.Primary);
				}
			}
			catch (ClusterException)
			{
				if (!isBestEffort)
				{
					throw;
				}
			}
			return result;
		}

		private IDistributedStoreKey GetClusterCompositeKey(AmClusterHandle clusterHandle, CachedChannelFactory<IDxStoreAccess> channelFactory = null, string nodeName = null, DxStoreKeyAccessMode mode = DxStoreKeyAccessMode.Read)
		{
			DistributedStore.Context context = new DistributedStore.Context
			{
				ClusterHandle = clusterHandle,
				ChannelFactory = channelFactory,
				NodeName = nodeName
			};
			return this.GetBaseKey(mode, context);
		}

		private T PerformAction<T>(DistributedStoreKey key, RequestInfo req, bool isPrimary, Func<IDistributedStoreKey, bool, StoreKind, T> func)
		{
			IDistributedStoreKey arg = isPrimary ? key.PrimaryStoreKey : key.ShadowStoreKey;
			StoreKind storeKind = isPrimary ? this.StoreSettings.Primary : this.StoreSettings.Shadow;
			this.PerfTracker.UpdateStart(key, storeKind, isPrimary);
			Exception exception = null;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			T result;
			try
			{
				result = func(arg, isPrimary, storeKind);
			}
			catch (Exception ex)
			{
				exception = ex;
				throw;
			}
			finally
			{
				this.PerfTracker.UpdateFinish(key, storeKind, isPrimary, req, stopwatch.ElapsedMilliseconds, exception, false);
			}
			return result;
		}

		private void EnqueueShadowAction<T>(DistributedStoreKey key, RequestInfo req, Func<IDistributedStoreKey, bool, StoreKind, T> func)
		{
			if (!this.StoreSettings.IsShadowConfigured)
			{
				return;
			}
			lock (this.shadowLock)
			{
				Queue<Action> queue;
				int maxAllowedLimit;
				if (req.OperationType == OperationType.Write)
				{
					queue = this.shadowWriteQueue;
					maxAllowedLimit = RegistryParameters.DistributedStoreShadowMaxAllowedWriteQueueLength;
				}
				else
				{
					maxAllowedLimit = RegistryParameters.DistributedStoreShadowMaxAllowedReadQueueLength;
					if (req.OperationCategory == OperationCategory.OpenKey)
					{
						queue = this.shadowOpenRequestsQueue;
					}
					else
					{
						queue = this.shadowReadQueue;
					}
				}
				if (this.EnsureQueueLengthInLimit(queue, maxAllowedLimit, key, req))
				{
					if (!req.IsGetBaseKeyRequest && !req.IsCloseKeyRequest && key.ShadowStoreKey == null)
					{
						queue.Enqueue(delegate
						{
							using (IDistributedStoreKey baseKeyByStoreKind = this.GetBaseKeyByStoreKind(DxStoreKeyAccessMode.Write, key.Context, this.StoreSettings.Shadow))
							{
								if (baseKeyByStoreKind != null)
								{
									key.ShadowStoreKey = baseKeyByStoreKind.OpenKey(key.FullKeyName, DxStoreKeyAccessMode.Write, false, null);
								}
							}
						});
					}
					queue.Enqueue(delegate
					{
						this.PerformAction<T>(key, req, false, func);
					});
					if (!this.isShadowActionExecuting)
					{
						this.isShadowActionExecuting = true;
						ThreadPool.QueueUserWorkItem(delegate(object o)
						{
							this.ExecuteShadowActions();
						});
					}
				}
			}
		}

		private bool EnsureQueueLengthInLimit(Queue<Action> queue, int maxAllowedLimit, DistributedStoreKey key, RequestInfo req)
		{
			if (queue.Count < maxAllowedLimit)
			{
				return true;
			}
			this.PerfTracker.UpdateStart(key, this.StoreSettings.Shadow, false);
			this.PerfTracker.UpdateFinish(key, this.StoreSettings.Shadow, false, req, 0L, null, true);
			return false;
		}

		private void ExecuteShadowActions()
		{
			int num = 0;
			while (!this.IsStopping)
			{
				Action action = null;
				lock (this.shadowLock)
				{
					if (this.shadowWriteQueue.Count + this.shadowOpenRequestsQueue.Count + this.shadowReadQueue.Count == 0)
					{
						this.isShadowActionExecuting = false;
						break;
					}
					if (this.shadowOpenRequestsQueue.Count > 0)
					{
						action = this.shadowOpenRequestsQueue.Dequeue();
					}
					else if (this.shadowWriteQueue.Count > 0)
					{
						action = this.shadowWriteQueue.Dequeue();
					}
					else if (this.shadowReadQueue.Count > 0)
					{
						action = this.shadowReadQueue.Dequeue();
					}
				}
				this.AttemptBestEffort(action);
				if (++num % 5 == 0)
				{
					num = 0;
					Thread.Yield();
					Thread.Sleep(1);
				}
			}
		}

		private bool AttemptBestEffort(Action action)
		{
			bool result = false;
			try
			{
				if (action != null)
				{
					action();
				}
				result = true;
			}
			catch (Exception)
			{
			}
			return result;
		}

		public const string ExchangeHighAvailabilityComponentName = "ExchangeHA";

		private static readonly object instanceLock = new object();

		private readonly object shadowLock = new object();

		private readonly Queue<Action> shadowReadQueue = new Queue<Action>();

		private readonly Queue<Action> shadowOpenRequestsQueue = new Queue<Action>();

		private readonly Queue<Action> shadowWriteQueue = new Queue<Action>();

		private static DistributedStore instance;

		private bool isShadowActionExecuting;

		private ManagerClientFactory managerClientFactory;

		private object restartTimerLock = new object();

		private volatile DistributedStore.ProcessRestartTimer restartTimer;

		public class DxStoreRegistryProviderWithVariantConfig : DxStoreRegistryConfigProvider
		{
			public override CommonSettings UpdateDefaultCommonSettings(CommonSettings input)
			{
				IActiveManagerSettings settings = DxStoreSetting.Instance.GetSettings();
				input.IsUseHttpTransportForInstanceCommunication = settings.DxStoreIsUseHttpForInstanceCommunication;
				input.IsUseHttpTransportForClientCommunication = settings.DxStoreIsUseHttpForClientCommunication;
				input.IsUseBinarySerializerForClientCommunication = settings.DxStoreIsUseBinarySerializerForClientCommunication;
				input.IsUseEncryption = settings.DxStoreIsEncryptionEnabled;
				return input;
			}
		}

		internal class ProcessRestartTimer : TimerComponent
		{
			[DllImport("kernel32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

			public ProcessRestartTimer() : base(TimeSpan.Zero, TimeSpan.FromSeconds(10.0), "Process restart timer")
			{
				this.initialMode = DxStoreSetting.Instance.GetSettings().DxStoreRunMode;
			}

			protected override void TimerCallbackInternal()
			{
				DxStoreMode dxStoreRunMode = DxStoreSetting.Instance.GetSettings().DxStoreRunMode;
				if (dxStoreRunMode != this.initialMode)
				{
					Process.GetCurrentProcess();
					DistributedStore.ProcessRestartTimer.TerminateProcess(Process.GetCurrentProcess().Handle, 0U);
				}
			}

			private DxStoreMode initialMode;
		}

		public class Context
		{
			public CachedChannelFactory<IDxStoreAccess> ChannelFactory { get; set; }

			public string NodeName { get; set; }

			internal AmClusterHandle ClusterHandle { get; set; }
		}
	}
}
