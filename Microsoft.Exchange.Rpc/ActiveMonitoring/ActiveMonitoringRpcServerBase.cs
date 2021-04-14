using System;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Rpc.ActiveMonitoring
{
	internal abstract class ActiveMonitoringRpcServerBase : RpcServerBase
	{
		public abstract int RequestMonitoring(Guid mdbGuid);

		public abstract void CancelMonitoring(Guid mdbGuid);

		public abstract int Heartbeat(string serverName, Guid mdbGuid);

		public abstract int RequestCredential(string serverName, Guid mdbGuid, string userPrincipalName, ref string credential);

		public abstract RpcErrorExceptionInfo GenericRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo);

		public abstract int CreateMonitoringMailbox(string displayName, Guid mdbGuid);

		public static void StopServer()
		{
			RpcServerBase.StopServer(ActiveMonitoringRpcServerBase.RpcIntfHandle);
		}

		public ActiveMonitoringRpcServerBase()
		{
		}

		internal static IntPtr RpcIntfHandle = (IntPtr)<Module>.IActiveMonitoringRpc_v1_0_s_ifspec;
	}
}
