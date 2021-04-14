using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.AirSync
{
	internal class DeviceAccessRuleData
	{
		public DeviceAccessRuleData(ActiveSyncDeviceAccessRule deviceAccessRule)
		{
			this.Identity = deviceAccessRule.OriginalId;
			this.QueryString = deviceAccessRule.QueryString;
			this.Characteristic = deviceAccessRule.Characteristic;
			this.AccessLevel = deviceAccessRule.AccessLevel;
		}

		public ADObjectId Identity { get; private set; }

		public string QueryString { get; private set; }

		public DeviceAccessCharacteristic Characteristic { get; private set; }

		public DeviceAccessLevel AccessLevel { get; private set; }

		public DeviceAccessState AccessState
		{
			get
			{
				switch (this.AccessLevel)
				{
				case DeviceAccessLevel.Allow:
					return DeviceAccessState.Allowed;
				case DeviceAccessLevel.Block:
					return DeviceAccessState.Blocked;
				case DeviceAccessLevel.Quarantine:
					return DeviceAccessState.Quarantined;
				default:
					return DeviceAccessState.Unknown;
				}
			}
		}

		public override string ToString()
		{
			return string.Format("Identity: {0}, Characteristic: {1}, QueryString: {2}, AccessLevel: {3}", new object[]
			{
				this.Identity,
				this.Characteristic,
				this.QueryString,
				this.AccessLevel
			});
		}
	}
}
