using System;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Transport.Probes
{
	internal class Tracer : ITracer
	{
		public void TraceDebug(string debugInfo)
		{
			WTFDiagnostics.TraceDebug(ExTraceGlobals.MapiSubmitLAMTracer, new TracingContext(), debugInfo, null, "TraceDebug", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\MailboxTransport\\Tracer.cs", 56);
		}

		public void TraceError(string errorInfo)
		{
			WTFDiagnostics.TraceError(ExTraceGlobals.MapiSubmitLAMTracer, new TracingContext(), errorInfo, null, "TraceError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\MailboxTransport\\Tracer.cs", 65);
		}
	}
}
