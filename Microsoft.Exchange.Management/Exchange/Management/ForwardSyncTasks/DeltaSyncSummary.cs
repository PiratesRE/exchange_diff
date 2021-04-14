using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public class DeltaSyncSummary : OverallSyncSummaryBase
	{
		public SizeAndCountStatistics ContextCount { get; set; }

		public SizeAndCountStatistics ContextSize { get; set; }

		public ThroughputStatistics ContextBytesPerSecond { get; set; }

		public ThroughputStatistics ContextsPerSecond { get; set; }

		public override IEnumerable<IEnumerable<ISyncBatchResults>> Samples { get; set; }

		public DeltaSyncSummary()
		{
			this.Samples = new List<IEnumerable<DeltaSyncBatchResults>>();
		}

		public override void CalculateStats()
		{
			base.CalculateStats();
			List<IEnumerable<DeltaSyncBatchResults>> source = this.Samples as List<IEnumerable<DeltaSyncBatchResults>>;
			if (DeltaSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site1 == null)
			{
				DeltaSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site1 = CallSite<Func<CallSite, object, SizeAndCountStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(SizeAndCountStatistics), typeof(DeltaSyncSummary)));
			}
			this.ContextCount = DeltaSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site1.Target(DeltaSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site1, SizeAndCountStatistics.Calculate(source.SelectMany((IEnumerable<DeltaSyncBatchResults> iter) => from r in iter
			select (r.Stats as DeltaSyncBatchStatistics).ContextCount)));
			if (DeltaSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site2 == null)
			{
				DeltaSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site2 = CallSite<Func<CallSite, object, SizeAndCountStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(SizeAndCountStatistics), typeof(DeltaSyncSummary)));
			}
			this.ContextSize = DeltaSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site2.Target(DeltaSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site2, SizeAndCountStatistics.Calculate(source.SelectMany((IEnumerable<DeltaSyncBatchResults> iter) => from r in iter
			select (int)(r.Stats as DeltaSyncBatchStatistics).ContextSize.Sum)));
			if (DeltaSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site3 == null)
			{
				DeltaSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site3 = CallSite<Func<CallSite, object, ThroughputStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(ThroughputStatistics), typeof(DeltaSyncSummary)));
			}
			this.ContextBytesPerSecond = DeltaSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site3.Target(DeltaSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site3, ThroughputStatistics.Calculate(source.SelectMany((IEnumerable<DeltaSyncBatchResults> iter) => from r in iter
			select (r.Stats as DeltaSyncBatchStatistics).ContextBytesPerSecond)));
			if (DeltaSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site4 == null)
			{
				DeltaSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site4 = CallSite<Func<CallSite, object, ThroughputStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(ThroughputStatistics), typeof(DeltaSyncSummary)));
			}
			this.ContextsPerSecond = DeltaSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site4.Target(DeltaSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site4, ThroughputStatistics.Calculate(source.SelectMany((IEnumerable<DeltaSyncBatchResults> iter) => from r in iter
			select (r.Stats as DeltaSyncBatchStatistics).ContextsPerSecond)));
		}

		[CompilerGenerated]
		private static class <CalculateStats>o__SiteContainer0
		{
			public static CallSite<Func<CallSite, object, SizeAndCountStatistics>> <>p__Site1;

			public static CallSite<Func<CallSite, object, SizeAndCountStatistics>> <>p__Site2;

			public static CallSite<Func<CallSite, object, ThroughputStatistics>> <>p__Site3;

			public static CallSite<Func<CallSite, object, ThroughputStatistics>> <>p__Site4;
		}
	}
}
