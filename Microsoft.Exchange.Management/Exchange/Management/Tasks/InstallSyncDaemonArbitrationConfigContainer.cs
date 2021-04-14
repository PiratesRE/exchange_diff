using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("install", "SyncDaemonArbitrationConfigContainer")]
	public sealed class InstallSyncDaemonArbitrationConfigContainer : InstallContainerTaskBase<SyncDaemonArbitrationConfig>
	{
		public new string[] Name
		{
			get
			{
				return InstallSyncDaemonArbitrationConfigContainer.name;
			}
		}

		public InstallSyncDaemonArbitrationConfigContainer()
		{
			base.Name = (string[])InstallSyncDaemonArbitrationConfigContainer.name.Clone();
		}

		public static readonly string GlobalSettingsContainerName = "Global Settings";

		public static readonly string SyncDaemonArbitrationConfigContainerName = "SyncDaemonArbitrationConfig";

		private static readonly string[] name = new string[]
		{
			InstallSyncDaemonArbitrationConfigContainer.GlobalSettingsContainerName,
			InstallSyncDaemonArbitrationConfigContainer.SyncDaemonArbitrationConfigContainerName
		};
	}
}
