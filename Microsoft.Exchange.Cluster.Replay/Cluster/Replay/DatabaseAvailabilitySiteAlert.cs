using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseAvailabilitySiteAlert : MonitoringSiteAlert
	{
		public DatabaseAvailabilitySiteAlert(string databaseName, Guid dbGuid) : base(databaseName, dbGuid)
		{
		}

		protected override void RaiseGreenEvent(IHealthValidationResultMinimal result)
		{
			ReplayCrimsonEvents.DatabaseLevelSiteAvailabilityCheckPassed.Log<string, int, string, Guid>(base.Identity, result.HealthyCopiesCount, EventUtil.TruncateStringInput(result.ErrorMessage, 32766), base.IdentityGuid);
			new EventNotificationItem("msexchangerepl", "DatabaseCopyAvailability", "SingleAvailableCopyCheckSucceeded", EventUtil.TruncateStringInput(result.ErrorMessage, 32766), ResultSeverityLevel.Informational)
			{
				StateAttribute1 = EventUtil.TruncateStringInput(result.ErrorMessage, 32766)
			}.Publish(false);
		}

		protected override void RaiseRedEvent(IHealthValidationResultMinimal result)
		{
			ReplayCrimsonEvents.DatabaseLevelSiteAvailabilityCheckFailed.Log<string, int, string, Guid>(base.Identity, result.HealthyCopiesCount, EventUtil.TruncateStringInput(result.ErrorMessage, 32766), base.IdentityGuid);
			new EventNotificationItem("msexchangerepl", "DatabaseCopyAvailability", "SingleAvailableCopyCheckFailed", EventUtil.TruncateStringInput(result.ErrorMessage, 32766), ResultSeverityLevel.Critical)
			{
				StateAttribute1 = EventUtil.TruncateStringInput(result.ErrorMessage, 32766)
			}.Publish(false);
		}
	}
}
