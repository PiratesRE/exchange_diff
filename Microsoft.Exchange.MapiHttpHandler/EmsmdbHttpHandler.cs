using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.MapiHttp;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Handler;
using Microsoft.Exchange.RpcClientAccess.Messages;
using Microsoft.Exchange.RpcClientAccess.Monitoring;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.RpcClientAccess.Server;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EmsmdbHttpHandler : MapiHttpHandler
	{
		internal static IExchangeAsyncDispatch ExchangeAsyncDispatch
		{
			get
			{
				return EmsmdbHttpHandler.exchangeAsyncDispatch;
			}
		}

		internal override string EndpointVdirPath
		{
			get
			{
				return MapiHttpEndpoints.VdirPathEmsmdb;
			}
		}

		internal override IAsyncOperationFactory OperationFactory
		{
			get
			{
				return EmsmdbHttpHandler.operationFactory;
			}
		}

		internal override bool TryEnsureHandlerIsInitialized()
		{
			if (EmsmdbHttpHandler.shutdownTime != null)
			{
				return false;
			}
			if (!EmsmdbHttpHandler.initialized)
			{
				lock (EmsmdbHttpHandler.initializeLock)
				{
					if (EmsmdbHttpHandler.shutdownTime != null)
					{
						return false;
					}
					if (!EmsmdbHttpHandler.initialized)
					{
						MapiHttpHandler.IsValidContextHandleDelegate = new Func<object, bool>(EmsmdbHttpHandler.InternalIsValidContextHandle);
						MapiHttpHandler.TryContextHandleRundownDelegate = new Func<object, bool>(EmsmdbHttpHandler.InternalTryContextHandleRundown);
						MapiHttpHandler.QueueDroppedConnectionDelegate = new Action<object>(EmsmdbHttpHandler.InternalQueueDroppedConnection);
						MapiHttpHandler.ShutdownHandlerDelegate = new Action(EmsmdbHttpHandler.InternalShutdownHandler);
						MapiHttpHandler.NeedTokenRehydrationDelegate = new Func<string, bool>(EmsmdbHttpHandler.InternalNeedTokenRehydration);
						EmsmdbHttpHandler.InitializeRpcClientAccess();
						EmsmdbHttpHandler.initialized = true;
					}
				}
				return true;
			}
			return true;
		}

		internal override void LogFailure(IList<string> requestIds, IList<string> cookies, string message, string userName, string protocolSequence, string clientAddress, string organization, Exception exception, Microsoft.Exchange.Diagnostics.Trace trace)
		{
			ProtocolLog.LogMapiHttpProtocolFailure(requestIds, cookies, "mapihttp: failure", message ?? string.Empty, exception, userName ?? string.Empty, organization ?? string.Empty, protocolSequence, clientAddress ?? string.Empty, trace);
		}

		private static void InternalShutdownHandler()
		{
			try
			{
				if (EmsmdbHttpHandler.shutdownTime == null)
				{
					EmsmdbHttpHandler.shutdownTime = new ExDateTime?(ExDateTime.Now);
					lock (EmsmdbHttpHandler.initializeLock)
					{
						if (EmsmdbHttpHandler.initialized)
						{
							EmsmdbHttpHandler.ShutdownRpcClientAccess();
							EmsmdbHttpHandler.initialized = false;
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private static bool InternalIsValidContextHandle(object contextHandle)
		{
			if (contextHandle == null)
			{
				return false;
			}
			IntPtr? intPtr = contextHandle as IntPtr?;
			return intPtr != null && !(intPtr.Value == IntPtr.Zero);
		}

		private static bool InternalTryContextHandleRundown(object contextHandle)
		{
			IntPtr? localContextHandle = contextHandle as IntPtr?;
			if (localContextHandle == null)
			{
				return true;
			}
			if (localContextHandle.Value == IntPtr.Zero)
			{
				return true;
			}
			MapiHttpHandler.DispatchCallSync(delegate
			{
				EmsmdbHttpHandler.ExchangeAsyncDispatch.ContextHandleRundown(localContextHandle.Value);
			});
			return true;
		}

		private static void InternalQueueDroppedConnection(object contextHandle)
		{
			IntPtr? intPtr = contextHandle as IntPtr?;
			if (intPtr == null)
			{
				return;
			}
			if (intPtr.Value == IntPtr.Zero)
			{
				return;
			}
			bool flag = false;
			lock (EmsmdbHttpHandler.connectionDroppedQueueLock)
			{
				EmsmdbHttpHandler.connectionDroppedQueue.Enqueue(intPtr.Value);
				if (!EmsmdbHttpHandler.isConnectionDroppedThreadRunning)
				{
					EmsmdbHttpHandler.isConnectionDroppedThreadRunning = true;
					if (!ThreadPool.QueueUserWorkItem(new WaitCallback(EmsmdbHttpHandler.ProcessConnectionDroppedQueue)))
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				EmsmdbHttpHandler.ProcessConnectionDroppedQueue(null);
			}
		}

		private static void ProcessConnectionDroppedQueue(object state)
		{
			bool flag = false;
			try
			{
				for (;;)
				{
					IntPtr contextHandle;
					lock (EmsmdbHttpHandler.connectionDroppedQueueLock)
					{
						if (EmsmdbHttpHandler.connectionDroppedQueue.Count <= 0)
						{
							EmsmdbHttpHandler.isConnectionDroppedThreadRunning = false;
							flag = true;
							break;
						}
						contextHandle = EmsmdbHttpHandler.connectionDroppedQueue.Dequeue();
					}
					MapiHttpHandler.DispatchCallSync(delegate
					{
						EmsmdbHttpHandler.ExchangeAsyncDispatch.DroppedConnection(contextHandle);
					});
				}
			}
			finally
			{
				if (!flag)
				{
					EmsmdbHttpHandler.isConnectionDroppedThreadRunning = false;
				}
			}
		}

		private static bool InternalNeedTokenRehydration(string requestType)
		{
			return !string.IsNullOrWhiteSpace(requestType) && (string.Compare(requestType, "Connect", true) == 0 || string.Compare(requestType, "EcDoConnectEx", true) == 0 || string.Compare(requestType, "Dummy", true) == 0 || string.Compare(requestType, "EcDoDummy", true) == 0);
		}

		private static IRpcDispatch CreateRpcDispatch()
		{
			IRpcDispatch result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				RpcDispatch rpcDispatch = new RpcDispatch(ConnectionHandler.Factory, new DriverFactory());
				disposeGuard.Add<RpcDispatch>(rpcDispatch);
				IRpcDispatch rpcDispatch2 = new WatsonOnUnhandledExceptionDispatch(rpcDispatch);
				disposeGuard.Success();
				result = rpcDispatch2;
			}
			return result;
		}

		private static IExchangeDispatch CreateExchangeDispatch(IRpcDispatch rpcDispatch)
		{
			return new ExchangeDispatch(rpcDispatch);
		}

		private static IExchangeAsyncDispatch CreateExchangeAsyncDispatch(IExchangeDispatch exchangeDispatch, DispatchPool dispatchPool)
		{
			return new ExchangeAsyncDispatch(exchangeDispatch, dispatchPool);
		}

		private static DispatchPool CreateDispatchPool()
		{
			return new DispatchPool("RpcClientAccess-MapiHttp", Configuration.ServiceConfiguration.MaximumRpcTasks, Configuration.ServiceConfiguration.MaximumRpcThreads, Configuration.ServiceConfiguration.MinimumRpcThreads, RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.DispatchTaskQueueLength, RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.DispatchTaskThreads, RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.DispatchTaskActiveThreads, RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.DispatchTaskOperationsRate);
		}

		private static void LogConfigurationEventConfig(ExEventLog.EventTuple tuple, params object[] args)
		{
			string periodicKey = null;
			if (tuple.Period == ExEventLog.EventPeriod.LogPeriodic)
			{
				periodicKey = args.Aggregate(tuple.EventId.GetHashCode(), (int hashCode, object arg) => hashCode ^= ((arg != null) ? arg.GetHashCode() : 0)).ToString();
			}
			EmsmdbHttpHandler.eventLogger.LogEvent(tuple, periodicKey, args);
		}

		private static void InitializeRpcClientAccess()
		{
			ExMonHandler.IsEnabled = false;
			Configuration.AppConfigFileName = string.Format("{0}ClientAccess\\mapi\\emsmdb\\web.config", ExchangeSetupContext.InstallPath);
			ProtocolLogConfiguration.SetDefaults(string.Format("{0}Logging\\MAPI Client Access\\", ExchangeSetupContext.InstallPath), "MAPI Mailbox Protocol Logs", "MAPIMB_", "MAPIMailboxProtocolLogs");
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				EmsmdbHttpHandler.processId = currentProcess.Id;
			}
			EmsmdbHttpHandler.eventLogger = new ExEventLog(EmsmdbHttpHandler.ComponentGuid, "MSExchangeMapiMailboxAppPool");
			EmsmdbHttpHandler.eventLogger.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_StartingMSExchangeMapiMailboxAppPool, string.Empty, new object[]
			{
				EmsmdbHttpHandler.processId,
				"Microsoft Exchange",
				"15.00.1497.015"
			});
			Configuration.EventLogger = new ConfigurationSchema.EventLogger(EmsmdbHttpHandler.LogConfigurationEventConfig);
			int num = Privileges.RemoveAllExcept(new string[]
			{
				"SeAuditPrivilege",
				"SeChangeNotifyPrivilege",
				"SeCreateGlobalPrivilege",
				"SeTcbPrivilege"
			}, "MSExchangeMapiMailboxAppPool");
			if (num != 0)
			{
				EmsmdbHttpHandler.eventLogger.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_MapiMailboxRemovingPrivilegeErrorOnStart, string.Empty, new object[0]);
				string failureDescription = string.Format("Failed to remove privileges from {0}, error code = {1}", "MSExchangeMapiMailboxAppPool", num);
				throw ProtocolException.FromResponseCode((LID)48028, failureDescription, ResponseCode.EndpointDisabled, null);
			}
			EmsmdbHttpHandler.configurationManager = new ConfigurationManager();
			EmsmdbHttpHandler.configurationManager.LoadAndRegisterForNotifications();
			ProtocolLog.Initialize();
			RpcClientAccessPerformanceCountersWrapper.Initialize(new EmsmdbPerformanceCountersWrapper(), new NullRpcHttpConnectionRegistrationPerformanceCounters(), new NullXtcPerformanceCounters());
			EmsmdbHttpHandler.dispatchPool = EmsmdbHttpHandler.CreateDispatchPool();
			EmsmdbHttpHandler.rpcDispatch = EmsmdbHttpHandler.CreateRpcDispatch();
			EmsmdbHttpHandler.exchangeDispatch = EmsmdbHttpHandler.CreateExchangeDispatch(EmsmdbHttpHandler.rpcDispatch);
			EmsmdbHttpHandler.exchangeAsyncDispatch = EmsmdbHttpHandler.CreateExchangeAsyncDispatch(EmsmdbHttpHandler.exchangeDispatch, EmsmdbHttpHandler.dispatchPool);
			EmsmdbHttpHandler.eventLogger.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_MSExchangeMapiMailboxAppPoolStartSuccess, string.Empty, new object[]
			{
				EmsmdbHttpHandler.processId,
				"Microsoft Exchange",
				"15.00.1497.015"
			});
		}

		private static void ShutdownRpcClientAccess()
		{
			EmsmdbHttpHandler.eventLogger.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_StoppingMSExchangeMapiMailboxAppPool, string.Empty, new object[]
			{
				EmsmdbHttpHandler.processId,
				"Microsoft Exchange",
				"15.00.1497.015"
			});
			if (EmsmdbHttpHandler.dispatchPool != null)
			{
				while (ExDateTime.Now - EmsmdbHttpHandler.shutdownTime.Value < EmsmdbHttpHandler.waitDrainOnShutdown)
				{
					Thread.Sleep(500);
					if (EmsmdbHttpHandler.dispatchPool.ActiveThreads == 0)
					{
						break;
					}
				}
			}
			Util.DisposeIfPresent(EmsmdbHttpHandler.dispatchPool);
			Util.DisposeIfPresent(EmsmdbHttpHandler.rpcDispatch);
			Util.DisposeIfPresent(EmsmdbHttpHandler.configurationManager);
			ProtocolLog.Shutdown();
			EmsmdbHttpHandler.eventLogger.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_MSExchangeMapiMailboxAppPoolStopSuccess, string.Empty, new object[]
			{
				EmsmdbHttpHandler.processId,
				"Microsoft Exchange",
				"15.00.1497.015"
			});
		}

		// Note: this type is marked as 'beforefieldinit'.
		static EmsmdbHttpHandler()
		{
			Dictionary<string, Func<HttpContextBase, AsyncOperation>> dictionary = new Dictionary<string, Func<HttpContextBase, AsyncOperation>>();
			dictionary.Add("Connect", (HttpContextBase context) => new EmsmdbConnectAsyncOperation(context));
			dictionary.Add("Disconnect", (HttpContextBase context) => new EmsmdbDisconnectAsyncOperation(context));
			dictionary.Add("Execute", (HttpContextBase context) => new EmsmdbExecuteAsyncOperation(context));
			dictionary.Add("NotificationWait", (HttpContextBase context) => new EmsmdbNotificationWaitAsyncOperation(context));
			dictionary.Add("Dummy", (HttpContextBase context) => new EmsmdbDummyAsyncOperation(context));
			dictionary.Add("EcDoConnectEx", (HttpContextBase context) => new EmsmdbLegacyConnectAsyncOperation(context));
			dictionary.Add("EcDoDisconnect", (HttpContextBase context) => new EmsmdbLegacyDisconnectAsyncOperation(context));
			dictionary.Add("EcDoRpcExt2", (HttpContextBase context) => new EmsmdbLegacyExecuteAsyncOperation(context));
			dictionary.Add("EcDoAsyncWaitEx", (HttpContextBase context) => new EmsmdbLegacyNotificationWaitAsyncOperation(context));
			dictionary.Add("EcDoDummy", (HttpContextBase context) => new EmsmdbDummyAsyncOperation(context));
			EmsmdbHttpHandler.operationFactory = new DictionaryBasedOperationFactory(dictionary);
			EmsmdbHttpHandler.waitDrainOnShutdown = TimeSpan.FromSeconds(30.0);
			EmsmdbHttpHandler.initializeLock = new object();
			EmsmdbHttpHandler.connectionDroppedQueueLock = new object();
			EmsmdbHttpHandler.connectionDroppedQueue = new Queue<IntPtr>();
			EmsmdbHttpHandler.initialized = false;
			EmsmdbHttpHandler.isConnectionDroppedThreadRunning = false;
			EmsmdbHttpHandler.shutdownTime = null;
			EmsmdbHttpHandler.configurationManager = null;
		}

		public const string ApplicationPoolName = "MSExchangeMapiMailboxAppPool";

		private const string LogTypeName = "MAPI Mailbox Protocol Logs";

		private const string LogFilePrefix = "MAPIMB_";

		private const string LogComponent = "MAPIMailboxProtocolLogs";

		private static readonly Guid ComponentGuid = new Guid("84036911-D9A7-4CD5-8162-861C43E08CA5");

		private static readonly IAsyncOperationFactory operationFactory;

		private static readonly TimeSpan waitDrainOnShutdown;

		private static readonly object initializeLock;

		private static readonly object connectionDroppedQueueLock;

		private static readonly Queue<IntPtr> connectionDroppedQueue;

		private static bool initialized;

		private static bool isConnectionDroppedThreadRunning;

		private static ExDateTime? shutdownTime;

		private static ConfigurationManager configurationManager;

		private static ExEventLog eventLogger;

		private static IRpcDispatch rpcDispatch;

		private static IExchangeDispatch exchangeDispatch;

		private static IExchangeAsyncDispatch exchangeAsyncDispatch;

		private static DispatchPool dispatchPool;

		private static int processId;
	}
}
