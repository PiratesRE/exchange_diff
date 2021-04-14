using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.BackSync.Configuration
{
	internal class TenantFullSyncPerformanceCounterSession : PerformanceCounterSession
	{
		public TenantFullSyncPerformanceCounterSession(bool enablePerformanceCounters) : base(enablePerformanceCounters)
		{
		}

		protected override ExPerformanceCounter RequestTime
		{
			get
			{
				return BackSyncPerfCounters.TenantFullSyncTime;
			}
		}

		protected override ExPerformanceCounter RequestCount
		{
			get
			{
				return BackSyncPerfCounters.TenantFullSyncCount;
			}
		}

		protected override ExPerformanceCounter TimeSinceLast
		{
			get
			{
				return BackSyncPerfCounters.TenantFullSyncTimeSinceLast;
			}
		}

		protected override PerformanceCounterSession.HitRatePerformanceCounters Success
		{
			get
			{
				return new PerformanceCounterSession.HitRatePerformanceCounters(BackSyncPerfCounters.TenantFullSyncResultSuccess, BackSyncPerfCounters.TenantFullSyncSuccessRate, BackSyncPerfCounters.TenantFullSyncSuccessBase);
			}
		}

		protected override PerformanceCounterSession.HitRatePerformanceCounters SystemError
		{
			get
			{
				return new PerformanceCounterSession.HitRatePerformanceCounters(BackSyncPerfCounters.TenantFullSyncResultSystemError, BackSyncPerfCounters.TenantFullSyncSystemErrorRate, BackSyncPerfCounters.TenantFullSyncSystemErrorBase);
			}
		}

		protected override PerformanceCounterSession.HitRatePerformanceCounters UserError
		{
			get
			{
				return new PerformanceCounterSession.HitRatePerformanceCounters(BackSyncPerfCounters.TenantFullSyncResultUserError, BackSyncPerfCounters.TenantFullSyncUserErrorRate, BackSyncPerfCounters.TenantFullSyncUserErrorBase);
			}
		}
	}
}
