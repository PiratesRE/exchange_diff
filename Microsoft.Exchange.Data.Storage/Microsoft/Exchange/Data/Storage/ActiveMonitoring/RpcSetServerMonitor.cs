using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.ActiveMonitoring;

namespace Microsoft.Exchange.Data.Storage.ActiveMonitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RpcSetServerMonitor
	{
		public static void Invoke(string serverName, string monitorName, string targetResource, bool? isRepairing, int timeoutInMSec = 30000)
		{
			RpcSetServerMonitor.Request attachedRequest = new RpcSetServerMonitor.Request(monitorName, targetResource, isRepairing);
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(attachedRequest, ActiveMonitoringGenericRpcCommandId.SetServerMonitor, 1, 0);
			ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcSetServerMonitor.Reply>(requestInfo, serverName, timeoutInMSec);
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		public const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.SetServerMonitor;

		[Serializable]
		public class Request
		{
			public Request(string monitorName, string targetResource, bool? isRepairing)
			{
				this.MonitorName = monitorName;
				this.TargetResource = targetResource;
				this.IsRepairing = isRepairing;
			}

			public string MonitorName { get; set; }

			public string TargetResource { get; set; }

			public bool? IsRepairing { get; set; }
		}

		[Serializable]
		public class Reply
		{
		}
	}
}
