using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public class OrgTaskInfo : TaskInfo
	{
		public OrgTaskInfoBlock Install
		{
			get
			{
				return base.GetBlock<OrgTaskInfoBlock>(InstallationModes.Install);
			}
			set
			{
				base.SetBlock(InstallationModes.Install, value);
			}
		}

		public OrgTaskInfoBlock BuildToBuildUpgrade
		{
			get
			{
				return base.GetBlock<OrgTaskInfoBlock>(InstallationModes.BuildToBuildUpgrade);
			}
			set
			{
				base.SetBlock(InstallationModes.BuildToBuildUpgrade, value);
			}
		}

		public OrgTaskInfoBlock Uninstall
		{
			get
			{
				return base.GetBlock<OrgTaskInfoBlock>(InstallationModes.Uninstall);
			}
			set
			{
				base.SetBlock(InstallationModes.Uninstall, value);
			}
		}
	}
}
