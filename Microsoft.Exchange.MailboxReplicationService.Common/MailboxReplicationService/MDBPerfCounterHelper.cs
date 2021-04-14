using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MDBPerfCounterHelper
	{
		public MDBPerfCounterHelper(string mdbName)
		{
			this.PerfCounter = MailboxReplicationServicePerMdbPerformanceCounters.GetInstance(mdbName);
			this.ReadTransferRate = new PerfCounterWithAverageRate(null, this.PerfCounter.ReadTransferRate, this.PerfCounter.ReadTransferRateBase, 1024, TimeSpan.FromSeconds(1.0));
			this.WriteTransferRate = new PerfCounterWithAverageRate(null, this.PerfCounter.WriteTransferRate, this.PerfCounter.WriteTransferRateBase, 1024, TimeSpan.FromSeconds(1.0));
			this.Completed = new PerfCounterWithAverageRate(this.PerfCounter.MoveRequestsCompleted, this.PerfCounter.MoveRequestsCompletedRate, this.PerfCounter.MoveRequestsCompletedRateBase, 1, TimeSpan.FromHours(1.0));
			this.CompletedWithWarnings = new PerfCounterWithAverageRate(this.PerfCounter.MoveRequestsCompletedWithWarnings, this.PerfCounter.MoveRequestsCompletedWithWarningsRate, this.PerfCounter.MoveRequestsCompletedWithWarningsRateBase, 1, TimeSpan.FromHours(1.0));
			this.Canceled = new PerfCounterWithAverageRate(this.PerfCounter.MoveRequestsCanceled, this.PerfCounter.MoveRequestsCanceledRate, this.PerfCounter.MoveRequestsCanceledRateBase, 1, TimeSpan.FromHours(1.0));
			this.FailTotal = new PerfCounterWithAverageRate(this.PerfCounter.MoveRequestsFailTotal, this.PerfCounter.MoveRequestsFailTotalRate, this.PerfCounter.MoveRequestsFailTotalRateBase, 1, TimeSpan.FromHours(1.0));
			this.FailBadItemLimit = new PerfCounterWithAverageRate(this.PerfCounter.MoveRequestsFailBadItemLimit, this.PerfCounter.MoveRequestsFailBadItemLimitRate, this.PerfCounter.MoveRequestsFailBadItemLimitRateBase, 1, TimeSpan.FromHours(1.0));
			this.FailNetwork = new PerfCounterWithAverageRate(this.PerfCounter.MoveRequestsFailNetwork, this.PerfCounter.MoveRequestsFailNetworkRate, this.PerfCounter.MoveRequestsFailNetworkRateBase, 1, TimeSpan.FromHours(1.0));
			this.FailStallCI = new PerfCounterWithAverageRate(this.PerfCounter.MoveRequestsFailStallCI, this.PerfCounter.MoveRequestsFailStallCIRate, this.PerfCounter.MoveRequestsFailStallCIRateBase, 1, TimeSpan.FromHours(1.0));
			this.FailStallHA = new PerfCounterWithAverageRate(this.PerfCounter.MoveRequestsFailStallHA, this.PerfCounter.MoveRequestsFailStallHARate, this.PerfCounter.MoveRequestsFailStallHARateBase, 1, TimeSpan.FromHours(1.0));
			this.FailMAPI = new PerfCounterWithAverageRate(this.PerfCounter.MoveRequestsFailMAPI, this.PerfCounter.MoveRequestsFailMAPIRate, this.PerfCounter.MoveRequestsFailMAPIRateBase, 1, TimeSpan.FromHours(1.0));
			this.FailOther = new PerfCounterWithAverageRate(this.PerfCounter.MoveRequestsFailOther, this.PerfCounter.MoveRequestsFailOtherRate, this.PerfCounter.MoveRequestsFailOtherRateBase, 1, TimeSpan.FromHours(1.0));
			this.TransientTotal = new PerfCounterWithAverageRate(this.PerfCounter.MoveRequestsTransientTotal, this.PerfCounter.MoveRequestsTransientTotalRate, this.PerfCounter.MoveRequestsTransientTotalRateBase, 1, TimeSpan.FromHours(1.0));
			this.NetworkFailures = new PerfCounterWithAverageRate(this.PerfCounter.MoveRequestsNetworkFailures, this.PerfCounter.MoveRequestsNetworkFailuresRate, this.PerfCounter.MoveRequestsNetworkFailuresRateBase, 1, TimeSpan.FromHours(1.0));
			this.ProxyBackoff = new PerfCounterWithAverageRate(this.PerfCounter.MoveRequestsProxyBackoff, this.PerfCounter.MoveRequestsProxyBackoffRate, this.PerfCounter.MoveRequestsProxyBackoffRateBase, 1, TimeSpan.FromHours(1.0));
			this.StallsTotal = new PerfCounterWithAverageRate(this.PerfCounter.MoveRequestsStallsTotal, this.PerfCounter.MoveRequestsStallsTotalRate, this.PerfCounter.MoveRequestsStallsTotalRateBase, 1, TimeSpan.FromHours(1.0));
			this.StallsHA = new PerfCounterWithAverageRate(this.PerfCounter.MoveRequestsStallsHA, this.PerfCounter.MoveRequestsStallsHARate, this.PerfCounter.MoveRequestsStallsHARateBase, 1, TimeSpan.FromHours(1.0));
			this.StallsCI = new PerfCounterWithAverageRate(this.PerfCounter.MoveRequestsStallsCI, this.PerfCounter.MoveRequestsStallsCIRate, this.PerfCounter.MoveRequestsStallsCIRateBase, 1, TimeSpan.FromHours(1.0));
		}

		public MailboxReplicationServicePerMdbPerformanceCountersInstance PerfCounter { get; private set; }

		public PerfCounterWithAverageRate Completed { get; private set; }

		public PerfCounterWithAverageRate CompletedWithWarnings { get; private set; }

		public PerfCounterWithAverageRate Canceled { get; private set; }

		public PerfCounterWithAverageRate FailTotal { get; private set; }

		public PerfCounterWithAverageRate FailBadItemLimit { get; private set; }

		public PerfCounterWithAverageRate FailNetwork { get; private set; }

		public PerfCounterWithAverageRate FailStallCI { get; private set; }

		public PerfCounterWithAverageRate FailStallHA { get; private set; }

		public PerfCounterWithAverageRate FailMAPI { get; private set; }

		public PerfCounterWithAverageRate FailOther { get; private set; }

		public PerfCounterWithAverageRate TransientTotal { get; private set; }

		public PerfCounterWithAverageRate NetworkFailures { get; private set; }

		public PerfCounterWithAverageRate ProxyBackoff { get; private set; }

		public PerfCounterWithAverageRate StallsTotal { get; private set; }

		public PerfCounterWithAverageRate StallsHA { get; private set; }

		public PerfCounterWithAverageRate StallsCI { get; private set; }

		public PerfCounterWithAverageRate ReadTransferRate { get; private set; }

		public PerfCounterWithAverageRate WriteTransferRate { get; private set; }

		public void RemovePerfCounter()
		{
			MailboxReplicationServicePerMdbPerformanceCounters.RemoveInstance(this.PerfCounter.Name);
		}
	}
}
