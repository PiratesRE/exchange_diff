using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Handler;
using Microsoft.Exchange.RpcClientAccess.Messages;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class RpcClientAccessService : BaseObject, IRpcService, IDisposable
	{
		public RpcClientAccessService(IRpcServiceManager serviceManager)
		{
			this.eventLog = RpcClientAccessService.CreateEventLog();
			this.serviceManager = serviceManager;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				this.eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_StartingRpcClientService, string.Empty, new object[]
				{
					currentProcess.Id,
					"Microsoft Exchange",
					"15.00.1497.012"
				});
			}
		}

		public string Name
		{
			get
			{
				base.CheckDisposed();
				return "MSExchangeRPC";
			}
		}

		internal static bool IsShuttingDown
		{
			get
			{
				return RpcClientAccessService.isShuttingDown;
			}
		}

		public bool IsEnabled()
		{
			base.CheckDisposed();
			return Configuration.ServiceConfiguration.IsServiceEnabled && !Configuration.ServiceConfiguration.IsDisabledOnMailboxRole;
		}

		public void OnStartBegin()
		{
			base.CheckDisposed();
			if (this.isRpcServiceEndpointStarted)
			{
				throw new InvalidOperationException("RpcClientAccessService.OnStartBegin(): This RPC service endpoint is already active! Please check if the RPC Client Access Service is being initialized twice.");
			}
			try
			{
				this.LogInitializationCheckPoint("RegisterServiceClass");
				ServiceHelper.RegisterSPN("exchangeMDB", this.eventLog, RpcClientAccessServiceEventLogConstants.Tuple_SpnRegisterFailure);
				RpcClientAccessPerformanceCountersWrapper.Initialize(new RcaPerformanceCounters(), new RpcHttpConnectionRegistrationPerformanceCounters(), new XtcPerformanceCounters());
				this.LogInitializationCheckPoint("ConfigurationManager");
				Configuration.EventLogger = new ConfigurationSchema.EventLogger(this.LogConfigurationEvent);
				this.configurationManager = new ConfigurationManager();
				this.configurationManager.AsyncUnhandledException += this.OnAsyncUnhandledException;
				this.configurationManager.ConfigurationLoadFailed += this.OnConfigurationLoadFailed;
				this.configurationManager.LoadAndRegisterForNotifications();
				ProtocolLog.Initialize();
				this.configurationManager.ConfigurationLoadFailed += this.OnConfigurationUpdateFailed;
				this.configurationManager.ConfigurationLoadFailed -= this.OnConfigurationLoadFailed;
				Configuration.ConfigurationChanged += this.OnConfigurationChanged;
				this.LogInitializationCheckPoint("CanStartServiceOnLocalServer");
				if (this.CanStartServiceOnLocalServer())
				{
					this.LogInitializationCheckPoint("RpcDispatch");
					RopLogon.AuthenticationContextCompression = new AuthContextDecompressor();
					this.rpcInterfaces = new Dictionary<string, RpcClientAccessService.RPCInterfaceContainer>(4);
					this.rpcDispatch = RpcClientAccessService.CreateRpcDispatch();
					this.rpcDispatchPool = RpcClientAccessService.CreateRpcDispatchPool();
					IExchangeDispatch exchangeDispatch = RpcClientAccessService.CreateExchangeDispatch(this.rpcDispatch);
					RpcClientAccessService.RPCInterfaceContainer value = new RpcClientAccessService.RPCInterfaceContainer(exchangeDispatch, RpcClientAccessService.CreateExchangeAsyncDispatch(exchangeDispatch, this.rpcDispatchPool), null);
					this.rpcInterfaces.Add("exchangeDispatch", value);
					RpcHttpConnectionRegistrationDispatch rpcHttpConnectionRegistrationDispatch = RpcClientAccessService.CreateRpcHttpConnectionRegistrationDispatch();
					DispatchPool dispatchPool = RpcClientAccessService.CreateRpcHttpConnectionRegistrationRpcDispatchPool();
					RpcClientAccessService.RPCInterfaceContainer value2 = new RpcClientAccessService.RPCInterfaceContainer(rpcHttpConnectionRegistrationDispatch, RpcClientAccessService.CreateRpcHttpConnectionRegistrationAsyncDispatch(rpcHttpConnectionRegistrationDispatch, dispatchPool), dispatchPool);
					this.rpcInterfaces.Add("rpcHttpConnectionRegistrationDispatch", value2);
					this.serviceManager.AddHttpPort(6001.ToString());
					this.serviceManager.EnableLrpc();
					RpcServer.Initialize((IExchangeAsyncDispatch)this.rpcInterfaces["exchangeDispatch"].AsyncDispatchInterface, this.rpcDispatch.MaximumConnections, this.eventLog);
					RpcAsynchronousServer.Initialize((IExchangeAsyncDispatch)this.rpcInterfaces["exchangeDispatch"].AsyncDispatchInterface, this.rpcDispatch.MaximumConnections, this.eventLog);
					RpcHttpConnectionRegistrationAsyncServer.Initialize((IRpcHttpConnectionRegistrationAsyncDispatch)this.rpcInterfaces["rpcHttpConnectionRegistrationDispatch"].AsyncDispatchInterface, this.rpcDispatch.MaximumConnections, this.eventLog);
					this.serviceManager.AddServer(new Action(RpcAsynchronousServer.Start), new Action(RpcAsynchronousServer.Stop));
					this.serviceManager.AddServer(new Action(RpcServer.Start), new Action(RpcServer.Stop));
					this.serviceManager.AddServer(new Action(RpcHttpConnectionRegistrationAsyncServer.Start), new Action(RpcHttpConnectionRegistrationAsyncServer.Stop));
				}
				this.isRpcServiceEndpointStarted = true;
			}
			finally
			{
				if (!this.isRpcServiceEndpointStarted)
				{
					this.CleanUpInternalRpcEndpointState();
				}
			}
		}

		public void OnStartEnd()
		{
			base.CheckDisposed();
			if (this.isWebServiceEndpointStarted)
			{
				throw new InvalidOperationException("RpcClientAccessService.OnStartEnd(): This web service endpoint is already active! Please check if the RPC Client Access Service is being initialized twice.");
			}
			if (Configuration.ServiceConfiguration.EnableWebServicesEndpoint)
			{
				try
				{
					this.LogInitializationCheckPoint("WebServiceEndPoint.Start");
					DispatchPool dispatchPool = RpcClientAccessService.CreateWebServiceDispatchPool();
					IExchangeAsyncDispatch exchangeAsyncDispatch = RpcClientAccessService.CreateExchangeAsyncDispatch((IExchangeDispatch)this.rpcInterfaces["exchangeDispatch"].DispatchInterface, dispatchPool);
					RpcClientAccessService.RPCInterfaceContainer value = new RpcClientAccessService.RPCInterfaceContainer(null, exchangeAsyncDispatch, dispatchPool);
					this.rpcInterfaces.Add("webserviceendpoint", value);
					VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
					string arg = snapshot.RpcClientAccess.XtcEndpoint.Enabled ? "444" : "443";
					string endpoint = string.Format("https://localhost:{0}/xrop", arg);
					WebServiceEndPoint.Start(exchangeAsyncDispatch, endpoint, this.eventLog);
					this.isWebServiceEndpointStarted = true;
				}
				finally
				{
					if (!this.isWebServiceEndpointStarted)
					{
						this.CleanUpInternalWebEndpointState();
					}
				}
			}
			if (Configuration.ServiceConfiguration.CanServicePrivateLogons)
			{
				this.eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_RpcClientAccessServiceStartPrivateSuccess, string.Empty, new object[0]);
			}
			if (Configuration.ServiceConfiguration.CanServicePublicLogons)
			{
				this.eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_RpcClientAccessServiceStartPublicSuccess, string.Empty, new object[0]);
			}
			this.checkExchangeRpcServiceResponsive = new CheckExchangeRpcServiceResponsive(this.eventLog);
		}

		public void OnStopBegin()
		{
			base.CheckDisposed();
			this.CleanUpInternalRpcEndpointState();
		}

		public void OnStopEnd()
		{
			base.CheckDisposed();
			this.CleanUpInternalWebEndpointState();
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				this.eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_RpcClientAccessServiceStopSuccess, string.Empty, new object[]
				{
					currentProcess.Id,
					"Microsoft Exchange",
					"15.00.1497.012"
				});
			}
		}

		public void HandleUnexpectedExceptionOnStart(Exception ex)
		{
			base.CheckDisposed();
			if (ex is DuplicateRpcEndpointException)
			{
				this.LogExceptionEvent(RpcClientAccessServiceEventLogConstants.Tuple_DuplicateRpcEndpoint, ex);
				return;
			}
			this.LogExceptionEvent(RpcClientAccessServiceEventLogConstants.Tuple_RpcClientServiceUnexpectedExceptionOnStart, ex);
		}

		public void HandleUnexpectedExceptionOnStop(Exception ex)
		{
			base.CheckDisposed();
			this.LogExceptionEvent(RpcClientAccessServiceEventLogConstants.Tuple_RpcClientServiceUnexpectedExceptionOnStop, ex);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RpcClientAccessService>(this);
		}

		protected override void InternalDispose()
		{
			if (this.rpcInterfaces != null)
			{
				foreach (RpcClientAccessService.RPCInterfaceContainer rpcinterfaceContainer in this.rpcInterfaces.Values)
				{
					rpcinterfaceContainer.Dispose();
				}
				this.rpcInterfaces = null;
			}
			Util.DisposeIfPresent(this.rpcDispatch);
			Util.DisposeIfPresent(this.configurationManager);
			Util.DisposeIfPresent(this.checkExchangeRpcServiceResponsive);
			ProtocolLog.Shutdown();
			base.InternalDispose();
		}

		private static ExEventLog CreateEventLog()
		{
			return new ExEventLog(RpcClientAccessService.ComponentGuid, "MSExchangeRPC");
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

		private static RpcHttpConnectionRegistrationDispatch CreateRpcHttpConnectionRegistrationDispatch()
		{
			return new RpcHttpConnectionRegistrationDispatch(RpcHttpConnectionRegistration.Instance);
		}

		private static IRpcHttpConnectionRegistrationAsyncDispatch CreateRpcHttpConnectionRegistrationAsyncDispatch(RpcHttpConnectionRegistrationDispatch rpcHttpConnectionRegistrationDispatch, DispatchPool dispatchPool)
		{
			return new RpcHttpConnectionRegistrationAsyncDispatch(rpcHttpConnectionRegistrationDispatch, dispatchPool);
		}

		private static DispatchPool CreateRpcDispatchPool()
		{
			return new DispatchPool("RpcDispatchPoolThread", Configuration.ServiceConfiguration.MaximumRpcTasks, Configuration.ServiceConfiguration.MaximumRpcThreads, Configuration.ServiceConfiguration.MinimumRpcThreads, RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.DispatchTaskQueueLength, RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.DispatchTaskThreads, RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.DispatchTaskActiveThreads, RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.DispatchTaskOperationsRate);
		}

		private static DispatchPool CreateRpcHttpConnectionRegistrationRpcDispatchPool()
		{
			return new DispatchPool("RpcHttpConnectionRegistrationDispatchPoolThread", Configuration.ServiceConfiguration.MaximumRpcHttpConnectionRegistrationTasks, Configuration.ServiceConfiguration.MaximumRpcHttpConnectionRegistrationThreads, Configuration.ServiceConfiguration.MinimumRpcHttpConnectionRegistrationThreads, RpcClientAccessPerformanceCountersWrapper.RpcHttpConnectionRegistrationPerformanceCounters.RpcHttpConnectionRegistrationDispatchTaskQueueLength, RpcClientAccessPerformanceCountersWrapper.RpcHttpConnectionRegistrationPerformanceCounters.RpcHttpConnectionRegistrationDispatchTaskThreads, RpcClientAccessPerformanceCountersWrapper.RpcHttpConnectionRegistrationPerformanceCounters.RpcHttpConnectionRegistrationDispatchTaskActiveThreads, RpcClientAccessPerformanceCountersWrapper.RpcHttpConnectionRegistrationPerformanceCounters.RpcHttpConnectionRegistrationDispatchTaskOperationsRate);
		}

		private static DispatchPool CreateWebServiceDispatchPool()
		{
			return new DispatchPool("XtcDispatchPoolThread", Configuration.ServiceConfiguration.MaximumWebServiceTasks, Configuration.ServiceConfiguration.MaximumWebServiceThreads, Configuration.ServiceConfiguration.MinimumWebServiceThreads, RpcClientAccessPerformanceCountersWrapper.XtcPerformanceCounters.XTCDispatchTaskQueueLength, RpcClientAccessPerformanceCountersWrapper.XtcPerformanceCounters.XTCDispatchTaskThreads, RpcClientAccessPerformanceCountersWrapper.XtcPerformanceCounters.XTCDispatchTaskActiveThreads, RpcClientAccessPerformanceCountersWrapper.XtcPerformanceCounters.XTCDispatchTaskOperationsRate);
		}

		private void CleanUpInternalRpcEndpointState()
		{
			RpcClientAccessService.isShuttingDown = true;
			WebServiceEndPoint.IsShuttingDown = true;
			Util.DisposeIfPresent(this.checkExchangeRpcServiceResponsive);
			if (this.rpcInterfaces != null)
			{
				foreach (RpcClientAccessService.RPCInterfaceContainer rpcinterfaceContainer in this.rpcInterfaces.Values)
				{
					if (rpcinterfaceContainer.DispatchPool != null)
					{
						ExDateTime now = ExDateTime.Now;
						while (ExDateTime.Now - now < RpcClientAccessService.waitDrainOnShutdown)
						{
							Thread.Sleep(500);
							if (rpcinterfaceContainer.DispatchPool.ActiveThreads == 0)
							{
								break;
							}
						}
					}
				}
			}
			Util.DisposeIfPresent(this.rpcDispatchPool);
			this.rpcDispatchPool = null;
			if (this.rpcInterfaces != null)
			{
				foreach (RpcClientAccessService.RPCInterfaceContainer rpcinterfaceContainer2 in this.rpcInterfaces.Values)
				{
					rpcinterfaceContainer2.Dispose();
				}
			}
			Util.DisposeIfPresent(this.rpcDispatch);
			this.rpcDispatch = null;
			this.rpcInterfaces = null;
			this.isRpcServiceEndpointStarted = false;
		}

		private void CleanUpInternalWebEndpointState()
		{
			if (this.rpcInterfaces != null)
			{
				this.rpcInterfaces.Remove("webserviceendpoint");
			}
			WebServiceEndPoint.Stop();
			this.isWebServiceEndpointStarted = false;
		}

		private void LogInitializationCheckPoint(string phase)
		{
		}

		private void LogConfigurationEvent(ExEventLog.EventTuple tuple, params object[] args)
		{
			string periodicKey = null;
			if (tuple.Period == ExEventLog.EventPeriod.LogPeriodic)
			{
				periodicKey = args.Aggregate(tuple.EventId.GetHashCode(), (int hashCode, object arg) => hashCode ^= ((arg != null) ? arg.GetHashCode() : 0)).ToString();
			}
			this.eventLog.LogEvent(tuple, periodicKey, args);
		}

		private bool CanStartServiceOnLocalServer()
		{
			ServiceConfiguration serviceConfiguration = Configuration.ServiceConfiguration;
			if (!serviceConfiguration.IsServiceEnabled)
			{
				this.eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_ServiceProtocolNotEnabled, string.Empty, new object[0]);
				return false;
			}
			if (serviceConfiguration.IsDisabledOnMailboxRole)
			{
				this.eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_CannotStartServiceOnMailboxRole, string.Empty, new object[0]);
				return false;
			}
			return true;
		}

		private void OnAsyncUnhandledException(Exception ex)
		{
			this.LogExceptionEvent(RpcClientAccessServiceEventLogConstants.Tuple_UnexpectedExceptionOnConfigurationUpdate, ex);
		}

		private void OnConfigurationChanged(object newConfiguration)
		{
			ProtocolLog.Referesh();
			if (this.configurationUpdateError && !this.configurationManager.HasConfigurationsThatFailToUpdate)
			{
				this.eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_ConfigurationUpdateAfterError, string.Empty, new object[0]);
			}
			if (Configuration.ServiceConfiguration.LogEveryConfigurationUpdate)
			{
				this.eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_ConfigurationUpdate, string.Empty, new object[0]);
			}
			this.configurationUpdateError = false;
			if (!this.CanStartServiceOnLocalServer())
			{
				this.serviceManager.StopService();
			}
		}

		private void OnConfigurationLoadFailed(Exception ex)
		{
			this.LogExceptionEvent(RpcClientAccessServiceEventLogConstants.Tuple_ConfigurationLoadFailed, ex);
			throw new RpcServiceAbortException(string.Format("RpcClientAccessService is being aborted due to ConfigurationLoadFailed {0}", ex.Message), ex);
		}

		private void OnConfigurationUpdateFailed(Exception ex)
		{
			this.LogExceptionEvent(RpcClientAccessServiceEventLogConstants.Tuple_ConfigurationUpdateFailed, ex);
			this.configurationUpdateError = true;
		}

		private void LogExceptionEvent(ExEventLog.EventTuple tuple, Exception ex)
		{
			this.eventLog.LogEvent(tuple, string.Empty, new object[]
			{
				ex
			});
		}

		internal const string RpcClientAccessServiceName = "MSExchangeRPC";

		private const string RpcClientAccessServicePrincipalClass = "exchangeMDB";

		private static readonly Guid ComponentGuid = ServiceConfiguration.ComponentGuid;

		private static readonly TimeSpan waitDrainOnShutdown = TimeSpan.FromSeconds(15.0);

		private static bool isShuttingDown = false;

		private readonly ExEventLog eventLog;

		private readonly IRpcServiceManager serviceManager;

		private ConfigurationManager configurationManager;

		private bool isRpcServiceEndpointStarted;

		private bool isWebServiceEndpointStarted;

		private IRpcDispatch rpcDispatch;

		private DispatchPool rpcDispatchPool;

		private Dictionary<string, RpcClientAccessService.RPCInterfaceContainer> rpcInterfaces;

		private bool configurationUpdateError;

		private CheckExchangeRpcServiceResponsive checkExchangeRpcServiceResponsive;

		private class RPCInterfaceContainer
		{
			public RPCInterfaceContainer(object dispatchInterface, object asyncDispatchInterface, DispatchPool dispatchPool)
			{
				this.dispatchInterface = dispatchInterface;
				this.asyncDispatchInterface = asyncDispatchInterface;
				this.dispatchPool = dispatchPool;
			}

			public object DispatchInterface
			{
				get
				{
					return this.dispatchInterface;
				}
			}

			public object AsyncDispatchInterface
			{
				get
				{
					return this.asyncDispatchInterface;
				}
			}

			public DispatchPool DispatchPool
			{
				get
				{
					return this.dispatchPool;
				}
			}

			public void Dispose()
			{
				Util.DisposeIfPresent(this.DispatchPool);
				this.dispatchPool = null;
				this.asyncDispatchInterface = null;
				this.dispatchInterface = null;
			}

			private object dispatchInterface;

			private object asyncDispatchInterface;

			private DispatchPool dispatchPool;
		}
	}
}
