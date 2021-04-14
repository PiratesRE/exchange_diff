using System;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Serializable]
	public class ActiveSyncDeviceConfiguration : MobileDeviceConfiguration
	{
		public ActiveSyncDeviceConfiguration(DeviceInfo deviceInfo) : base(deviceInfo)
		{
			if (base.ClientType != MobileClientType.EAS)
			{
				throw new ArgumentException("ClientType is not EAS.");
			}
		}

		public string DeviceActiveSyncVersion
		{
			get
			{
				return base.ClientVersion;
			}
			internal set
			{
				base.ClientVersion = value;
			}
		}

		private new string ClientVersion { get; set; }
	}
}
