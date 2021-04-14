using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class DirectoryNotExistCondition : PathOnServerCondition
	{
		public DirectoryNotExistCondition(string computerName, string directoryName) : base(computerName, directoryName)
		{
		}

		public override bool Verify()
		{
			bool flag = !WmiWrapper.IsDirectoryExisting(base.ComputerName, base.PathName);
			TaskLogger.Trace("DirectoryNotExistCondition.Verify() returns {0}: <Server '{1}', DirectoryName '{2}'>", new object[]
			{
				flag,
				base.ComputerName,
				base.PathName
			});
			return flag;
		}
	}
}
