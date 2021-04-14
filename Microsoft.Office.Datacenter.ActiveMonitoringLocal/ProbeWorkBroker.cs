using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class ProbeWorkBroker<TDataAccess> : TypedWorkBroker<ProbeDefinition, ProbeWorkItem, ProbeResult, TDataAccess>, IProbeWorkBroker, IWorkBrokerBase where TDataAccess : DataAccess, new()
	{
		public ProbeWorkBroker(WorkItemFactory factory) : base(factory)
		{
			if (Settings.EnableStreamInsightPush)
			{
				this.publisher = new DataInsightsPublisher();
			}
		}

		public IDataAccessQuery<ProbeResult> GetProbeResults(ProbeDefinition definition, DateTime startTime)
		{
			return base.GetResultsQuery(definition, startTime);
		}

		public IDataAccessQuery<ProbeResult> GetProbeResults(string sampleMask, DateTime startTime, DateTime endTime)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			IEnumerable<ProbeResult> query = from r in base.GetResultsQuery<ProbeResult>(sampleMask, startTime)
			where r.ExecutionEndTime <= endTime
			select r;
			return tdataAccess.AsDataAccessQuery<ProbeResult>(query);
		}

		public Task<StatusEntryCollection> GetStatusEntries(string key, CancellationToken cancellationToken, TracingContext traceContext)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			return tdataAccess.GetStatusEntries(key, cancellationToken, traceContext);
		}

		public Task SaveStatusEntries(StatusEntryCollection entries, CancellationToken cancellationToken, TracingContext traceContext)
		{
			return base.SaveStatusEntriesInternal(entries, cancellationToken, traceContext);
		}

		public void PublishResult(ProbeResult result)
		{
			this.PublishResult(result, result.TraceContext);
		}

		internal override void PublishResult(WorkItemResult result, TracingContext traceContext)
		{
			base.PublishResult(result, traceContext);
			if (Settings.EnableStreamInsightPush)
			{
				this.publisher.PublishToInsightsEngine(result as ProbeResult);
			}
		}

		bool IWorkBrokerBase.IsLocal()
		{
			return base.IsLocal();
		}

		TimeSpan IWorkBrokerBase.get_DefaultResultWindow()
		{
			return base.DefaultResultWindow;
		}

		private DataInsightsPublisher publisher;
	}
}
