using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class PathOnFixedOrNetworkDriveCondition : PathOnServerCondition
	{
		public PathOnFixedOrNetworkDriveCondition(string computerName, string pathName) : base(computerName, pathName)
		{
		}

		public override bool Verify()
		{
			bool flag = WmiWrapper.IsPathOnFixedOrNetworkDrive(base.ComputerName, base.PathName);
			TaskLogger.Trace("PathOnFixedOrNetworkDriveCondition.Verify() returns {0}: <Server '{1}', PathName '{2}'>", new object[]
			{
				flag,
				base.ComputerName,
				base.PathName
			});
			return flag;
		}
	}
}
