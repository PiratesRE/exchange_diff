using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Transport.Probes
{
	internal interface IResultsChecker
	{
		bool LastSendMailFailed(CancellationToken cancellationToken, int deploymentId, long previousSequenceNumber, int numofMinutesToLookBack, string resultName, int workItemId, TracingContext traceContext);

		List<ProbeResult> GetPreviousResults(CancellationToken cancellationToken, int deploymentId, string previousRunResultName, int numofMinutesToLookBack, TracingContext traceContext);

		List<ProbeResult> GetPreviousNSpecificStageResults(CancellationToken cancellationToken, int deploymentId, string lamResultNameprefix, int numofMinutesToLookBack, int numOfResultsToReturn, string searchStringInExtensionXml, TracingContext traceContext);

		List<ProbeResult> GetPreviousProbeResults(CancellationToken cancellationToken, int numofMinutesToLookBack, string resultName, TracingContext traceContext);
	}
}
