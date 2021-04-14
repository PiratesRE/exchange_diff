using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public class ServerTaskInfo : TaskInfo
	{
		public ServerTaskInfoBlock Install
		{
			get
			{
				return base.GetBlock<ServerTaskInfoBlock>(InstallationModes.Install);
			}
			set
			{
				base.SetBlock(InstallationModes.Install, value);
			}
		}

		public ServerTaskInfoBlock BuildToBuildUpgrade
		{
			get
			{
				return base.GetBlock<ServerTaskInfoBlock>(InstallationModes.BuildToBuildUpgrade);
			}
			set
			{
				base.SetBlock(InstallationModes.BuildToBuildUpgrade, value);
			}
		}

		public ServerTaskInfoBlock DisasterRecovery
		{
			get
			{
				return base.GetBlock<ServerTaskInfoBlock>(InstallationModes.DisasterRecovery);
			}
			set
			{
				base.SetBlock(InstallationModes.DisasterRecovery, value);
			}
		}

		public ServerTaskInfoBlock Uninstall
		{
			get
			{
				return base.GetBlock<ServerTaskInfoBlock>(InstallationModes.Uninstall);
			}
			set
			{
				base.SetBlock(InstallationModes.Uninstall, value);
			}
		}
	}
}
