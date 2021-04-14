using System;
using Microsoft.Exchange.Net.MonitoringWebClient;
using Microsoft.Exchange.Net.MonitoringWebClient.Owa;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Owa.Probes
{
	public abstract class OwaLogonProbe : OwaBaseProbe
	{
		internal static ITestStep CreateScenario(ProbeDefinition probeDefinition, Uri targetUri)
		{
			string userDomain = probeDefinition.Account.Substring(probeDefinition.Account.IndexOf('@') + 1);
			OwaLoginParameters owaLoginParameters = new OwaLoginParameters();
			owaLoginParameters.ShouldDownloadStaticFile = bool.Parse(probeDefinition.Attributes.GetValueOrDefault("DownloadStaticFile", "true"));
			owaLoginParameters.ShouldDownloadStaticFileOnLogonPage = bool.Parse(probeDefinition.Attributes.GetValueOrDefault("DownloadStaticFileOnLogon", "false"));
			owaLoginParameters.ShouldMeasureClientLatency = bool.Parse(probeDefinition.Attributes.GetValueOrDefault("MeasureClientLatency", "true"));
			ITestFactory testFactory = new TestFactory();
			return testFactory.CreateOwaLoginScenario(targetUri, probeDefinition.Account, userDomain, (probeDefinition.AccountPassword != null) ? probeDefinition.AccountPassword.ConvertToSecureString() : null, owaLoginParameters, testFactory);
		}

		internal override ITestStep CreateScenario(Uri targetUri)
		{
			return OwaLogonProbe.CreateScenario(base.Definition, targetUri);
		}

		public const string DownloadStaticFileParameterName = "DownloadStaticFile";

		public const string DownloadStaticFileOnLogonParameterName = "DownloadStaticFileOnLogon";

		public const string MeasureClientLatencyParameterName = "MeasureClientLatency";
	}
}
