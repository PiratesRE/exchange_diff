using System;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Data.Storage.ActiveMonitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ActiveMonitoringGenericRpcHelper
	{
		public static int LocalServerVersion
		{
			get
			{
				if (ActiveMonitoringGenericRpcHelper.localServerVersion == 0)
				{
					lock (ActiveMonitoringGenericRpcHelper.locker)
					{
						if (ActiveMonitoringGenericRpcHelper.localServerVersion == 0)
						{
							Version version = Assembly.GetExecutingAssembly().GetName().Version;
							ServerVersion serverVersion = new ServerVersion(version.Major, version.Minor, version.Build, version.Revision);
							ActiveMonitoringGenericRpcHelper.localServerVersion = serverVersion.ToInt();
						}
					}
				}
				return ActiveMonitoringGenericRpcHelper.localServerVersion;
			}
		}

		public static ActiveMonitoringRpcClient RpcClientFactory(string serverName, int timeoutMs)
		{
			ActiveMonitoringRpcClient activeMonitoringRpcClient = new ActiveMonitoringRpcClient(serverName);
			if (timeoutMs != -1)
			{
				activeMonitoringRpcClient.SetTimeOut(timeoutMs);
			}
			return activeMonitoringRpcClient;
		}

		public static void RunRpcOperation(string serverName, int timeoutMs, ActiveMonitoringRpcExceptionWrapper rpcExceptionWrapperInstance, ActiveMonitoringGenericRpcHelper.InternalRpcOperation rpcOperation)
		{
			RpcErrorExceptionInfo errorInfo = null;
			rpcExceptionWrapperInstance.ClientRetryableOperation(serverName, delegate
			{
				using (ActiveMonitoringRpcClient activeMonitoringRpcClient = ActiveMonitoringGenericRpcHelper.RpcClientFactory(serverName, timeoutMs))
				{
					errorInfo = rpcOperation(activeMonitoringRpcClient);
				}
			});
			rpcExceptionWrapperInstance.ClientRethrowIfFailed(serverName, errorInfo);
		}

		public static RpcGenericReplyInfo PrepareServerReply(RpcGenericRequestInfo request, object attachedReply, int majorVersion, int minorVersion)
		{
			byte[] attachedData = SerializationServices.Serialize(attachedReply);
			return new RpcGenericReplyInfo(ActiveMonitoringGenericRpcHelper.LocalServerVersion, request.CommandId, majorVersion, minorVersion, attachedData);
		}

		public static RpcGenericRequestInfo PrepareClientRequest(object attachedRequest, ActiveMonitoringGenericRpcCommandId commandId, int majorVersion, int minorVersion)
		{
			byte[] attachedData = SerializationServices.Serialize(attachedRequest);
			return new RpcGenericRequestInfo(ActiveMonitoringGenericRpcHelper.LocalServerVersion, (int)commandId, majorVersion, minorVersion, attachedData);
		}

		public static T RunRpcAndGetReply<T>(RpcGenericRequestInfo requestInfo, string serverName, int timeoutInMSec) where T : class
		{
			RpcGenericReplyInfo replyInfo = null;
			ActiveMonitoringGenericRpcHelper.RunRpcOperation(serverName, timeoutInMSec, ActiveMonitoringRpcExceptionWrapper.Instance, delegate(ActiveMonitoringRpcClient rpcClient)
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
				throw new ActiveMonitoringRpcVersionNotSupportedException(requestInfo.ServerVersion, requestInfo.CommandId, requestInfo.CommandMajorVersion, requestInfo.CommandMinorVersion, ActiveMonitoringGenericRpcHelper.LocalServerVersion, majorVersion, minorVersion);
			}
			return SerializationServices.Deserialize<T>(requestInfo.AttachedData);
		}

		public const int TinyRpcTimeoutMs = 5000;

		public const int ShortRpcTimeoutMs = 30000;

		public const int LongRpcTimeoutMs = 300000;

		public const int LongerRpcTimeoutMs = 900000;

		private static int localServerVersion;

		private static object locker = new object();

		public delegate RpcErrorExceptionInfo InternalRpcOperation(ActiveMonitoringRpcClient rpcClient);
	}
}
