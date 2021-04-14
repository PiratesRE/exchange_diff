using System;
using System.Threading.Tasks;
using Microsoft.Exchange.Net.MonitoringWebClient;
using Microsoft.Exchange.Net.MonitoringWebClient.Ecp;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Eac.Probes
{
	public class EacBackEndPingProbe : EacWebClientProbeBase
	{
		internal override Task ExecuteScenario(IHttpSession session)
		{
			Uri uri = new Uri(base.Definition.Endpoint);
			ITestStep testStep = new EcpPing(uri);
			return testStep.CreateTask(session);
		}
	}
}
