using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseRedundancyAlert : MonitoringAlert
	{
		public DatabaseRedundancyAlert(string databaseName, Guid dbGuid) : base(databaseName, dbGuid)
		{
		}

		protected override TimeSpan DatabaseHealthCheckGreenTransitionSuppression
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckOneCopyGreenTransitionSuppressionInSec);
			}
		}

		protected override void RaiseGreenEvent(IHealthValidationResultMinimal result)
		{
			ReplayEventLogConstants.Tuple_MonitoringDatabaseRedundancyCheckPassed.LogEvent(null, new object[]
			{
				base.Identity,
				result.HealthyCopiesCount,
				EventUtil.TruncateStringInput(result.ErrorMessage, 32766)
			});
			ReplayCrimsonEvents.DatabaseLevelRedundancyCheckPassed.Log<string, int, string, Guid>(base.Identity, result.HealthyCopiesCount, EventUtil.TruncateStringInput(result.ErrorMessage, 32766), base.IdentityGuid);
		}

		protected override void RaiseRedEvent(IHealthValidationResultMinimal result)
		{
			ReplayEventLogConstants.Tuple_MonitoringDatabaseRedundancyCheckFailed.LogEvent(null, new object[]
			{
				base.Identity,
				result.HealthyCopiesCount,
				EventUtil.TruncateStringInput(result.ErrorMessage, 32766)
			});
			ReplayCrimsonEvents.DatabaseLevelRedundancyCheckFailed.Log<string, int, string, Guid>(base.Identity, result.HealthyCopiesCount, EventUtil.TruncateStringInput(result.ErrorMessage, 32766), base.IdentityGuid);
		}
	}
}
