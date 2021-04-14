using System;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Rpc.Common;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ActiveManagerGenericRpcHelper
	{
		public static int LocalServerVersion
		{
			get
			{
				if (ActiveManagerGenericRpcHelper.localServerVersion == 0)
				{
					Version version = Assembly.GetExecutingAssembly().GetName().Version;
					ServerVersion serverVersion = new ServerVersion(version.Major, version.Minor, version.Build, version.Revision);
					ActiveManagerGenericRpcHelper.localServerVersion = serverVersion.ToInt();
				}
				return ActiveManagerGenericRpcHelper.localServerVersion;
			}
		}

		public static RpcGenericReplyInfo PrepareServerReply(RpcGenericRequestInfo request, object attachedReply, int majorVersion, int minorVersion)
		{
			byte[] attachedData = SerializationServices.Serialize(attachedReply);
			return new RpcGenericReplyInfo(ActiveManagerGenericRpcHelper.LocalServerVersion, request.CommandId, majorVersion, minorVersion, attachedData);
		}

		public static RpcGenericRequestInfo PrepareClientRequest(object attachedRequest, int commandId, int majorVersion, int minorVersion)
		{
			byte[] attachedData = SerializationServices.Serialize(attachedRequest);
			return new RpcGenericRequestInfo(ActiveManagerGenericRpcHelper.LocalServerVersion, commandId, majorVersion, minorVersion, attachedData);
		}

		public static T RunRpcAndGetReply<T>(RpcGenericRequestInfo requestInfo, string serverName, int timeoutInMSec) where T : class
		{
			RpcGenericReplyInfo replyInfo = null;
			AmRpcClientHelper.RunRpcOperation(AmRpcOperationHint.GenericRpc, serverName, new int?(timeoutInMSec), delegate(AmRpcClient rpcClient, string rpcServerName)
			{
				ExTraceGlobals.ActiveMonitoringRpcTracer.TraceDebug<string>(0L, "GenericRequest(): Now making GenericRequest RPC to server {0}.", serverName);
				return rpcClient.GenericRequest(requestInfo, out replyInfo);
			});
			return SerializationServices.Deserialize<T>(replyInfo.AttachedData);
		}

		public static T ValidateAndGetAttachedRequest<T>(RpcGenericRequestInfo requestInfo, int majorVersion, int minorVersion) where T : class
		{
			if (requestInfo.CommandMajorVersion > majorVersion)
			{
				throw new ActiveManagerGenericRpcVersionNotSupportedException(requestInfo.ServerVersion, requestInfo.CommandId, requestInfo.CommandMajorVersion, requestInfo.CommandMinorVersion, ActiveManagerGenericRpcHelper.LocalServerVersion, majorVersion, minorVersion);
			}
			return SerializationServices.Deserialize<T>(requestInfo.AttachedData);
		}

		private static int localServerVersion;
	}
}
