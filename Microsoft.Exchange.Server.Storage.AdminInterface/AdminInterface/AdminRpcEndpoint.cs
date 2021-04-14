using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.AdminRpc;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	public sealed class AdminRpcEndpoint : IAdminRpcEndpoint
	{
		private AdminRpcEndpoint()
		{
		}

		public static IAdminRpcEndpoint Instance
		{
			get
			{
				return AdminRpcEndpoint.hookableInstance.Value;
			}
		}

		public static int GetInterfaceInstance(Guid instanceGuid, out IAdminRpcServer instance)
		{
			instance = Globals.AdminRpcServer;
			return (int)ErrorCode.NoError;
		}

		public bool StartInterface(Guid? instanceGuid, bool isLocalOnly)
		{
			bool flag = false;
			if (this.admin20server == null)
			{
				try
				{
					this.admin20server = (AdminRpcEndpoint.Admin20RpcServerEndpoint)AdminRpcServerBase.RegisterServerInstance(typeof(AdminRpcEndpoint.Admin20RpcServerEndpoint), instanceGuid, isLocalOnly, "Exchange Server STORE Admin20 Interface");
					this.admin40server = (AdminRpcEndpoint.Admin40RpcServerEndpoint)AdminRpcServerBase.RegisterServerInstance(typeof(AdminRpcEndpoint.Admin40RpcServerEndpoint), instanceGuid, isLocalOnly, "Exchange Server STORE Admin40 Interface");
					this.admin50server = (AdminRpcEndpoint.Admin50RpcServerEndpoint)AdminRpcServerBase.RegisterServerInstance(typeof(AdminRpcEndpoint.Admin50RpcServerEndpoint), instanceGuid, isLocalOnly, "Exchange Server STORE Admin50 Interface");
					flag = true;
				}
				catch (DuplicateRpcEndpointException exception)
				{
					NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
					Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_DuplicateAdminRpcEndpoint, new object[]
					{
						(instanceGuid != null) ? instanceGuid.Value : Guid.Empty,
						DiagnosticsNativeMethods.GetCurrentProcessId().ToString()
					});
				}
				finally
				{
					if (!flag)
					{
						this.StopInterface();
					}
				}
			}
			return flag;
		}

		public void StopInterface()
		{
			if (this.admin20server != null)
			{
				RpcServerBase.UnregisterInterface(Admin20RpcServer.RpcIntfHandle, true);
				this.admin20server = null;
			}
			if (this.admin40server != null)
			{
				RpcServerBase.UnregisterInterface(Admin40RpcServer.RpcIntfHandle, true);
				this.admin40server = null;
			}
			if (this.admin50server != null)
			{
				RpcServerBase.UnregisterInterface(Admin50RpcServer.RpcIntfHandle, true);
				this.admin50server = null;
			}
		}

		internal static IDisposable SetTestHook(IAdminRpcEndpoint testHook)
		{
			return AdminRpcEndpoint.SetTestHook(testHook);
		}

		private static Hookable<IAdminRpcEndpoint> hookableInstance = Hookable<IAdminRpcEndpoint>.Create(false, new AdminRpcEndpoint());

		private AdminRpcEndpoint.Admin20RpcServerEndpoint admin20server;

		private AdminRpcEndpoint.Admin40RpcServerEndpoint admin40server;

		private AdminRpcEndpoint.Admin50RpcServerEndpoint admin50server;

		private sealed class Admin20RpcServerEndpoint : Admin20RpcServer
		{
			public override int GetInterfaceInstance(Guid instanceGuid, out IAdminRpcServer instance)
			{
				return AdminRpcEndpoint.GetInterfaceInstance(instanceGuid, out instance);
			}
		}

		private sealed class Admin40RpcServerEndpoint : Admin40RpcServer
		{
			public override int GetInterfaceInstance(Guid instanceGuid, out IAdminRpcServer instance)
			{
				return AdminRpcEndpoint.GetInterfaceInstance(instanceGuid, out instance);
			}
		}

		private sealed class Admin50RpcServerEndpoint : Admin50RpcServer
		{
			public override int GetInterfaceInstance(Guid instanceGuid, out IAdminRpcServer instance)
			{
				return AdminRpcEndpoint.GetInterfaceInstance(instanceGuid, out instance);
			}
		}
	}
}
