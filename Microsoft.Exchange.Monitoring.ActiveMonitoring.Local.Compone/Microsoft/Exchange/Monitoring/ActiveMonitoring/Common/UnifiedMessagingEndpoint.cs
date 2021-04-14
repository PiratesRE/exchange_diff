using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal abstract class UnifiedMessagingEndpoint : IEndpoint
	{
		public int SipTcpListeningPort { get; protected set; }

		public int SipTlsListeningPort { get; protected set; }

		public string CertificateThumbprint { get; protected set; }

		public UMStartupMode StartupMode { get; protected set; }

		public string CertificateSubjectName { get; protected set; }

		protected TracingContext TraceContext
		{
			get
			{
				return this.traceContext;
			}
		}

		protected ITopologyConfigurationSession TopologyConfigurationSession { get; set; }

		protected Server Server { get; set; }

		public bool RestartOnChange
		{
			get
			{
				return true;
			}
		}

		public Exception Exception { get; set; }

		public abstract bool DetectChange();

		public abstract void Initialize();

		protected abstract void GetUMPropertiesFromAD(Server server, out UMStartupMode startupMode, out int sipTcpListeningPort, out int sipTlsListeningPort, out string certificateThumbprint);

		protected string GetCertificateSubjectNameFromThumbprint(string certificateThumbprint)
		{
			string result;
			if (string.IsNullOrEmpty(certificateThumbprint))
			{
				result = string.Empty;
			}
			else
			{
				result = Utils.GetCertificateSubjectNameFromThumbprint(this.TraceContext, certificateThumbprint);
			}
			return result;
		}

		protected bool HasAnyUMPropertyChanged(Server server)
		{
			UMStartupMode umstartupMode;
			int num;
			int num2;
			string text;
			this.GetUMPropertiesFromAD(server, out umstartupMode, out num, out num2, out text);
			if (this.StartupMode != umstartupMode)
			{
				WTFDiagnostics.TraceFunction(ExTraceGlobals.UnifiedMessagingEndpointTracer, this.TraceContext, string.Format("UMStartupMode changed Initial Value {0} New Value {1}", this.StartupMode, umstartupMode), null, "HasAnyUMPropertyChanged", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\UnifiedMessagingEndpoint.cs", 179);
				return true;
			}
			if (this.SipTcpListeningPort != num)
			{
				WTFDiagnostics.TraceFunction(ExTraceGlobals.UnifiedMessagingEndpointTracer, this.TraceContext, string.Format("SipTCPListeningPort changed Initial Value {0} New Value {1}", this.SipTcpListeningPort, num), null, "HasAnyUMPropertyChanged", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\UnifiedMessagingEndpoint.cs", 185);
				return true;
			}
			if (this.SipTlsListeningPort != num2)
			{
				WTFDiagnostics.TraceFunction(ExTraceGlobals.UnifiedMessagingEndpointTracer, this.TraceContext, string.Format("SipTLSListeningPort changed Initial Value {0} New Value {1}", this.SipTlsListeningPort, num2), null, "HasAnyUMPropertyChanged", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\UnifiedMessagingEndpoint.cs", 191);
				return true;
			}
			if (!string.IsNullOrEmpty(this.CertificateThumbprint) && !string.IsNullOrEmpty(text))
			{
				if (!this.CertificateThumbprint.Equals(text, StringComparison.OrdinalIgnoreCase))
				{
					WTFDiagnostics.TraceFunction(ExTraceGlobals.UnifiedMessagingEndpointTracer, this.TraceContext, string.Format("CertificateThumbprint changed Initial Value {0} New Value {1}", this.CertificateThumbprint, text), null, "HasAnyUMPropertyChanged", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\UnifiedMessagingEndpoint.cs", 199);
					return true;
				}
			}
			else if (!string.IsNullOrEmpty(this.CertificateThumbprint) || !string.IsNullOrEmpty(text))
			{
				WTFDiagnostics.TraceFunction(ExTraceGlobals.UnifiedMessagingEndpointTracer, this.TraceContext, string.Format("CertificateThumbprint changed Initial Value {0} New Value {1}", this.CertificateThumbprint, text), null, "HasAnyUMPropertyChanged", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\UnifiedMessagingEndpoint.cs", 207);
				return true;
			}
			return false;
		}

		private TracingContext traceContext = TracingContext.Default;
	}
}
