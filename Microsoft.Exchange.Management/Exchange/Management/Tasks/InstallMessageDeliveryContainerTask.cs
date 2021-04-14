using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("install", "MessageDeliveryContainer")]
	public sealed class InstallMessageDeliveryContainerTask : InstallContainerTaskBase<MessageDeliveryGlobalSettings>
	{
		public new string[] Name
		{
			get
			{
				return InstallMessageDeliveryContainerTask.name;
			}
		}

		public InstallMessageDeliveryContainerTask()
		{
			base.Name = (string[])InstallMessageDeliveryContainerTask.name.Clone();
		}

		public static readonly string GlobalSettingsContainerName = "Global Settings";

		private static readonly string[] name = new string[]
		{
			InstallMessageDeliveryContainerTask.GlobalSettingsContainerName,
			MessageDeliveryGlobalSettings.DefaultName
		};
	}
}
