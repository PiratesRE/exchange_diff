using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Transport.Probes
{
	internal interface ITracer
	{
		void TraceDebug(string debugInfo);

		void TraceError(string errorInfo);
	}
}
