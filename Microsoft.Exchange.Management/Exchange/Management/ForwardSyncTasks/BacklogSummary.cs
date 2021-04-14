using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Description;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public class BacklogSummary
	{
		public SizeAndCountStatistics LinkCount { get; set; }

		public SizeAndCountStatistics ObjectCount { get; set; }

		public ResponseTimeStatistics ResponseTime { get; set; }

		public ServiceEndpoint MsoEndPoint { get; set; }

		public IEnumerable<BacklogEstimateResults> Batches { get; set; }

		public int TotalTenants
		{
			get
			{
				return this.Batches.Sum((BacklogEstimateResults batch) => batch.ContextBacklogs.Count<ContextBacklogMeasurement>());
			}
		}

		public IEnumerable<string> TopTenTenants
		{
			get
			{
				if (this.topTenTenants == null)
				{
					this.topTenTenants = from t in this.Batches.SelectMany((BacklogEstimateResults batch) => batch.ContextBacklogs).OrderByDescending((ContextBacklogMeasurement t) => t, BacklogSummary.BacklogComparer).Take(10)
					select string.Format("{0} O:{1} L:{2}", t.ContextId, t.Objects, t.Links);
				}
				return this.topTenTenants;
			}
		}

		public BacklogSummary()
		{
			this.Batches = new List<BacklogEstimateResults>();
		}

		public virtual void CalculateStats()
		{
			if (BacklogSummary.<CalculateStats>o__SiteContainer8.<>p__Site9 == null)
			{
				BacklogSummary.<CalculateStats>o__SiteContainer8.<>p__Site9 = CallSite<Func<CallSite, object, ResponseTimeStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(ResponseTimeStatistics), typeof(BacklogSummary)));
			}
			this.ResponseTime = BacklogSummary.<CalculateStats>o__SiteContainer8.<>p__Site9.Target(BacklogSummary.<CalculateStats>o__SiteContainer8.<>p__Site9, ResponseTimeStatistics.Calculate(from r in this.Batches
			select r.ResponseTime));
			if (BacklogSummary.<CalculateStats>o__SiteContainer8.<>p__Sitea == null)
			{
				BacklogSummary.<CalculateStats>o__SiteContainer8.<>p__Sitea = CallSite<Func<CallSite, object, SizeAndCountStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(SizeAndCountStatistics), typeof(BacklogSummary)));
			}
			this.ObjectCount = BacklogSummary.<CalculateStats>o__SiteContainer8.<>p__Sitea.Target(BacklogSummary.<CalculateStats>o__SiteContainer8.<>p__Sitea, SizeAndCountStatistics.Calculate(this.Batches.SelectMany((BacklogEstimateResults batch) => from b in batch.ContextBacklogs
			select (int)b.Objects)));
			if (BacklogSummary.<CalculateStats>o__SiteContainer8.<>p__Siteb == null)
			{
				BacklogSummary.<CalculateStats>o__SiteContainer8.<>p__Siteb = CallSite<Func<CallSite, object, SizeAndCountStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(SizeAndCountStatistics), typeof(BacklogSummary)));
			}
			this.LinkCount = BacklogSummary.<CalculateStats>o__SiteContainer8.<>p__Siteb.Target(BacklogSummary.<CalculateStats>o__SiteContainer8.<>p__Siteb, SizeAndCountStatistics.Calculate(this.Batches.SelectMany((BacklogEstimateResults batch) => from b in batch.ContextBacklogs
			select (int)b.Links)));
		}

		private static BacklogSummary.ContextBacklogMeasurementComparer BacklogComparer = new BacklogSummary.ContextBacklogMeasurementComparer();

		private IEnumerable<string> topTenTenants;

		public class ContextBacklogMeasurementComparer : IComparer<ContextBacklogMeasurement>
		{
			public int Compare(ContextBacklogMeasurement x, ContextBacklogMeasurement y)
			{
				uint num = x.Objects + x.Links - y.Objects - y.Links;
				if (num != 0U)
				{
					return (int)num;
				}
				return (int)(x.Objects - y.Objects);
			}
		}

		[CompilerGenerated]
		private static class <CalculateStats>o__SiteContainer8
		{
			public static CallSite<Func<CallSite, object, ResponseTimeStatistics>> <>p__Site9;

			public static CallSite<Func<CallSite, object, SizeAndCountStatistics>> <>p__Sitea;

			public static CallSite<Func<CallSite, object, SizeAndCountStatistics>> <>p__Siteb;
		}
	}
}
