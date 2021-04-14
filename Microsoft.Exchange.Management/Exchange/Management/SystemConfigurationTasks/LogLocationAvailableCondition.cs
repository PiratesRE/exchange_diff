using System;
using System.Globalization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class LogLocationAvailableCondition : PathOnServerCondition
	{
		public LogLocationAvailableCondition(string computerName, string pathName, string logPrefix) : base(computerName, pathName)
		{
			this.logPrefix = logPrefix;
		}

		public override bool Verify()
		{
			string dirName = null;
			try
			{
				dirName = LocalLongFullPath.ParseFromPathNameAndFileName(base.PathName, this.logPrefix + ".log").PathName;
			}
			catch (ArgumentException ex)
			{
				TaskLogger.Trace("LogLocationAvailableCondition.Verify() caught exception '{0}': <Server '{1}', PathName '{2}'>", new object[]
				{
					ex.Message,
					base.ComputerName,
					base.PathName
				});
				return false;
			}
			bool flag = true;
			if (WmiWrapper.IsDirectoryExisting(base.ComputerName, dirName))
			{
				flag = false;
			}
			else if (WmiWrapper.IsFileExistingInPath(base.ComputerName, base.PathName, new WmiWrapper.FileFilter(this.LogFileFilter)))
			{
				flag = false;
			}
			else if (WmiWrapper.IsFileExisting(base.ComputerName, base.PathName))
			{
				flag = false;
			}
			TaskLogger.Trace("LogLocationAvailableCondition.Verify() returns {0}: <Server '{1}', PathName '{2}'>", new object[]
			{
				flag,
				base.ComputerName,
				base.PathName
			});
			return flag;
		}

		private bool LogFileFilter(string name, string ext)
		{
			return name.StartsWith(this.logPrefix, true, CultureInfo.InvariantCulture) && 0 == string.Compare(ext, "log", true, CultureInfo.InvariantCulture);
		}

		private const string logFileExtensionWithoutPeriod = "log";

		private readonly string logPrefix;
	}
}
