using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class DeviceAutoBlockThreshold
	{
		public DeviceAutoBlockThreshold(ActiveSyncDeviceAutoblockThreshold threshold)
		{
			this.BehaviorType = threshold.BehaviorType;
			if (threshold.WhenChanged != null)
			{
				this.LastChangeTime = (ExDateTime)threshold.WhenChanged.Value.ToUniversalTime();
			}
			else
			{
				this.LastChangeTime = ExDateTime.UtcNow;
			}
			this.BehaviorTypeIncidenceLimit = threshold.BehaviorTypeIncidenceLimit;
			this.BehaviorTypeIncidenceDuration = threshold.BehaviorTypeIncidenceDuration;
			this.DeviceBlockDuration = threshold.DeviceBlockDuration;
			this.AdminEmailInsert = threshold.AdminEmailInsert;
		}

		public DeviceAutoBlockThreshold(AutoblockThresholdType type)
		{
			this.BehaviorType = type;
			this.LastChangeTime = ExDateTime.UtcNow;
		}

		public AutoblockThresholdType BehaviorType { get; private set; }

		public ExDateTime LastChangeTime { get; set; }

		public int BehaviorTypeIncidenceLimit { get; set; }

		public EnhancedTimeSpan BehaviorTypeIncidenceDuration { get; set; }

		public EnhancedTimeSpan DeviceBlockDuration { get; set; }

		public string AdminEmailInsert { get; set; }

		public const DeviceAccessStateReason FirstAutoBlockReason = DeviceAccessStateReason.UserAgentsChanges;

		public const DeviceAccessStateReason LastAutoBlockReason = DeviceAccessStateReason.CommandFrequency;
	}
}
