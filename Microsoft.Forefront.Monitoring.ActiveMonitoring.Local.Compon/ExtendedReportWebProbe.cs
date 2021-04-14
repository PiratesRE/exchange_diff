using System;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	internal class ExtendedReportWebProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.ExecutionContext = string.Format("ExtendedReportWebProbe started at {0}.{1}", DateTime.UtcNow, Environment.NewLine);
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost/ExtendedReport/HealthCheck");
				httpWebRequest.Method = "GET";
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				if (httpWebResponse.StatusCode != HttpStatusCode.OK)
				{
					string text = string.Format("Health check probe is failed: {0}", httpWebResponse.ToString());
					WTFDiagnostics.TraceFunction(ExTraceGlobals.WebServiceTracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\ExtendedReportWebProbe.cs", 57);
					base.Result.FailureContext = text;
				}
				else
				{
					ProbeResult result = base.Result;
					result.ExecutionContext += "ExtendedReportWebProbe finished without error.";
					WTFDiagnostics.TraceInformation(ExTraceGlobals.WebServiceTracer, new TracingContext(), "The health check probe completed and validated.\r\n", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\ExtendedReportWebProbe.cs", 63);
				}
			}
			catch (WebException ex)
			{
				string text2 = string.Format("WebException when making web request: {0}", ex.ToString());
				WTFDiagnostics.TraceFunction(ExTraceGlobals.WebServiceTracer, base.TraceContext, text2, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\ExtendedReportWebProbe.cs", 69);
				base.Result.FailureContext = text2;
				ProbeResult result2 = base.Result;
				result2.ExecutionContext += "ExtendedReportWebProbe caught an exception.";
				throw;
			}
		}

		private const string ProbeUri = "http://localhost/ExtendedReport/HealthCheck";
	}
}
