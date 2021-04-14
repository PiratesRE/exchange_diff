using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class MailboxRulesPerformanceTracker : DisposeTrackableBase
	{
		public MailboxRulesPerformanceTracker(Stopwatch stopwatch)
		{
			PerformanceContext.Current.TakeSnapshot(true);
			this.initialLdap = PerformanceContext.Current.RequestCount;
			RpcDataProvider.Instance.TakeSnapshot(true);
			this.initialMapi = RpcDataProvider.Instance.RequestCount;
			this.stopwatch = stopwatch;
			this.stopwatch.Start();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxRulesPerformanceTracker>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.stopwatch.Stop();
				PerformanceContext.Current.TakeSnapshot(false);
				uint num = PerformanceContext.Current.RequestCount - this.initialLdap;
				MSExchangeStoreDriver.MailboxRulesActiveDirectoryQueries.IncrementBy((long)num);
				RpcDataProvider.Instance.TakeSnapshot(false);
				uint num2 = RpcDataProvider.Instance.RequestCount - this.initialMapi;
				MSExchangeStoreDriver.MailboxRulesMapiOperations.IncrementBy((long)num2);
			}
		}

		private readonly Stopwatch stopwatch;

		private readonly uint initialLdap;

		private readonly uint initialMapi;
	}
}
