using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseRedundancyTwoCopyAlert : MonitoringAlert
	{
		public DatabaseRedundancyTwoCopyAlert(string databaseName, Guid dbGuid) : base(databaseName, dbGuid)
		{
		}

		protected override TimeSpan DatabaseHealthCheckGreenTransitionSuppression
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckTwoCopyGreenTransitionSuppressionInSec);
			}
		}

		protected override TimeSpan DatabaseHealthCheckGreenPeriodicInterval
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckTwoCopyGreenPeriodicIntervalInSec);
			}
		}

		protected override TimeSpan DatabaseHealthCheckRedTransitionSuppression
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckTwoCopyRedTransitionSuppressionInSec);
			}
		}

		protected override TimeSpan DatabaseHealthCheckRedPeriodicInterval
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckTwoCopyRedPeriodicIntervalInSec);
			}
		}

		protected override bool IsValidationSuccessful(IHealthValidationResultMinimal serverValidationResult)
		{
			return serverValidationResult.HealthyCopiesCount > 2;
		}

		protected override void RaiseGreenEvent(IHealthValidationResultMinimal result)
		{
			ReplayCrimsonEvents.DatabaseLevelRedundancyTwoCopyCheckPassed.Log<string, int, string, Guid>(base.Identity, result.HealthyCopiesCount, EventUtil.TruncateStringInput(result.ErrorMessage, 32766), base.IdentityGuid);
		}

		protected override void RaiseRedEvent(IHealthValidationResultMinimal result)
		{
			ReplayCrimsonEvents.DatabaseLevelRedundancyTwoCopyCheckFailed.Log<string, int, string, Guid>(base.Identity, result.HealthyCopiesCount, EventUtil.TruncateStringInput(result.ErrorMessage, 32766), base.IdentityGuid);
		}
	}
}
