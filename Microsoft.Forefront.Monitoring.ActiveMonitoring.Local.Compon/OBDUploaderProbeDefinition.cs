using System;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	internal class OBDUploaderProbeDefinition
	{
		public OBDUploaderProbeDefinition(string extensionXml, TracingContext traceContext)
		{
			this.traceContext = traceContext;
			this.LoadWorkContext(extensionXml);
		}

		public string ProgressFileFolder { get; set; }

		public string RawLogFileFolder { get; set; }

		public string ProgressFileNamePattern { get; set; }

		public string RawLogFileNamePattern { get; set; }

		public int SLAThresholdInHours { get; set; }

		public int LiveTrafficCheckThresholdInHours { get; set; }

		private void LoadWorkContext(string workContextXml)
		{
			if (string.IsNullOrWhiteSpace(workContextXml))
			{
				throw new ArgumentException("Work Context XML is null");
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DataminingTracer, this.traceContext, "OBDUploaderProbeDefinition: Loading work context", null, "LoadWorkContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Datamining\\Probes\\OBDUploaderProbeDefinition.cs", 80);
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.LoadXml(workContextXml);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DataminingTracer, this.traceContext, "OBDUploaderProbeDefinition: Parsing //WorkContext", null, "LoadWorkContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Datamining\\Probes\\OBDUploaderProbeDefinition.cs", 84);
			XmlElement xmlElement = Utils.CheckXmlElement(safeXmlDocument.SelectSingleNode("//WorkContext"), "//WorkContext");
			XmlElement xmlElement2 = Utils.CheckXmlElement(xmlElement.SelectSingleNode("OBDUploaderConfiguration"), "WorkContext.OBDUploaderConfiguration");
			this.ProgressFileFolder = xmlElement2.GetAttribute("ProgressFileFolder");
			this.RawLogFileFolder = xmlElement2.GetAttribute("RawLogFileFolder");
			this.ProgressFileNamePattern = xmlElement2.GetAttribute("ProgressFileNamePattern");
			this.RawLogFileNamePattern = xmlElement2.GetAttribute("RawLogFileNamePattern");
			this.SLAThresholdInHours = Utils.GetInteger(xmlElement2.GetAttribute("SLAThresholdInHours"), "OBDUploaderConfiguration.SLAThresholdInHours", 6, 1);
			this.LiveTrafficCheckThresholdInHours = Utils.GetInteger(xmlElement2.GetAttribute("LiveTrafficCheckThresholdInHours"), "OBDUploaderConfiguration.LiveTrafficCheckThresholdInHours", 2, 1);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DataminingTracer, this.traceContext, string.Format("OBDUploaderProbeDefinition: the result is {0},{1},{2},{3},{4},{5}", new object[]
			{
				this.ProgressFileFolder,
				this.RawLogFileFolder,
				this.ProgressFileNamePattern,
				this.RawLogFileNamePattern,
				this.SLAThresholdInHours,
				this.LiveTrafficCheckThresholdInHours
			}), null, "LoadWorkContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Datamining\\Probes\\OBDUploaderProbeDefinition.cs", 94);
		}

		private TracingContext traceContext;
	}
}
