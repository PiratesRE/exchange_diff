using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public interface IProbeWorkBroker : IWorkBrokerBase
	{
		IDataAccessQuery<ProbeResult> GetProbeResults(ProbeDefinition definition, DateTime startTime);

		IDataAccessQuery<ProbeResult> GetProbeResults(string sampleMask, DateTime startTime, DateTime endTime);

		Task<StatusEntryCollection> GetStatusEntries(string key, CancellationToken cancellationToken, TracingContext traceContext);

		Task SaveStatusEntries(StatusEntryCollection entries, CancellationToken cancellationToken, TracingContext traceContext);

		void PublishResult(ProbeResult result);

		IDataAccessQuery<TEntity> AsDataAccessQuery<TEntity>(IEnumerable<TEntity> query);
	}
}
