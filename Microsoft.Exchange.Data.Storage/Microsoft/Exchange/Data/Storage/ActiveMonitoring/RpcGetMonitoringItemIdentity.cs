using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.ActiveMonitoring;

namespace Microsoft.Exchange.Data.Storage.ActiveMonitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RpcGetMonitoringItemIdentity
	{
		public static List<RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity> Invoke(string serverName, string healthSetName, int timeoutInMSec = 900000)
		{
			RpcGetMonitoringItemIdentity.Request attachedRequest = new RpcGetMonitoringItemIdentity.Request(healthSetName);
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(attachedRequest, ActiveMonitoringGenericRpcCommandId.GetMonitoringItemIdentity, 1, 0);
			RpcGetMonitoringItemIdentity.Reply reply = ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcGetMonitoringItemIdentity.Reply>(requestInfo, serverName, timeoutInMSec);
			return reply.MonitorIdentities;
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		public const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.GetMonitoringItemIdentity;

		[Serializable]
		public class Request
		{
			public Request(string healthSetName)
			{
				this.HealthSetName = healthSetName;
			}

			public string HealthSetName { get; set; }
		}

		[Serializable]
		public class Reply
		{
			public List<RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity> MonitorIdentities { get; set; }
		}

		[Serializable]
		public class RpcMonitorItemIdentity
		{
			public string HealthSetName { get; set; }

			public string Name { get; set; }

			public string TargetResource { get; set; }

			public string ItemType { get; set; }
		}
	}
}
