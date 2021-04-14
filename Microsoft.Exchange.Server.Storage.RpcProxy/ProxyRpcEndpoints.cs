using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.RpcProxy;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.AdminRpc;
using Microsoft.Exchange.Rpc.ExchangeServer;
using Microsoft.Exchange.Rpc.PoolRpc;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.DirectoryServices;
using Microsoft.Exchange.Server.Storage.WorkerManager;

namespace Microsoft.Exchange.Server.Storage.RpcProxy
{
	public sealed class ProxyRpcEndpoints
	{
		private ProxyRpcEndpoints(RpcInstanceManager rpcManager, ProxySessionManager sessionManager, ProxyAdminRpcServer adminServer, ProxyPoolRpcServer poolRpcServer, ProxyMapiRpcServer mapiRpcServer)
		{
			this.rpcManager = rpcManager;
			this.sessionManager = sessionManager;
			this.adminRpcServer = adminServer;
			this.poolRpcServer = poolRpcServer;
			this.mapiRpcServer = mapiRpcServer;
		}

		private static ProxyRpcEndpoints Instance
		{
			get
			{
				return ProxyRpcEndpoints.instance;
			}
		}

		public static bool Initialize(int nonRecoveryDatabasesMax, int recoveryDatabasesMax, int activeDatabasesMax)
		{
			bool flag = false;
			if (ProxyRpcEndpoints.instance == null)
			{
				try
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Initializing RPC global server.");
					string[] array = new string[2];
					string[] array2 = new string[2];
					array[0] = "ncalrpc";
					array2[0] = null;
					array[1] = "ncacn_ip_tcp";
					array2[1] = null;
					RpcServerBase.StartGlobalServer(array, array2, ProxyRpcEndpoints.GetRpcThreadCount());
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Initializing RPC instance manager.");
					RpcInstanceManager manager = new RpcInstanceManager(WorkerManager.Instance, nonRecoveryDatabasesMax, recoveryDatabasesMax, activeDatabasesMax);
					ProxySessionManager manager2 = new ProxySessionManager(manager);
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Initializing RPC endpoints.");
					ProxyRpcEndpoints.instance = new ProxyRpcEndpoints(manager, manager2, new ProxyAdminRpcServer(manager), new ProxyPoolRpcServer(manager2), new ProxyMapiRpcServer(manager2));
					flag = ProxyRpcEndpoints.instance.StartInterfaces();
				}
				catch (DuplicateRpcEndpointException ex)
				{
					NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
					if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder = new StringBuilder(200);
						stringBuilder.Append("Duplicate RPC endpoint detected. [Exception=");
						stringBuilder.Append(ex.ToString());
						stringBuilder.Append("]");
						Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, stringBuilder.ToString());
					}
				}
				finally
				{
					if (!flag)
					{
						ProxyRpcEndpoints.Terminate();
					}
				}
			}
			return flag;
		}

		public static void Terminate()
		{
			if (ProxyRpcEndpoints.instance != null)
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Stopping pool RPC server.");
				ProxyRpcEndpoints.instance.poolRpcServer.StopAcceptingClientRequests();
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Stopping RPC session manager.");
				ProxyRpcEndpoints.instance.sessionManager.StopAcceptingClientRequests();
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Stopping RPC manager.");
				ProxyRpcEndpoints.instance.rpcManager.StopAcceptingCalls();
				ProxyRpcEndpoints.instance.StopInterfaces();
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Stopping RPC global server.");
				RpcServerBase.StopGlobalServer();
				ProxyRpcEndpoints.instance = null;
			}
		}

		private static int GetAdminInterfaceInstance(Guid instanceGuid, out IAdminRpcServer instance)
		{
			instance = null;
			ProxyRpcEndpoints proxyRpcEndpoints = ProxyRpcEndpoints.Instance;
			if (proxyRpcEndpoints == null)
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.RpcProxy.ExTraceGlobals.ProxyAdminTracer.TraceError(0L, "Proxy RPC endpoint is not initialized. RPC call rejected.");
				return -2147221227;
			}
			instance = proxyRpcEndpoints.adminRpcServer;
			return (int)ErrorCode.NoError;
		}

		private static int GetPoolInterfaceInstance(Guid instanceGuid, out IPoolRpcServer instance)
		{
			instance = null;
			ProxyRpcEndpoints proxyRpcEndpoints = ProxyRpcEndpoints.Instance;
			if (proxyRpcEndpoints == null)
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.RpcProxy.ExTraceGlobals.ProxyMapiTracer.TraceError(0L, "Proxy RPC endpoint is not initialized. RPC call rejected.");
				return -2147221227;
			}
			instance = proxyRpcEndpoints.poolRpcServer;
			return (int)ErrorCode.NoError;
		}

		private static void PoolConnectionDropped(IntPtr contextHandle)
		{
			ProxyRpcEndpoints proxyRpcEndpoints = ProxyRpcEndpoints.Instance;
			if (proxyRpcEndpoints != null)
			{
				proxyRpcEndpoints.poolRpcServer.EcPoolDisconnect(contextHandle);
			}
		}

		private static IProxyServer GetMTInterfaceInstace()
		{
			ProxyRpcEndpoints proxyRpcEndpoints = ProxyRpcEndpoints.Instance;
			if (proxyRpcEndpoints == null)
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.RpcProxy.ExTraceGlobals.ProxyMapiTracer.TraceError(0L, "Proxy RPC endpoint is not initialized. RPC call rejected.");
				throw new FailRpcException("EmsmdbMT interface not started", -2147221227);
			}
			return proxyRpcEndpoints.mapiRpcServer;
		}

		private static uint GetRpcThreadCount()
		{
			int? maximumRpcThreadCount = ((IRpcProxyDirectory)Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory).GetMaximumRpcThreadCount(NullExecutionContext.Instance);
			if (maximumRpcThreadCount == null || maximumRpcThreadCount.Value <= 0)
			{
				return 500U;
			}
			return (uint)maximumRpcThreadCount.Value;
		}

		private bool StartInterfaces()
		{
			bool flag = false;
			if (this.admin20server == null)
			{
				try
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Starting admin20 RPC interface.");
					this.admin20server = (ProxyRpcEndpoints.Admin20RpcServerEndpoint)RpcServerBase.RegisterInterface(typeof(ProxyRpcEndpoints.Admin20RpcServerEndpoint), true, true, "Exchange Server STORE Admin20 Proxy Interface");
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Starting admin40 RPC interface.");
					this.admin40server = (ProxyRpcEndpoints.Admin40RpcServerEndpoint)RpcServerBase.RegisterInterface(typeof(ProxyRpcEndpoints.Admin40RpcServerEndpoint), true, true, "Exchange Server STORE Admin40 Proxy Interface");
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Starting admin50 RPC interface.");
					this.admin50server = (ProxyRpcEndpoints.Admin50RpcServerEndpoint)RpcServerBase.RegisterInterface(typeof(ProxyRpcEndpoints.Admin50RpcServerEndpoint), true, true, "Exchange Server STORE Admin50 Proxy Interface");
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Starting emsmdbPool RPC interface.");
					this.poolServer = (ProxyRpcEndpoints.PoolRpcServerEndpoint)RpcServerBase.RegisterInterface(typeof(ProxyRpcEndpoints.PoolRpcServerEndpoint), true, true, "Exchange Server STORE EmsmdbPool Proxy Interface");
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Starting emsmdbPoolNotify RPC interface.");
					this.poolNotifyServer = (ProxyRpcEndpoints.PoolNotifyRpcServerEndpoint)RpcServerBase.RegisterAutoListenInterface(typeof(ProxyRpcEndpoints.PoolNotifyRpcServerEndpoint), 65536, false, true, "Exchange Server STORE EmsmdbPoolNotify Proxy Interface");
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Starting emsmdbMT RPC interface.");
					this.mapiServer = (ProxyRpcEndpoints.MapiMTRpcServerEndpoint)RpcServerBase.RegisterInterface(typeof(ProxyRpcEndpoints.MapiMTRpcServerEndpoint), true, "Exchange Server STORE EmsmdbMT Proxy Interface");
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Starting emsmdbMTAsync RPC interface.");
					this.asyncMapiServer = (ProxyRpcEndpoints.AsyncMapiMTRpcServerEndpoint)RpcServerBase.RegisterAutoListenInterface(typeof(ProxyRpcEndpoints.AsyncMapiMTRpcServerEndpoint), 65536);
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						this.StopInterfaces();
					}
				}
			}
			return flag;
		}

		private void StopInterfaces()
		{
			if (this.admin20server != null)
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Stopping admin20 RPC interface.");
				RpcServerBase.UnregisterInterface(Admin20RpcServer.RpcIntfHandle, true);
				this.admin20server = null;
			}
			if (this.admin40server != null)
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Stopping admin40 RPC interface.");
				RpcServerBase.UnregisterInterface(Admin40RpcServer.RpcIntfHandle, true);
				this.admin40server = null;
			}
			if (this.admin50server != null)
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Stopping admin50 RPC interface.");
				RpcServerBase.UnregisterInterface(Admin50RpcServer.RpcIntfHandle, true);
				this.admin50server = null;
			}
			if (this.poolServer != null)
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Stopping emsmdbPool RPC interface.");
				RpcServerBase.UnregisterInterface(PoolRpcServerBase.RpcIntfHandle, true);
				this.poolServer = null;
			}
			if (this.poolNotifyServer != null)
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Stopping emsmdbPoolNotify RPC interface.");
				RpcServerBase.UnregisterInterface(PoolNotifyRpcServerBase.RpcIntfHandle, true);
				this.poolNotifyServer = null;
			}
			if (this.mapiServer != null)
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Stopping emsmdbMT RPC interface.");
				RpcServerBase.UnregisterInterface(ExchangeRpcServerMT.RpcIntfHandle, true);
				this.mapiServer = null;
			}
			if (this.asyncMapiServer != null)
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.Service.ExTraceGlobals.StartupShutdownTracer.TraceDebug(0L, "Stopping asyncemsmdbMT RPC interface.");
				RpcServerBase.UnregisterInterface(ExchangeRpcServerMTAsync.RpcIntfHandle, true);
				this.asyncMapiServer = null;
			}
		}

		private const int MaximumMTConnections = 65536;

		private const int MaximumNotificationCalls = 65536;

		private const uint DefaultMaximumRpcThreads = 500U;

		private const string ProtocolLRPC = "ncalrpc";

		private const string ProtocolTCP = "ncacn_ip_tcp";

		private static ProxyRpcEndpoints instance;

		private RpcInstanceManager rpcManager;

		private ProxySessionManager sessionManager;

		private ProxyRpcEndpoints.Admin20RpcServerEndpoint admin20server;

		private ProxyRpcEndpoints.Admin40RpcServerEndpoint admin40server;

		private ProxyRpcEndpoints.Admin50RpcServerEndpoint admin50server;

		private ProxyRpcEndpoints.PoolRpcServerEndpoint poolServer;

		private ProxyRpcEndpoints.PoolNotifyRpcServerEndpoint poolNotifyServer;

		private ProxyRpcEndpoints.MapiMTRpcServerEndpoint mapiServer;

		private ProxyRpcEndpoints.AsyncMapiMTRpcServerEndpoint asyncMapiServer;

		private ProxyAdminRpcServer adminRpcServer;

		private ProxyPoolRpcServer poolRpcServer;

		private ProxyMapiRpcServer mapiRpcServer;

		private sealed class Admin20RpcServerEndpoint : Admin20RpcServer
		{
			public override int GetInterfaceInstance(Guid instanceGuid, out IAdminRpcServer instance)
			{
				return ProxyRpcEndpoints.GetAdminInterfaceInstance(instanceGuid, out instance);
			}
		}

		private sealed class Admin40RpcServerEndpoint : Admin40RpcServer
		{
			public override int GetInterfaceInstance(Guid instanceGuid, out IAdminRpcServer instance)
			{
				return ProxyRpcEndpoints.GetAdminInterfaceInstance(instanceGuid, out instance);
			}
		}

		private sealed class Admin50RpcServerEndpoint : Admin50RpcServer
		{
			public override int GetInterfaceInstance(Guid instanceGuid, out IAdminRpcServer instance)
			{
				return ProxyRpcEndpoints.GetAdminInterfaceInstance(instanceGuid, out instance);
			}
		}

		private sealed class PoolRpcServerEndpoint : PoolRpcServerBase
		{
			public override int GetInterfaceInstance(Guid instanceGuid, out IPoolRpcServer instance)
			{
				return ProxyRpcEndpoints.GetPoolInterfaceInstance(instanceGuid, out instance);
			}

			public override void ConnectionDropped(IntPtr contextHandle)
			{
				ProxyRpcEndpoints.PoolConnectionDropped(contextHandle);
			}
		}

		private sealed class PoolNotifyRpcServerEndpoint : PoolNotifyRpcServerBase
		{
			public override int GetInterfaceInstance(Guid instanceGuid, out IPoolRpcServer instance)
			{
				return ProxyRpcEndpoints.GetPoolInterfaceInstance(instanceGuid, out instance);
			}

			public override void ConnectionDropped(IntPtr contextHandle)
			{
				ProxyRpcEndpoints.PoolConnectionDropped(contextHandle);
			}
		}

		private sealed class MapiMTRpcServerEndpoint : ExchangeRpcServerMT
		{
			public override IProxyServer GetProxyServer()
			{
				return ProxyRpcEndpoints.GetMTInterfaceInstace();
			}
		}

		private sealed class AsyncMapiMTRpcServerEndpoint : ExchangeRpcServerMTAsync
		{
			public override IProxyServer GetProxyServer()
			{
				return ProxyRpcEndpoints.GetMTInterfaceInstace();
			}
		}
	}
}
