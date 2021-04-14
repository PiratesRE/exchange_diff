using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class SystemPathUniqueUnderServerCondition : ServerCondition
	{
		public SystemPathUniqueUnderServerCondition(string systemPath, ADObjectId serverId, IList<StorageGroup> storageGroups) : base(serverId, storageGroups, null, null)
		{
			this.systemPath = systemPath;
		}

		public override bool Verify()
		{
			bool flag = true;
			foreach (StorageGroup storageGroup in base.StorageGroups)
			{
				if (string.Compare((storageGroup.SystemFolderPath != null) ? storageGroup.SystemFolderPath.PathName : string.Empty, this.systemPath, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					flag = false;
					break;
				}
			}
			TaskLogger.Trace("SystemPathUniqueUnderServerCondition.Verify() returns {0}: <Server '{1}', SystemPath '{2}'>", new object[]
			{
				flag,
				base.ServerId.ToString(),
				this.systemPath
			});
			return flag;
		}

		private readonly string systemPath;
	}
}
