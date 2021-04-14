using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class RpcGetMonitoringItemIdentityImpl
	{
		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcGetMonitoringItemIdentity.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcGetMonitoringItemIdentity.Request>(requestInfo, 1, 0);
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, new RpcGetMonitoringItemIdentity.Reply
			{
				MonitorIdentities = new List<RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity>(),
				MonitorIdentities = RpcGetMonitoringItemIdentityImpl.GetAllRpcMonitorIdentities(request.HealthSetName)
			}, 1, 0);
		}

		private static List<RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity> GetAllRpcMonitorIdentities(string healthSetName)
		{
			List<RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity> list = new List<RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity>();
			list.AddRange(RpcGetMonitoringItemIdentityImpl.GetRpcMonitorIdentitiesFromCrimsonDefinition<ProbeDefinition>(healthSetName));
			list.AddRange(RpcGetMonitoringItemIdentityImpl.GetRpcMonitorIdentitiesFromCrimsonDefinition<MonitorDefinition>(healthSetName));
			list.AddRange(RpcGetMonitoringItemIdentityImpl.GetRpcMonitorIdentitiesFromCrimsonDefinition<ResponderDefinition>(healthSetName));
			list.AddRange(RpcGetMonitoringItemIdentityImpl.GetRpcMonitorIdentitiesFromCrimsonDefinition<MaintenanceDefinition>(healthSetName));
			return list;
		}

		private static List<RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity> GetRpcMonitorIdentitiesFromCrimsonDefinition<T>(string healthSetName) where T : WorkDefinition, IPersistence, new()
		{
			List<RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity> list = new List<RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity>();
			using (CrimsonReader<T> crimsonReader = new CrimsonReader<T>())
			{
				for (WorkDefinition workDefinition = crimsonReader.ReadNext(); workDefinition != null; workDefinition = crimsonReader.ReadNext())
				{
					if (string.Equals(healthSetName, workDefinition.ServiceName, StringComparison.InvariantCultureIgnoreCase))
					{
						list.Add(new RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity
						{
							HealthSetName = workDefinition.ServiceName,
							Name = workDefinition.Name,
							TargetResource = workDefinition.TargetResource,
							ItemType = RpcGetMonitoringItemIdentityImpl.GetTypeNameFromDefinition<T>()
						});
					}
				}
			}
			return list;
		}

		private static string GetTypeNameFromDefinition<T>() where T : WorkDefinition, IPersistence, new()
		{
			if (typeof(T) == typeof(ProbeDefinition))
			{
				return "Probe";
			}
			if (typeof(T) == typeof(MonitorDefinition))
			{
				return "Monitor";
			}
			if (typeof(T) == typeof(ResponderDefinition))
			{
				return "Responder";
			}
			if (typeof(T) == typeof(MaintenanceDefinition))
			{
				return "Maintenance";
			}
			return "Unknown";
		}

		public const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.GetMonitoringItemIdentity;
	}
}
