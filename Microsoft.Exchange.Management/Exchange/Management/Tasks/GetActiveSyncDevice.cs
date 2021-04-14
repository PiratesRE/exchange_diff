using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "ActiveSyncDevice", DefaultParameterSetName = "Identity")]
	public sealed class GetActiveSyncDevice : GetMobileDeviceBase<ActiveSyncDeviceIdParameter, ActiveSyncDevice>
	{
		public GetActiveSyncDevice()
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

		protected override void WriteResult(IConfigurable dataObject)
		{
			this.WriteWarning(Strings.WarningCmdletIsDeprecated("Get-ActiveSyncDevice", "Get-MobileDevice"));
			MobileDevice mobileDevice = (MobileDevice)dataObject;
			base.WriteResult(new ActiveSyncDevice(mobileDevice));
		}
	}
}
