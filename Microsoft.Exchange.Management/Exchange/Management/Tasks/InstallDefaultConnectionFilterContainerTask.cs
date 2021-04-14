using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("install", "DefaultConnectionFilterContainer")]
	public sealed class InstallDefaultConnectionFilterContainerTask : InstallContainerTaskBase<DefaultConnectionFilterGlobalSettings>
	{
		public new string[] Name
		{
			get
			{
				return InstallDefaultConnectionFilterContainerTask.name;
			}
		}

		public InstallDefaultConnectionFilterContainerTask()
		{
			base.Name = (string[])InstallDefaultConnectionFilterContainerTask.name.Clone();
		}

		private static readonly string[] name = new string[]
		{
			InstallMessageDeliveryContainerTask.GlobalSettingsContainerName,
			MessageDeliveryGlobalSettings.DefaultName,
			DefaultMessageFilterGlobalSettings.DefaultName,
			DefaultConnectionFilterGlobalSettings.DefaultName
		};
	}
}
