using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Tasks
{
	public class MonitoringOverrideHelpers
	{
		internal string MonitoringItemIdentity { get; set; }

		internal string HealthSet { get; set; }

		internal string MonitoringItemName { get; set; }

		internal string TargetResource { get; set; }

		internal void ParseAndValidateIdentity(string monitoringItemIdentity, bool global)
		{
			this.MonitoringItemIdentity = monitoringItemIdentity;
			this.HealthSet = null;
			this.MonitoringItemName = null;
			this.TargetResource = null;
			string[] array = MonitoringOverrideHelpers.SplitMonitoringItemIdentity(monitoringItemIdentity, '\\');
			if (array.Length < 2 || (global && array.Length != 2) || array.Length > 3 || string.IsNullOrWhiteSpace(array[0]) || string.IsNullOrWhiteSpace(array[1]))
			{
				throw new InvalidIdentityException(this.MonitoringItemIdentity, global ? "<HealthSet>\\<MonitoringItemName>" : "<HealthSet>\\<MonitoringItemName>\\[TargetResource]");
			}
			this.HealthSet = array[0];
			this.MonitoringItemName = array[1];
			if (array.Length == 3 && !string.IsNullOrWhiteSpace(array[2]))
			{
				this.TargetResource = array[2];
			}
		}

		internal static string[] SplitMonitoringItemIdentity(string fullIdentity, char splitCharacter)
		{
			return fullIdentity.Split(new char[]
			{
				splitCharacter
			});
		}

		internal static void ValidateApplyVersion(Version version)
		{
			if (version.Major == -1 || version.Minor == -1 || version.Build == -1 || version.Revision == -1)
			{
				throw new InvalidVersionException(version.ToString(), "Major.Minor.Build.Revision");
			}
		}

		internal static void ValidateOverrideDuration(EnhancedTimeSpan? duration)
		{
			EnhancedTimeSpan zero = EnhancedTimeSpan.Zero;
			EnhancedTimeSpan t = EnhancedTimeSpan.FromDays(365.0);
			if ((duration != null && duration.Value <= zero) || (duration != null && duration.Value > t))
			{
				throw new InvalidDurationException(zero.ToString(), t.ToString());
			}
		}

		internal const string ParameterServer = "Server";

		internal const string ParameterIdentity = "Identity";

		internal const string ParameterItemType = "ItemType";

		internal const string ParameterPropertyName = "PropertyName";

		internal const string ParameterPropertyValue = "PropertyValue";

		internal const string ParameterDuration = "Duration";

		internal const string ParameterApplyVersion = "ApplyVersion";

		internal const string PropertyValueString = "PropertyValue";

		internal const string ExpirationTimeString = "ExpirationTime";

		internal const string TimeUpdatedString = "TimeUpdated";

		internal const string CreatedByString = "CreatedBy";

		internal const string ApplyVersionString = "ApplyVersion";

		internal const string WatermarkString = "Watermark";

		internal const char BackslashSeparator = '\\';

		internal const char TildaSeparator = '~';

		internal const int MaximumOverrideDuration = 365;
	}
}
