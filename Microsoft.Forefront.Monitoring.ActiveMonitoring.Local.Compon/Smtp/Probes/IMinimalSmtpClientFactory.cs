using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public interface IMinimalSmtpClientFactory
	{
		IMinimalSmtpClient CreateSmtpClient(string host, SmtpProbeWorkDefinition workDefinition, DelTraceDebug traceDebug);
	}
}
