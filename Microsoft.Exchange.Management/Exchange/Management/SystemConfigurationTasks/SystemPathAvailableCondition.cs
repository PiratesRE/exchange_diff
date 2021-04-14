using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class SystemPathAvailableCondition : PathOnServerCondition
	{
		public SystemPathAvailableCondition(string computerName, string pathName) : base(computerName, pathName)
		{
		}

		public override bool Verify()
		{
			string text = null;
			try
			{
				text = LocalLongFullPath.ParseFromPathNameAndFileName(base.PathName, "tmp.edb").PathName;
			}
			catch (ArgumentException ex)
			{
				TaskLogger.Trace("SystemPathAvailableCondition.Verify() caught exception '{0}': <Server '{1}', PathName '{2}'>", new object[]
				{
					ex.Message,
					base.ComputerName,
					base.PathName
				});
				return false;
			}
			bool flag = true;
			if (WmiWrapper.IsDirectoryExisting(base.ComputerName, text))
			{
				flag = false;
			}
			else if (WmiWrapper.IsFileExisting(base.ComputerName, text))
			{
				flag = false;
			}
			else if (WmiWrapper.IsFileExisting(base.ComputerName, base.PathName))
			{
				flag = false;
			}
			TaskLogger.Trace("SystemPathAvailableCondition.Verify() returns {0}: <Server '{1}', PathName '{2}'>", new object[]
			{
				flag,
				base.ComputerName,
				base.PathName
			});
			return flag;
		}

		private const string temporarySystemFileName = "tmp.edb";
	}
}
