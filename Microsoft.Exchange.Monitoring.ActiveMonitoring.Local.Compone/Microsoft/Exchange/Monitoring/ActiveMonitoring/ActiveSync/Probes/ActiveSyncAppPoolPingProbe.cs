using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveSync.Probes
{
	public class ActiveSyncAppPoolPingProbe : ActiveSyncProbeBase
	{
		public static ProbeDefinition CreateDefinition(string assemblyPath)
		{
			return new ProbeDefinition
			{
				AssemblyPath = assemblyPath,
				TypeName = typeof(ActiveSyncAppPoolPingProbe).FullName,
				Name = "ActiveSyncSelfTestProbe",
				ServiceName = ExchangeComponent.ActiveSyncProtocol.Name,
				TargetResource = "MSExchangeSyncAppPool",
				RecurrenceIntervalSeconds = 60,
				TimeoutSeconds = 58,
				MaxRetryAttempts = 3,
				Endpoint = "https://localhost:444/Microsoft-Server-ActiveSync/exhealth.check"
			};
		}

		public override void PopulateDefinition<Definition>(Definition definition, Dictionary<string, string> propertyBag)
		{
			ProbeDefinition probeDefinition = definition as ProbeDefinition;
			if (probeDefinition == null)
			{
				throw new ArgumentException("definition must be a ProbeDefinition");
			}
			probeDefinition.Endpoint = "https://localhost:444/Microsoft-Server-ActiveSync/exhealth.check";
			probeDefinition.Attributes["InvokeNowExecution"] = true.ToString();
			if (propertyBag.ContainsKey("Endpoint"))
			{
				probeDefinition.Endpoint = propertyBag["Endpoint"];
			}
		}

		internal override IEnumerable<PropertyInformation> GetSubstitutePropertyInformation()
		{
			return new List<PropertyInformation>();
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.StateAttribute21 = "PDWS;";
			WTFDiagnostics.TraceDebug(ExTraceGlobals.ActiveSyncTracer, base.TraceContext, "Entering ActiveSyncSelfTestProbe DoWork().", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ActiveSync\\ActiveSyncAppPoolPingProbe.cs", 107);
			this.latencyMeasurementStart = DateTime.UtcNow;
			base.TrustAllCerts();
			HttpWebRequest request = ActiveSyncProbeUtil.CreateEmptyGetCommand(base.Definition.Endpoint);
			this.probeTrackingObject = new ActiveSyncProbeStateObject(request, base.Result, ProbeState.Get1);
			this.probeTrackingObject.Result.StateAttribute22 = base.Definition.Endpoint;
			this.isInvokeNowExecution = false;
			if (base.Definition.Attributes.ContainsKey("InvokeNowExecution"))
			{
				bool.TryParse(base.Definition.Attributes["InvokeNowExecution"], out this.isInvokeNowExecution);
			}
			this.probeTrackingObject.TimeoutLimit = DateTime.UtcNow.AddMilliseconds(55000.0);
			base.Result.StateAttribute21 = string.Concat(new object[]
			{
				"MaxTimeout:",
				this.probeTrackingObject.TimeoutLimit,
				";",
				base.Result.StateAttribute21
			});
			ProbeResult result = base.Result;
			object stateAttribute = result.StateAttribute21;
			result.StateAttribute21 = string.Concat(new object[]
			{
				stateAttribute,
				"PDWE:",
				DateTime.UtcNow.TimeOfDay,
				";"
			});
			base.DoWork(cancellationToken);
		}

		protected override void ParseResponseSetNextState(ActiveSyncProbeStateObject probeStateObject)
		{
			ProbeResult result = probeStateObject.Result;
			result.StateAttribute21 += "PSMS;";
			probeStateObject.State = ProbeState.Finish;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ActiveSyncTracer, base.TraceContext, "Parsing Get response.", null, "ParseResponseSetNextState", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ActiveSync\\ActiveSyncAppPoolPingProbe.cs", 150);
			if (probeStateObject.WebResponses[probeStateObject.LastResponseIndex].HttpStatus != 200)
			{
				probeStateObject.State = ProbeState.Failure;
				ProbeResult result2 = probeStateObject.Result;
				object stateAttribute = result2.StateAttribute21;
				result2.StateAttribute21 = string.Concat(new object[]
				{
					stateAttribute,
					"PSME:",
					DateTime.UtcNow.TimeOfDay,
					";"
				});
				return;
			}
			ProbeResult result3 = probeStateObject.Result;
			object stateAttribute2 = result3.StateAttribute21;
			result3.StateAttribute21 = string.Concat(new object[]
			{
				stateAttribute2,
				"PSME:",
				DateTime.UtcNow.TimeOfDay,
				";"
			});
		}

		protected override void HandleSocketError(ActiveSyncProbeStateObject probeStateObject)
		{
			DateTime utcNow = DateTime.UtcNow;
			ProbeResult result = probeStateObject.Result;
			result.StateAttribute21 += "PVIPS;";
			WTFDiagnostics.TraceError(ExTraceGlobals.ActiveSyncTracer, base.TraceContext, "Socket exception considered a failure, should never have SocketException on local machines.", null, "HandleSocketError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ActiveSync\\ActiveSyncAppPoolPingProbe.cs", 174);
			probeStateObject.State = ProbeState.Failure;
			ProbeResult result2 = probeStateObject.Result;
			object stateAttribute = result2.StateAttribute21;
			result2.StateAttribute21 = string.Concat(new object[]
			{
				stateAttribute,
				"PVIPE:",
				DateTime.UtcNow.TimeOfDay,
				";"
			});
		}

		protected const string Endpoint = "https://localhost:444/Microsoft-Server-ActiveSync/exhealth.check";

		private const int Timeout = 55000;
	}
}
