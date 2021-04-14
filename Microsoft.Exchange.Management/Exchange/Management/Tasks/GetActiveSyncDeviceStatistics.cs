using System;
using System.Management.Automation;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "ActiveSyncDeviceStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetActiveSyncDeviceStatistics : GetMobileDeviceStatisticsBase<ActiveSyncDeviceIdParameter, ActiveSyncDevice>
	{
		public GetActiveSyncDeviceStatistics()
		{
			this.ActiveSync = true;
			this.OWAforDevices = false;
		}

		private new SwitchParameter ActiveSync
		{
			get
			{
				return base.ActiveSync;
			}
			set
			{
				base.ActiveSync = value;
			}
		}

		private new SwitchParameter OWAforDevices
		{
			get
			{
				return base.OWAforDevices;
			}
			set
			{
				base.OWAforDevices = value;
			}
		}

		protected override MobileDeviceConfiguration CreateDeviceConfiguration(DeviceInfo deviceInfo)
		{
			return new ActiveSyncDeviceConfiguration(deviceInfo);
		}

		protected override MobileDeviceIdParameter CreateIdentityObject()
		{
			return new ActiveSyncDeviceIdParameter(this.DataObject);
		}

		protected override void InternalProcessRecord()
		{
			this.WriteWarning(Strings.WarningCmdletIsDeprecated("Get-ActiveSyncDeviceStatistics", "Get-MobileDeviceStatistics"));
			base.InternalProcessRecord();
		}
	}
}
