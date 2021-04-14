using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Data.Storage.ActiveMonitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RpcGetMonitoringItemHelp
	{
		public static List<PropertyInformation> Invoke(string serverName, string monitorIdentity, int timeoutInMSec = 900000)
		{
			RpcGetMonitoringItemHelp.Request attachedRequest = new RpcGetMonitoringItemHelp.Request(monitorIdentity);
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(attachedRequest, ActiveMonitoringGenericRpcCommandId.GetMonitoringItemHelp, 1, 0);
			RpcGetMonitoringItemHelp.Reply reply = ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcGetMonitoringItemHelp.Reply>(requestInfo, serverName, timeoutInMSec);
			return reply.HelpEntries;
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		public const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.GetMonitoringItemHelp;

		[Serializable]
		public class Request
		{
			public Request(string monitorIdentity)
			{
				this.MonitorIdentity = monitorIdentity;
			}

			public string MonitorIdentity { get; set; }
		}

		[Serializable]
		public class Reply
		{
			public List<PropertyInformation> HelpEntries { get; set; }
		}
	}
}
