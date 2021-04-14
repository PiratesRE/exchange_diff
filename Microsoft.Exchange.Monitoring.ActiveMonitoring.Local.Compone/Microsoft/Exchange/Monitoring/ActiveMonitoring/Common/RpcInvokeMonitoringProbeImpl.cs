using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class RpcInvokeMonitoringProbeImpl
	{
		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcInvokeMonitoringProbe.Reply reply = new RpcInvokeMonitoringProbe.Reply();
			reply.ProbeResult = new RpcInvokeMonitoringProbe.RpcMonitorProbeResult();
			try
			{
				RpcInvokeNowCommon.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcInvokeNowCommon.Request>(requestInfo, 1, 0);
				RpcInvokeMonitoringProbeImpl.RunInvokeNow(request, reply);
			}
			catch (Exception ex)
			{
				reply.ErrorMessage = ex.Message;
			}
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, reply, 1, 0);
		}

		private static void RunInvokeNow(RpcInvokeNowCommon.Request request, RpcInvokeMonitoringProbe.Reply reply)
		{
			DateTime localTime = ExDateTime.Now.LocalTime;
			string text = Guid.NewGuid().ToString("N");
			int workDefinitionId = 0;
			ProbeResult result = null;
			if (!RpcInvokeMonitoringProbeImpl.TryLogInvokeNowRequest(request, localTime, text, reply))
			{
				return;
			}
			if (!RpcInvokeMonitoringProbeImpl.TryGetPickupStartEvent(localTime, text, reply))
			{
				return;
			}
			if (!RpcInvokeMonitoringProbeImpl.TryGetPickupEndEvent(localTime, text, reply, ref workDefinitionId))
			{
				return;
			}
			if (!RpcInvokeMonitoringProbeImpl.TryGetProbeResult(localTime, text, reply, workDefinitionId, ref result))
			{
				return;
			}
			RpcInvokeMonitoringProbeImpl.SetReplyInformation(request, reply, result, text);
			ManagedAvailabilityCrimsonEvents.InvokeNowSucceeded.Log<string, string, string, string, string, string, string, InvokeNowState, InvokeNowResult, string, string>(text, string.Empty, string.Empty, request.MonitorIdentity, request.PropertyBag, request.ExtensionAttributes, localTime.ToString(), InvokeNowState.MonitorInvokeFinished, InvokeNowResult.Succeeded, string.Empty, workDefinitionId.ToString());
		}

		private static bool TryLogInvokeNowRequest(RpcInvokeNowCommon.Request request, DateTime requestTime, string requestId, RpcInvokeMonitoringProbe.Reply reply)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			RpcInvokeMonitoringProbeImpl.SetAssemblyPathAndType(request.MonitorIdentity, ref empty, ref empty2);
			if (string.IsNullOrEmpty(empty) || string.IsNullOrEmpty(empty2))
			{
				reply.ErrorMessage = Strings.InvokeNowAssemblyInfoFailure(request.MonitorIdentity);
				ManagedAvailabilityCrimsonEvents.InvokeNowFailed.Log<string, string, string, string, string, string, string, InvokeNowState, InvokeNowResult, string, string>(requestId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, InvokeNowState.None, InvokeNowResult.Failed, reply.ErrorMessage, string.Empty);
				return false;
			}
			ManagedAvailabilityCrimsonEvents.InvokeNowRequest.Log<string, string, string, string, string, string, string, string, string, string, string>(requestId, empty2, empty, request.MonitorIdentity, request.PropertyBag, request.ExtensionAttributes, requestTime.ToString(), string.Empty, string.Empty, string.Empty, string.Empty);
			return true;
		}

		private static bool TryGetProbeResult(DateTime requestTime, string requestId, RpcInvokeMonitoringProbe.Reply reply, int workDefinitionId, ref ProbeResult result)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			while (stopwatch.Elapsed < RpcInvokeMonitoringProbeImpl.MaxWaitTimeSpan && result == null)
			{
				using (CrimsonReader<ProbeResult> crimsonReader = new CrimsonReader<ProbeResult>())
				{
					crimsonReader.QueryUserPropertyCondition = string.Format("(WorkItemId='{0}')", workDefinitionId);
					crimsonReader.IsReverseDirection = true;
					crimsonReader.QueryStartTime = new DateTime?(requestTime);
					crimsonReader.QueryEndTime = new DateTime?(ExDateTime.Now.LocalTime);
					result = crimsonReader.ReadNext();
				}
				if (result == null)
				{
					Thread.Sleep(5000);
				}
			}
			if (result == null)
			{
				reply.ErrorMessage = Strings.InvokeNowProbeResultNotFound(requestId, workDefinitionId);
				ManagedAvailabilityCrimsonEvents.InvokeNowFailed.Log<string, string, string, string, string, string, string, InvokeNowState, InvokeNowResult, string, string>(requestId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, InvokeNowState.None, InvokeNowResult.Failed, reply.ErrorMessage, string.Empty);
				return false;
			}
			return true;
		}

		private static bool TryGetPickupEndEvent(DateTime requestTime, string requestId, RpcInvokeMonitoringProbe.Reply reply, ref int workDefinitionId)
		{
			InvokeNowEntry invokeNowEntry = null;
			Stopwatch stopwatch = Stopwatch.StartNew();
			while (stopwatch.Elapsed < RpcInvokeMonitoringProbeImpl.MaxWaitTimeSpan && invokeNowEntry == null)
			{
				invokeNowEntry = RpcInvokeMonitoringProbeImpl.GetInvokeNowResult(requestTime, requestId, 2003);
				if (invokeNowEntry == null)
				{
					invokeNowEntry = RpcInvokeMonitoringProbeImpl.GetInvokeNowResult(requestTime, requestId, 2004);
					if (invokeNowEntry != null)
					{
						break;
					}
				}
				if (invokeNowEntry == null)
				{
					Thread.Sleep(5000);
				}
			}
			if (invokeNowEntry == null || invokeNowEntry.Result == InvokeNowResult.Failed)
			{
				reply.ErrorMessage = ((invokeNowEntry != null && !string.IsNullOrWhiteSpace(invokeNowEntry.ErrorMessage)) ? invokeNowEntry.ErrorMessage : Strings.InvokeNowDefinitionFailure(requestId));
				ManagedAvailabilityCrimsonEvents.InvokeNowFailed.Log<string, string, string, string, string, string, string, InvokeNowState, InvokeNowResult, string, string>(requestId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, InvokeNowState.None, InvokeNowResult.Failed, reply.ErrorMessage, string.Empty);
				return false;
			}
			if (!int.TryParse(invokeNowEntry.WorkDefinitionId, out workDefinitionId))
			{
				reply.ErrorMessage = Strings.InvokeNowInvalidWorkDefinition(requestId);
				ManagedAvailabilityCrimsonEvents.InvokeNowFailed.Log<string, string, string, string, string, string, string, InvokeNowState, InvokeNowResult, string, string>(requestId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, InvokeNowState.None, InvokeNowResult.Failed, reply.ErrorMessage, string.Empty);
				return false;
			}
			return true;
		}

		private static bool TryGetPickupStartEvent(DateTime requestTime, string requestId, RpcInvokeMonitoringProbe.Reply reply)
		{
			InvokeNowEntry invokeNowEntry = null;
			Stopwatch stopwatch = Stopwatch.StartNew();
			while (stopwatch.Elapsed < RpcInvokeMonitoringProbeImpl.MaxWaitTimeSpan && invokeNowEntry == null)
			{
				invokeNowEntry = RpcInvokeMonitoringProbeImpl.GetInvokeNowResult(requestTime, requestId, 2002);
				if (invokeNowEntry == null)
				{
					Thread.Sleep(5000);
				}
			}
			if (invokeNowEntry == null)
			{
				reply.ErrorMessage = Strings.InvokeNowPickupEventNotFound(requestId);
				ManagedAvailabilityCrimsonEvents.InvokeNowFailed.Log<string, string, string, string, string, string, string, InvokeNowState, InvokeNowResult, string, string>(requestId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, InvokeNowState.None, InvokeNowResult.Failed, reply.ErrorMessage, string.Empty);
				return false;
			}
			return true;
		}

		private static void SetReplyInformation(RpcInvokeNowCommon.Request request, RpcInvokeMonitoringProbe.Reply reply, ProbeResult result, string requestId)
		{
			reply.ProbeResult.Error = result.Error;
			reply.ProbeResult.Exception = result.Exception;
			reply.ProbeResult.ExecutionContext = result.ExecutionContext;
			reply.ProbeResult.ExecutionEndTime = result.ExecutionEndTime;
			reply.ProbeResult.ExecutionId = result.ExecutionId;
			reply.ProbeResult.ExecutionStartTime = result.ExecutionStartTime;
			reply.ProbeResult.ExtensionXml = result.ExtensionXml;
			reply.ProbeResult.FailureContext = result.FailureContext;
			reply.ProbeResult.IsNotified = result.IsNotified;
			reply.ProbeResult.MonitorIdentity = request.MonitorIdentity;
			reply.ProbeResult.PoisonedCount = result.PoisonedCount;
			reply.ProbeResult.RequestId = Guid.Parse(requestId);
			reply.ProbeResult.ResultId = result.ResultId;
			reply.ProbeResult.ResultName = result.ResultName;
			reply.ProbeResult.ResultType = result.ResultType;
			reply.ProbeResult.RetryCount = result.RetryCount;
			reply.ProbeResult.SampleValue = result.SampleValue;
			reply.ProbeResult.ServiceName = result.ServiceName;
			reply.ProbeResult.StateAttribute1 = result.StateAttribute1;
			reply.ProbeResult.StateAttribute2 = result.StateAttribute2;
			reply.ProbeResult.StateAttribute3 = result.StateAttribute3;
			reply.ProbeResult.StateAttribute4 = result.StateAttribute4;
			reply.ProbeResult.StateAttribute5 = result.StateAttribute5;
			reply.ProbeResult.StateAttribute6 = result.StateAttribute6;
			reply.ProbeResult.StateAttribute7 = result.StateAttribute7;
			reply.ProbeResult.StateAttribute8 = result.StateAttribute8;
			reply.ProbeResult.StateAttribute9 = result.StateAttribute9;
			reply.ProbeResult.StateAttribute10 = result.StateAttribute10;
			reply.ProbeResult.StateAttribute11 = result.StateAttribute11;
			reply.ProbeResult.StateAttribute12 = result.StateAttribute12;
			reply.ProbeResult.StateAttribute13 = result.StateAttribute13;
			reply.ProbeResult.StateAttribute14 = result.StateAttribute14;
			reply.ProbeResult.StateAttribute15 = result.StateAttribute15;
			reply.ProbeResult.StateAttribute16 = result.StateAttribute16;
			reply.ProbeResult.StateAttribute17 = result.StateAttribute17;
			reply.ProbeResult.StateAttribute18 = result.StateAttribute18;
			reply.ProbeResult.StateAttribute19 = result.StateAttribute19;
			reply.ProbeResult.StateAttribute20 = result.StateAttribute20;
			reply.ProbeResult.StateAttribute21 = result.StateAttribute21;
			reply.ProbeResult.StateAttribute22 = result.StateAttribute22;
			reply.ProbeResult.StateAttribute23 = result.StateAttribute23;
			reply.ProbeResult.StateAttribute24 = result.StateAttribute24;
			reply.ProbeResult.StateAttribute25 = result.StateAttribute25;
		}

		private static InvokeNowEntry GetInvokeNowResult(DateTime startTime, string requestId, int eventId)
		{
			using (CrimsonReader<InvokeNowEntry> crimsonReader = new CrimsonReader<InvokeNowEntry>(null, null, "Microsoft-Exchange-ManagedAvailability/InvokeNowResult"))
			{
				crimsonReader.QueryEndTime = new DateTime?(ExDateTime.Now.LocalTime.AddDays(1.0));
				crimsonReader.QueryStartTime = new DateTime?(startTime);
				while (!crimsonReader.EndOfEventsReached)
				{
					InvokeNowEntry invokeNowEntry = crimsonReader.ReadNext();
					if (invokeNowEntry != null && requestId.Equals(invokeNowEntry.Id.ToString("N"), StringComparison.InvariantCultureIgnoreCase) && invokeNowEntry.LocalDataAccessMetaData.EventId == eventId)
					{
						return invokeNowEntry;
					}
				}
			}
			return null;
		}

		private static void SetAssemblyPathAndType(string monitorIdentity, ref string assemblyPath, ref string typeName)
		{
			WorkDefinition workDefinition = InvokeNowCommon.GetWorkDefinition(monitorIdentity);
			if (workDefinition != null && workDefinition is ProbeDefinition)
			{
				assemblyPath = workDefinition.AssemblyPath;
				typeName = workDefinition.TypeName;
			}
		}

		public const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.InvokeMonitoringProbe;

		private const int WaitTimeInSeconds = 90;

		private const int SleepTimeInSeconds = 5;

		private static readonly TimeSpan MaxWaitTimeSpan = TimeSpan.FromSeconds(90.0);
	}
}
