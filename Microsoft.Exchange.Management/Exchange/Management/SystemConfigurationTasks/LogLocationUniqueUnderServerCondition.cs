using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class LogLocationUniqueUnderServerCondition : ServerCondition
	{
		public LogLocationUniqueUnderServerCondition(string logLocation, ADObjectId serverId, IList<StorageGroup> storageGroups) : base(serverId, storageGroups, null, null)
		{
			this.logLocation = logLocation;
		}

		public override bool Verify()
		{
			bool flag = true;
			foreach (StorageGroup storageGroup in base.StorageGroups)
			{
				if (string.Compare((storageGroup.LogFolderPath != null) ? storageGroup.LogFolderPath.PathName : string.Empty, this.logLocation, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					flag = false;
					break;
				}
			}
			TaskLogger.Trace("LogLocationUniqueUnderServerCondition.Verify() returns {0}: <Server '{1}', LogLocation '{2}'>", new object[]
			{
				flag,
				base.ServerId.ToString(),
				this.logLocation
			});
			return flag;
		}

		private readonly string logLocation;
	}
}
