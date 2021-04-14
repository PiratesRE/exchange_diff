using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("install", "DefaultMessageFilterContainer")]
	public sealed class InstallDefaultMessageFilterContainerTask : InstallContainerTaskBase<DefaultMessageFilterGlobalSettings>
	{
		public new string[] Name
		{
			get
			{
				return InstallDefaultMessageFilterContainerTask.name;
			}
		}

		public InstallDefaultMessageFilterContainerTask()
		{
			base.Name = (string[])InstallDefaultMessageFilterContainerTask.name.Clone();
		}

		private static readonly string[] name = new string[]
		{
			InstallMessageDeliveryContainerTask.GlobalSettingsContainerName,
			MessageDeliveryGlobalSettings.DefaultName,
			DefaultMessageFilterGlobalSettings.DefaultName
		};
	}
}
