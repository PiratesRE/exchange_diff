using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class PathOnFixedDriveCondition : PathOnServerCondition
	{
		public PathOnFixedDriveCondition(string computerName, string pathName) : base(computerName, pathName)
		{
		}

		public override bool Verify()
		{
			bool flag = WmiWrapper.IsPathOnFixedDrive(base.ComputerName, base.PathName);
			TaskLogger.Trace("PathOnFixedDriveCondition.Verify() returns {0}: <Server '{1}', PathName '{2}'>", new object[]
			{
				flag,
				base.ComputerName,
				base.PathName
			});
			return flag;
		}
	}
}
