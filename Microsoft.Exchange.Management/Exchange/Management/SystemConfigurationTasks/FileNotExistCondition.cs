using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class FileNotExistCondition : PathOnServerCondition
	{
		public FileNotExistCondition(string computerName, string fileName) : base(computerName, fileName)
		{
		}

		public override bool Verify()
		{
			bool flag = !WmiWrapper.IsFileExisting(base.ComputerName, base.PathName);
			TaskLogger.Trace("FileNotExistCondition.Verify() returns {0}: <Server '{1}', FileName '{2}'>", new object[]
			{
				flag,
				base.ComputerName,
				base.PathName
			});
			return flag;
		}
	}
}
