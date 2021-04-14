using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class DefaultProbeHelper : ProbeDefinitionHelper
	{
		internal override List<ProbeDefinition> CreateDefinition()
		{
			if (base.DiscoveryContext is PerfCounter)
			{
				string message = "Perf Counter should not have probe definition";
				WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, base.TraceContext, message, null, "CreateDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DefaultProbeHelper.cs", 41);
				throw new XmlException(message);
			}
			if (base.DiscoveryContext is NTEvent && !base.TypeName.EndsWith("GenericEventLogProbe"))
			{
				string message2 = "NTEvent can only use the GenericEventLogProbe probe";
				WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, base.TraceContext, message2, null, "CreateDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DefaultProbeHelper.cs", 49);
				throw new XmlException(message2);
			}
			ProbeDefinition probeDefinition = base.CreateProbeDefinition();
			return new List<ProbeDefinition>(new ProbeDefinition[]
			{
				probeDefinition
			});
		}
	}
}
