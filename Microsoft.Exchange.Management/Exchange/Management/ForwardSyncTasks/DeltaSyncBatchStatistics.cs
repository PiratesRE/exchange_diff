using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public class DeltaSyncBatchStatistics : SyncBatchStatisticsBase
	{
		public int ContextCount { get; set; }

		public SizeAndCountStatistics ContextSize { get; set; }

		public double ContextsPerSecond { get; set; }

		public double ContextBytesPerSecond { get; set; }

		public void Calculate(DeltaSyncBatchResults deltaSyncBatch)
		{
			base.Calculate(deltaSyncBatch.Objects, deltaSyncBatch.Links);
			this.ContextCount = deltaSyncBatch.Contexts.Count<DirectoryContext>();
			if (DeltaSyncBatchStatistics.<Calculate>o__SiteContainer0.<>p__Site1 == null)
			{
				DeltaSyncBatchStatistics.<Calculate>o__SiteContainer0.<>p__Site1 = CallSite<Func<CallSite, object, SizeAndCountStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(SizeAndCountStatistics), typeof(DeltaSyncBatchStatistics)));
			}
			this.ContextSize = DeltaSyncBatchStatistics.<Calculate>o__SiteContainer0.<>p__Site1.Target(DeltaSyncBatchStatistics.<Calculate>o__SiteContainer0.<>p__Site1, SizeAndCountStatistics.Calculate(from o in deltaSyncBatch.Contexts
			select SyncBatchStatisticsBase.SerializedSize(o)));
			this.ContextsPerSecond = (double)this.ContextCount / base.ResponseTime.TotalSeconds;
			this.ContextBytesPerSecond = (double)this.ContextSize.Sum / base.ResponseTime.TotalSeconds;
		}

		[CompilerGenerated]
		private static class <Calculate>o__SiteContainer0
		{
			public static CallSite<Func<CallSite, object, SizeAndCountStatistics>> <>p__Site1;
		}
	}
}
