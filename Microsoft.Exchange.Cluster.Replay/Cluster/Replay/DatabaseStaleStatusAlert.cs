using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseStaleStatusAlert : MonitoringAlert
	{
		public DatabaseStaleStatusAlert(string databaseName, Guid dbGuid) : base(databaseName, dbGuid)
		{
		}

		protected override TimeSpan DatabaseHealthCheckGreenTransitionSuppression
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckStaleStatusGreenTransitionSuppressionInSec);
			}
		}

		protected override TimeSpan DatabaseHealthCheckGreenPeriodicInterval
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckStaleStatusGreenPeriodicIntervalInSec);
			}
		}

		protected override TimeSpan DatabaseHealthCheckRedTransitionSuppression
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckStaleStatusRedTransitionSuppressionInSec);
			}
		}

		protected override TimeSpan DatabaseHealthCheckRedPeriodicInterval
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckStaleStatusRedPeriodicIntervalInSec);
			}
		}

		protected override bool IsValidationSuccessful(IHealthValidationResultMinimal result)
		{
			return !result.IsAnyCachedCopyStatusStale;
		}

		protected override void RaiseGreenEvent(IHealthValidationResultMinimal result)
		{
			MonitoringAlert.Tracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "DatabaseStaleStatusAlert: RaiseGreenEvent() called for DB '{0}' ({1})", result.Identity, result.IdentityGuid);
		}

		protected override void RaiseRedEvent(IHealthValidationResultMinimal result)
		{
			MonitoringAlert.Tracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "DatabaseStaleStatusAlert: RaiseRedEvent() called for DB '{0}' ({1})", result.Identity, result.IdentityGuid);
			ReplayCrimsonEvents.DatabaseStaleStatusCheckFailed.Log<Guid, string>(base.IdentityGuid, base.Identity);
		}
	}
}
