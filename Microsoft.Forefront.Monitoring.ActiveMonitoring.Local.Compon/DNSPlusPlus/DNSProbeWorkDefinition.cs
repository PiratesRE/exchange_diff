using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	internal class DNSProbeWorkDefinition
	{
		public DNSProbeWorkDefinition(string workContextXml, TracingContext traceContext)
		{
			this.traceContext = traceContext;
			this.LoadWorkContext(workContextXml);
		}

		public DnsConfiguration Configuration { get; private set; }

		public List<DnsProbeOperation> Operations { get; private set; }

		private void LoadWorkContext(string workContextXml)
		{
			if (string.IsNullOrWhiteSpace(workContextXml))
			{
				throw new ArgumentException("Work Context XML is null");
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, this.traceContext, "DNSProbeWorkDefinition: Loading work context", null, "LoadWorkContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsProbeWorkDefinition.cs", 59);
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.LoadXml(workContextXml);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, this.traceContext, "DNSProbeWorkDefinition: Parsing //WorkContext", null, "LoadWorkContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsProbeWorkDefinition.cs", 63);
			XmlElement xmlElement = Utils.CheckXmlElement(safeXmlDocument.SelectSingleNode("//WorkContext"), "//WorkContext");
			XmlElement xmlElement2 = Utils.CheckXmlElement(xmlElement.SelectSingleNode("DnsLocation"), "WorkContext.DnsLocation");
			bool boolean = Utils.GetBoolean(xmlElement2.GetAttribute("IsLocal"), "DnsLocation.IsLocal");
			WTFDiagnostics.TraceInformation<bool>(ExTraceGlobals.DNSTracer, this.traceContext, "DNSProbeWorkDefinition: DnsLocation.IsLocal={0}", boolean, null, "LoadWorkContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsProbeWorkDefinition.cs", 68);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, this.traceContext, "DNSProbeWorkDefinition: Parsing /WorkContext/DnsConfiguration", null, "LoadWorkContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsProbeWorkDefinition.cs", 70);
			XmlElement configNode = xmlElement.SelectSingleNode("DnsConfiguration") as XmlElement;
			this.Configuration = DnsConfiguration.CreateInstance(configNode, this.traceContext, boolean);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, this.traceContext, "DNSProbeWorkDefinition: Parsing /WorkContext/DnsOperations", null, "LoadWorkContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsProbeWorkDefinition.cs", 75);
			XmlElement operationsNode = Utils.CheckXmlElement(xmlElement.SelectSingleNode("DnsOperations"), "DnsOperations");
			this.Operations = DnsProbeOperation.GetOperations(operationsNode, this.Configuration, this.traceContext);
		}

		private TracingContext traceContext;
	}
}
