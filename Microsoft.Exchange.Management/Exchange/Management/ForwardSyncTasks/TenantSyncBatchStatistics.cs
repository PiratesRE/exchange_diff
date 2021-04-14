using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public class TenantSyncBatchStatistics : SyncBatchStatisticsBase
	{
		public int ErrorCount { get; set; }

		public SizeAndCountStatistics ErrorSize { get; set; }

		public double ErrorsPerSecond { get; set; }

		public double ErrorBytesPerSecond { get; set; }

		public void Calculate(TenantSyncBatchResults tenantSyncBatch)
		{
			base.Calculate(tenantSyncBatch.Objects, tenantSyncBatch.Links);
			this.ErrorCount = tenantSyncBatch.Errors.Count<DirectoryObjectError>();
			if (TenantSyncBatchStatistics.<Calculate>o__SiteContainer0.<>p__Site1 == null)
			{
				TenantSyncBatchStatistics.<Calculate>o__SiteContainer0.<>p__Site1 = CallSite<Func<CallSite, object, SizeAndCountStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(SizeAndCountStatistics), typeof(TenantSyncBatchStatistics)));
			}
			this.ErrorSize = TenantSyncBatchStatistics.<Calculate>o__SiteContainer0.<>p__Site1.Target(TenantSyncBatchStatistics.<Calculate>o__SiteContainer0.<>p__Site1, SizeAndCountStatistics.Calculate(from o in tenantSyncBatch.Errors
			select SyncBatchStatisticsBase.SerializedSize(o)));
			this.ErrorsPerSecond = (double)this.ErrorCount / base.ResponseTime.TotalSeconds;
			this.ErrorBytesPerSecond = (double)this.ErrorSize.Sum / base.ResponseTime.TotalSeconds;
		}

		[CompilerGenerated]
		private static class <Calculate>o__SiteContainer0
		{
			public static CallSite<Func<CallSite, object, SizeAndCountStatistics>> <>p__Site1;
		}
	}
}
