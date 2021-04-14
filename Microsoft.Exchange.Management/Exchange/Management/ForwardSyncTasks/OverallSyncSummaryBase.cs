using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Description;
using Microsoft.CSharp.RuntimeBinder;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public class OverallSyncSummaryBase
	{
		public SizeAndCountStatistics LinkCount { get; set; }

		public SizeAndCountStatistics LinkSize { get; set; }

		public ThroughputStatistics LinkBytesPerSecond { get; set; }

		public ThroughputStatistics LinksPerSecond { get; set; }

		public SizeAndCountStatistics ObjectCount { get; set; }

		public SizeAndCountStatistics ObjectSize { get; set; }

		public ThroughputStatistics ObjectBytesPerSecond { get; set; }

		public ThroughputStatistics ObjectsPerSecond { get; set; }

		public ResponseTimeStatistics ResponseTime { get; set; }

		public ServiceEndpoint MsoEndPoint { get; set; }

		public virtual IEnumerable<IEnumerable<ISyncBatchResults>> Samples { get; set; }

		public virtual void CalculateStats()
		{
			if (OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site1 == null)
			{
				OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site1 = CallSite<Func<CallSite, object, ResponseTimeStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(ResponseTimeStatistics), typeof(OverallSyncSummaryBase)));
			}
			this.ResponseTime = OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site1.Target(OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site1, ResponseTimeStatistics.Calculate(this.Samples.SelectMany((IEnumerable<ISyncBatchResults> iter) => from r in iter
			select r.Stats.ResponseTime)));
			if (OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site2 == null)
			{
				OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site2 = CallSite<Func<CallSite, object, SizeAndCountStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(SizeAndCountStatistics), typeof(OverallSyncSummaryBase)));
			}
			this.ObjectCount = OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site2.Target(OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site2, SizeAndCountStatistics.Calculate(this.Samples.SelectMany((IEnumerable<ISyncBatchResults> iter) => from r in iter
			select r.Stats.ObjectCount)));
			if (OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site3 == null)
			{
				OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site3 = CallSite<Func<CallSite, object, SizeAndCountStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(SizeAndCountStatistics), typeof(OverallSyncSummaryBase)));
			}
			this.LinkCount = OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site3.Target(OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site3, SizeAndCountStatistics.Calculate(this.Samples.SelectMany((IEnumerable<ISyncBatchResults> iter) => from r in iter
			select r.Stats.LinkCount)));
			if (OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site4 == null)
			{
				OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site4 = CallSite<Func<CallSite, object, SizeAndCountStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(SizeAndCountStatistics), typeof(OverallSyncSummaryBase)));
			}
			this.ObjectSize = OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site4.Target(OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site4, SizeAndCountStatistics.Calculate(this.Samples.SelectMany((IEnumerable<ISyncBatchResults> iter) => from r in iter
			select (int)r.Stats.ObjectSize.Sum)));
			if (OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site5 == null)
			{
				OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site5 = CallSite<Func<CallSite, object, SizeAndCountStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(SizeAndCountStatistics), typeof(OverallSyncSummaryBase)));
			}
			this.LinkSize = OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site5.Target(OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site5, SizeAndCountStatistics.Calculate(this.Samples.SelectMany((IEnumerable<ISyncBatchResults> iter) => from r in iter
			select (int)r.Stats.LinkSize.Sum)));
			if (OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site6 == null)
			{
				OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site6 = CallSite<Func<CallSite, object, ThroughputStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(ThroughputStatistics), typeof(OverallSyncSummaryBase)));
			}
			this.ObjectBytesPerSecond = OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site6.Target(OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site6, ThroughputStatistics.Calculate(this.Samples.SelectMany((IEnumerable<ISyncBatchResults> iter) => from r in iter
			select r.Stats.ObjectBytesPerSecond)));
			if (OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site7 == null)
			{
				OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site7 = CallSite<Func<CallSite, object, ThroughputStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(ThroughputStatistics), typeof(OverallSyncSummaryBase)));
			}
			this.LinkBytesPerSecond = OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site7.Target(OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site7, ThroughputStatistics.Calculate(this.Samples.SelectMany((IEnumerable<ISyncBatchResults> iter) => from r in iter
			select r.Stats.LinkBytesPerSecond)));
			if (OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site8 == null)
			{
				OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site8 = CallSite<Func<CallSite, object, ThroughputStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(ThroughputStatistics), typeof(OverallSyncSummaryBase)));
			}
			this.ObjectsPerSecond = OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site8.Target(OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site8, ThroughputStatistics.Calculate(this.Samples.SelectMany((IEnumerable<ISyncBatchResults> iter) => from r in iter
			select r.Stats.ObjectsPerSecond)));
			if (OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site9 == null)
			{
				OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site9 = CallSite<Func<CallSite, object, ThroughputStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(ThroughputStatistics), typeof(OverallSyncSummaryBase)));
			}
			this.LinksPerSecond = OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site9.Target(OverallSyncSummaryBase.<CalculateStats>o__SiteContainer0.<>p__Site9, ThroughputStatistics.Calculate(this.Samples.SelectMany((IEnumerable<ISyncBatchResults> iter) => from r in iter
			select r.Stats.LinksPerSecond)));
		}

		[CompilerGenerated]
		private static class <CalculateStats>o__SiteContainer0
		{
			public static CallSite<Func<CallSite, object, ResponseTimeStatistics>> <>p__Site1;

			public static CallSite<Func<CallSite, object, SizeAndCountStatistics>> <>p__Site2;

			public static CallSite<Func<CallSite, object, SizeAndCountStatistics>> <>p__Site3;

			public static CallSite<Func<CallSite, object, SizeAndCountStatistics>> <>p__Site4;

			public static CallSite<Func<CallSite, object, SizeAndCountStatistics>> <>p__Site5;

			public static CallSite<Func<CallSite, object, ThroughputStatistics>> <>p__Site6;

			public static CallSite<Func<CallSite, object, ThroughputStatistics>> <>p__Site7;

			public static CallSite<Func<CallSite, object, ThroughputStatistics>> <>p__Site8;

			public static CallSite<Func<CallSite, object, ThroughputStatistics>> <>p__Site9;
		}
	}
}
