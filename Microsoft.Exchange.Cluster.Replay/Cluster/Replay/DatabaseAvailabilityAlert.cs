using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseAvailabilityAlert : MonitoringAlert
	{
		public DatabaseAvailabilityAlert(string databaseName, Guid dbGuid) : base(databaseName, dbGuid)
		{
		}

		protected override void RaiseGreenEvent(IHealthValidationResultMinimal result)
		{
			ReplayCrimsonEvents.DatabaseLevelAvailabilityCheckPassed.Log<string, int, string, Guid>(base.Identity, result.HealthyCopiesCount, EventUtil.TruncateStringInput(result.ErrorMessage, 32766), base.IdentityGuid);
		}

		protected override void RaiseRedEvent(IHealthValidationResultMinimal result)
		{
			ReplayEventLogConstants.Tuple_MonitoringDatabaseAvailabilityCheckFailed.LogEvent(null, new object[]
			{
				base.Identity,
				result.HealthyCopiesCount,
				EventUtil.TruncateStringInput(result.ErrorMessage, 32766)
			});
			ReplayCrimsonEvents.DatabaseLevelAvailabilityCheckFailed.Log<string, int, string, Guid>(base.Identity, result.HealthyCopiesCount, EventUtil.TruncateStringInput(result.ErrorMessage, 32766), base.IdentityGuid);
		}
	}
}
