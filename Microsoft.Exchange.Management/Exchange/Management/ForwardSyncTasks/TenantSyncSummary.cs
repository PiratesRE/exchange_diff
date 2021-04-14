using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public class TenantSyncSummary : OverallSyncSummaryBase
	{
		public SizeAndCountStatistics ErrorCount { get; set; }

		public SizeAndCountStatistics ErrorSize { get; set; }

		public ThroughputStatistics ErrorBytesPerSecond { get; set; }

		public ThroughputStatistics ErrorsPerSecond { get; set; }

		public override IEnumerable<IEnumerable<ISyncBatchResults>> Samples { get; set; }

		public TenantSyncSummary()
		{
			this.Samples = new List<IEnumerable<TenantSyncBatchResults>>();
		}

		public override void CalculateStats()
		{
			base.CalculateStats();
			List<IEnumerable<TenantSyncBatchResults>> source = this.Samples as List<IEnumerable<TenantSyncBatchResults>>;
			if (TenantSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site1 == null)
			{
				TenantSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site1 = CallSite<Func<CallSite, object, SizeAndCountStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(SizeAndCountStatistics), typeof(TenantSyncSummary)));
			}
			this.ErrorCount = TenantSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site1.Target(TenantSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site1, SizeAndCountStatistics.Calculate(source.SelectMany((IEnumerable<TenantSyncBatchResults> iter) => from r in iter
			select (r.Stats as TenantSyncBatchStatistics).ErrorCount)));
			if (TenantSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site2 == null)
			{
				TenantSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site2 = CallSite<Func<CallSite, object, SizeAndCountStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(SizeAndCountStatistics), typeof(TenantSyncSummary)));
			}
			this.ErrorSize = TenantSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site2.Target(TenantSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site2, SizeAndCountStatistics.Calculate(source.SelectMany((IEnumerable<TenantSyncBatchResults> iter) => from r in iter
			select (int)(r.Stats as TenantSyncBatchStatistics).ErrorSize.Sum)));
			if (TenantSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site3 == null)
			{
				TenantSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site3 = CallSite<Func<CallSite, object, ThroughputStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(ThroughputStatistics), typeof(TenantSyncSummary)));
			}
			this.ErrorBytesPerSecond = TenantSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site3.Target(TenantSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site3, ThroughputStatistics.Calculate(source.SelectMany((IEnumerable<TenantSyncBatchResults> iter) => from r in iter
			select (r.Stats as TenantSyncBatchStatistics).ErrorBytesPerSecond)));
			if (TenantSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site4 == null)
			{
				TenantSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site4 = CallSite<Func<CallSite, object, ThroughputStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(ThroughputStatistics), typeof(TenantSyncSummary)));
			}
			this.ErrorsPerSecond = TenantSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site4.Target(TenantSyncSummary.<CalculateStats>o__SiteContainer0.<>p__Site4, ThroughputStatistics.Calculate(source.SelectMany((IEnumerable<TenantSyncBatchResults> iter) => from r in iter
			select (r.Stats as TenantSyncBatchStatistics).ErrorsPerSecond)));
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
