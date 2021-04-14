using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Hygiene.AsyncQueue.DataGeneration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class AsyncQueueProbeNewRequest : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation<DateTime>(ExTraceGlobals.DNSTracer, base.TraceContext, "AyncQueueProbeNewRequest : DoWork start, currentTime={0}", DateTime.UtcNow, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\AsyncQueue\\Probes\\AsyncQueueProbeNewRequest.cs", 33);
			Stopwatch stopwatch = Stopwatch.StartNew();
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			int num = 0;
			stringBuilder.Append("AsyncQueueProbeNewRequest starting");
			try
			{
				StressStepGeneration stressStepGeneration = new StressStepGeneration();
				stressStepGeneration.StressStepGenerationByXMLString(base.Definition.ExtensionAttributes);
				num = stressStepGeneration.ProduceRequests();
				stringBuilder.AppendFormat("New Requests={0}", num);
				WTFDiagnostics.TraceInformation<int>(ExTraceGlobals.AsyncEngineTracer, base.TraceContext, "AyncQueueProbeNewRequest: Generated new probe requests: {0}", num, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\AsyncQueue\\Probes\\AsyncQueueProbeNewRequest.cs", 50);
				if (num == 0)
				{
					throw new AsyncQueueMonitorException(string.Format("Probe requests were not created", new object[0]));
				}
			}
			catch (Exception arg)
			{
				WTFDiagnostics.TraceInformation<Exception>(ExTraceGlobals.AsyncEngineTracer, base.TraceContext, "Exception:{0}", arg, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\AsyncQueue\\Probes\\AsyncQueueProbeNewRequest.cs", 58);
				stringBuilder2.AppendFormat("Exception={0}", arg);
				throw;
			}
			finally
			{
				stopwatch.Stop();
				stringBuilder.AppendLine(string.Format("AyncQueueProbeNewRequest: Total New Requests Added ={0}", num));
				base.Result.SampleValue = (double)stopwatch.ElapsedMilliseconds;
				base.Result.ExecutionContext = stringBuilder.ToString();
				base.Result.FailureContext = stringBuilder2.ToString();
				WTFDiagnostics.TraceInformation(ExTraceGlobals.AsyncEngineTracer, base.TraceContext, "AyncQueueProbeNewRequest: DoWork end", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\AsyncQueue\\Probes\\AsyncQueueProbeNewRequest.cs", 71);
			}
		}
	}
}
