using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "ActiveSyncDeviceClassContainer", SupportsShouldProcess = true)]
	public sealed class InstallActiveSyncDeviceClassContainer : InstallContainerTaskBase<ActiveSyncDeviceClasses>
	{
		public InstallActiveSyncDeviceClassContainer()
		{
			base.Name = InstallActiveSyncDeviceClassContainer.name;
		}

		public new string[] Name
		{
			get
			{
				return InstallActiveSyncDeviceClassContainer.name;
			}
		}

		protected override ADObjectId GetBaseContainer()
		{
			ADObjectId baseContainer = base.GetBaseContainer();
			return baseContainer.GetChildId("Mobile Mailbox Settings");
		}

		private static readonly string[] name = new string[]
		{
			"ExchangeDeviceClasses"
		};
	}
}
