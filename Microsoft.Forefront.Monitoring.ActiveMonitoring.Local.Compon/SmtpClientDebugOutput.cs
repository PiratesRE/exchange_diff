using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Net.ExSmtpClient;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	internal class SmtpClientDebugOutput : ISmtpClientDebugOutput
	{
		public void Output(Trace tracer, object context, string message, params object[] args)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.SMTPTracer, this.traceContext, message, args, null, "Output", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpClientDebugOutput.cs", 48);
		}

		private TracingContext traceContext = new TracingContext();
	}
}
