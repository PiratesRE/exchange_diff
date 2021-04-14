using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class TestActiveMonitoringProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition(string probeName)
		{
			return new ProbeDefinition
			{
				AssemblyPath = TestActiveMonitoringProbe.AssemblyPath,
				TypeName = TestActiveMonitoringProbe.TypeName,
				ServiceName = ExchangeComponent.Monitoring.Name,
				RecurrenceIntervalSeconds = 300,
				TimeoutSeconds = 30,
				MaxRetryAttempts = 0,
				Enabled = true,
				Name = probeName
			};
		}

		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition definition, Dictionary<string, string> propertyBag)
		{
			definition.TargetResource = propertyBag["TargetResource"];
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.HeartbeatTracer, base.TraceContext, "TestActiveMonitoringProbe.DoWork: TestActiveMonitoring Probe is logging a message", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Probes\\TestActiveMonitoringProbe.cs", 79);
			if (!Settings.IsCortex)
			{
				throw new Exception("Failed by design.");
			}
			WebClient webClient = new WebClient();
			webClient.DownloadData("http://ipv6.google.com");
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(TestActiveMonitoringProbe).FullName;
	}
}
