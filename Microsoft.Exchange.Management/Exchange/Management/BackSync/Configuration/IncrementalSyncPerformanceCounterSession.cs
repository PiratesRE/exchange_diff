using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.BackSync.Configuration
{
	internal class IncrementalSyncPerformanceCounterSession : PerformanceCounterSession
	{
		public IncrementalSyncPerformanceCounterSession(bool enablePerformanceCounters) : base(enablePerformanceCounters)
		{
		}

		protected override ExPerformanceCounter RequestTime
		{
			get
			{
				return BackSyncPerfCounters.DeltaSyncTime;
			}
		}

		protected override ExPerformanceCounter RequestCount
		{
			get
			{
				return BackSyncPerfCounters.DeltaSyncCount;
			}
		}

		protected override ExPerformanceCounter TimeSinceLast
		{
			get
			{
				return BackSyncPerfCounters.DeltaSyncTimeSinceLast;
			}
		}

		protected override PerformanceCounterSession.HitRatePerformanceCounters Success
		{
			get
			{
				return new PerformanceCounterSession.HitRatePerformanceCounters(BackSyncPerfCounters.DeltaSyncResultSuccess, BackSyncPerfCounters.DeltaSyncSuccessRate, BackSyncPerfCounters.DeltaSyncSuccessBase);
			}
		}

		protected override PerformanceCounterSession.HitRatePerformanceCounters SystemError
		{
			get
			{
				return new PerformanceCounterSession.HitRatePerformanceCounters(BackSyncPerfCounters.DeltaSyncResultSystemError, BackSyncPerfCounters.DeltaSyncSystemErrorRate, BackSyncPerfCounters.DeltaSyncSystemErrorBase);
			}
		}

		protected override PerformanceCounterSession.HitRatePerformanceCounters UserError
		{
			get
			{
				return new PerformanceCounterSession.HitRatePerformanceCounters(BackSyncPerfCounters.DeltaSyncResultUserError, BackSyncPerfCounters.DeltaSyncUserErrorRate, BackSyncPerfCounters.DeltaSyncUserErrorBase);
			}
		}

		public override void ReportChangeCount(int changeCount)
		{
			if (base.EnablePerformanceCounters)
			{
				BackSyncPerfCounters.DeltaSyncChangeCount.RawValue = (long)changeCount;
			}
		}

		public override void ReportSameCookie(bool sameCookie)
		{
			if (base.EnablePerformanceCounters)
			{
				if (sameCookie)
				{
					BackSyncPerfCounters.DeltaSyncRetryCookieCount.Increment();
					return;
				}
				BackSyncPerfCounters.DeltaSyncRetryCookieCount.RawValue = 0L;
			}
		}
	}
}
