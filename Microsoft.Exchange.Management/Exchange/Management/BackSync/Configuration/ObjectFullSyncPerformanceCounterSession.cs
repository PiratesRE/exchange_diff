using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.BackSync.Configuration
{
	internal class ObjectFullSyncPerformanceCounterSession : PerformanceCounterSession
	{
		public ObjectFullSyncPerformanceCounterSession(bool enablePerformanceCounters) : base(enablePerformanceCounters)
		{
		}

		protected override ExPerformanceCounter RequestTime
		{
			get
			{
				return BackSyncPerfCounters.ObjectFullSyncTime;
			}
		}

		protected override ExPerformanceCounter RequestCount
		{
			get
			{
				return BackSyncPerfCounters.ObjectFullSyncCount;
			}
		}

		protected override ExPerformanceCounter TimeSinceLast
		{
			get
			{
				return BackSyncPerfCounters.ObjectFullSyncTimeSinceLast;
			}
		}

		protected override PerformanceCounterSession.HitRatePerformanceCounters Success
		{
			get
			{
				return new PerformanceCounterSession.HitRatePerformanceCounters(BackSyncPerfCounters.ObjectFullSyncResultSuccess, BackSyncPerfCounters.ObjectFullSyncSuccessRate, BackSyncPerfCounters.ObjectFullSyncSuccessBase);
			}
		}

		protected override PerformanceCounterSession.HitRatePerformanceCounters SystemError
		{
			get
			{
				return new PerformanceCounterSession.HitRatePerformanceCounters(BackSyncPerfCounters.ObjectFullSyncResultSystemError, BackSyncPerfCounters.ObjectFullSyncSystemErrorRate, BackSyncPerfCounters.ObjectFullSyncSystemErrorBase);
			}
		}

		protected override PerformanceCounterSession.HitRatePerformanceCounters UserError
		{
			get
			{
				return new PerformanceCounterSession.HitRatePerformanceCounters(BackSyncPerfCounters.ObjectFullSyncResultUserError, BackSyncPerfCounters.ObjectFullSyncUserErrorRate, BackSyncPerfCounters.ObjectFullSyncUserErrorBase);
			}
		}
	}
}
