using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Data.Storage.ActiveMonitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RpcInvokeMonitoringProbe
	{
		public static RpcInvokeMonitoringProbe.Reply Invoke(string serverName, string identity, string propertyBagXml, string extensionAttributes, int timeoutInMSec = 300000)
		{
			RpcInvokeNowCommon.Request attachedRequest = new RpcInvokeNowCommon.Request(identity, propertyBagXml, extensionAttributes);
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(attachedRequest, ActiveMonitoringGenericRpcCommandId.InvokeMonitoringProbe, 1, 0);
			return ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcInvokeMonitoringProbe.Reply>(requestInfo, serverName, timeoutInMSec);
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		public const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.InvokeMonitoringProbe;

		[Serializable]
		public class Reply
		{
			public RpcInvokeMonitoringProbe.RpcMonitorProbeResult ProbeResult { get; set; }

			public string ErrorMessage { get; set; }
		}

		[Serializable]
		public class RpcMonitorProbeResult
		{
			public string MonitorIdentity { get; set; }

			public Guid RequestId { get; set; }

			public DateTime ExecutionStartTime { get; set; }

			public DateTime ExecutionEndTime { get; set; }

			public string Error { get; set; }

			public string Exception { get; set; }

			public byte PoisonedCount { get; set; }

			public int ExecutionId { get; set; }

			public double SampleValue { get; set; }

			public string ExecutionContext { get; set; }

			public string FailureContext { get; set; }

			public string ExtensionXml { get; set; }

			public ResultType ResultType { get; set; }

			public byte RetryCount { get; set; }

			public string ResultName { get; set; }

			public bool IsNotified { get; set; }

			public int ResultId { get; set; }

			public string ServiceName { get; set; }

			public string StateAttribute1 { get; set; }

			public string StateAttribute2 { get; set; }

			public string StateAttribute3 { get; set; }

			public string StateAttribute4 { get; set; }

			public string StateAttribute5 { get; set; }

			public double StateAttribute6 { get; set; }

			public double StateAttribute7 { get; set; }

			public double StateAttribute8 { get; set; }

			public double StateAttribute9 { get; set; }

			public double StateAttribute10 { get; set; }

			public string StateAttribute11 { get; set; }

			public string StateAttribute12 { get; set; }

			public string StateAttribute13 { get; set; }

			public string StateAttribute14 { get; set; }

			public string StateAttribute15 { get; set; }

			public double StateAttribute16 { get; set; }

			public double StateAttribute17 { get; set; }

			public double StateAttribute18 { get; set; }

			public double StateAttribute19 { get; set; }

			public double StateAttribute20 { get; set; }

			public string StateAttribute21 { get; set; }

			public string StateAttribute22 { get; set; }

			public string StateAttribute23 { get; set; }

			public string StateAttribute24 { get; set; }

			public string StateAttribute25 { get; set; }
		}
	}
}
