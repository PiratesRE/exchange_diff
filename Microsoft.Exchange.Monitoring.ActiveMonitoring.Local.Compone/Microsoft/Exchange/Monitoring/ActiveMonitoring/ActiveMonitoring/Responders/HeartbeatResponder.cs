using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders
{
	public class HeartbeatResponder : ResponderWorkItem
	{
		public static ResponderDefinition CreateDefinition(string responderName, string monitorName)
		{
			return new ResponderDefinition
			{
				AssemblyPath = HeartbeatResponder.AssemblyPath,
				TypeName = HeartbeatResponder.TypeName,
				ServiceName = ExchangeComponent.Monitoring.Name,
				RecurrenceIntervalSeconds = 0,
				TimeoutSeconds = 30,
				WaitIntervalSeconds = 1,
				Enabled = true,
				Name = responderName,
				AlertMask = monitorName,
				AlertTypeId = monitorName
			};
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.HeartbeatTracer, base.TraceContext, "Heartbeat responder successfully executed.", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\HeartbeatResponder.cs", 70);
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(HeartbeatResponder).FullName;
	}
}
