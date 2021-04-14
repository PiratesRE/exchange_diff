using System;
using System.Reflection;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class RpcLogCrimsonEventImpl
	{
		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcLogCrimsonEventImpl.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcLogCrimsonEventImpl.Request>(requestInfo, 1, 0);
			RpcLogCrimsonEventImpl.LogCrimsonEventByReflection(typeof(ManagedAvailabilityCrimsonEvents), request.CrimsonEventName, request.IsPeriodic, request.Parameters);
			RpcLogCrimsonEventImpl.Reply attachedReply = new RpcLogCrimsonEventImpl.Reply();
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, attachedReply, 1, 0);
		}

		public static void SendRequest(string serverName, string crimsonEventName, bool isPeriodic, TimeSpan timeout, params object[] parameters)
		{
			RpcLogCrimsonEventImpl.Request attachedRequest = new RpcLogCrimsonEventImpl.Request(crimsonEventName, isPeriodic, parameters);
			WTFDiagnostics.TraceDebug<string, string, bool>(ExTraceGlobals.GenericRpcTracer, RpcLogCrimsonEventImpl.traceContext, "RpcLogCrimsonEventImpl.SendRequest() called. (serverName:{0}, eventName: {1}, isPeriodic: {2})", serverName, crimsonEventName, isPeriodic, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcLogCrimsonEventImpl.cs", 102);
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(attachedRequest, ActiveMonitoringGenericRpcCommandId.LogCrimsonEvent, 1, 0);
			ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcLogCrimsonEventImpl.Reply>(requestInfo, serverName, (int)timeout.TotalMilliseconds);
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.GenericRpcTracer, RpcLogCrimsonEventImpl.traceContext, "RpcLogCrimsonEventImpl.SendRequest() returned. (serverName:{0})", serverName, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcLogCrimsonEventImpl.cs", 123);
		}

		internal static void LogCrimsonEventByReflection(Type containerClass, string eventName, bool isPeriodic, object[] parameters)
		{
			FieldInfo field = containerClass.GetField(eventName);
			if (field == null)
			{
				throw new InvalidOperationException(string.Format("Failed to find event {0} in ManagedAvailabilityCrimsonEvents", eventName));
			}
			CrimsonEvent crimsonEvent = (CrimsonEvent)field.GetValue(containerClass);
			Type type = crimsonEvent.GetType();
			type.InvokeMember(isPeriodic ? "LogPeriodicGeneric" : "LogGeneric", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, crimsonEvent, parameters);
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		public const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.LogCrimsonEvent;

		private static TracingContext traceContext = TracingContext.Default;

		[Serializable]
		internal class Request
		{
			internal Request(string crimsonEventName, bool isPeriodic, object[] parameters)
			{
				this.CrimsonEventName = crimsonEventName;
				this.IsPeriodic = isPeriodic;
				this.Parameters = parameters;
			}

			internal string CrimsonEventName { get; private set; }

			internal bool IsPeriodic { get; private set; }

			internal object[] Parameters { get; private set; }
		}

		[Serializable]
		internal class Reply
		{
		}
	}
}
