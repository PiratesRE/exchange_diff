using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class StorageGroupNumberLessThanMaxLimitCondition : ServerCondition
	{
		public StorageGroupNumberLessThanMaxLimitCondition(bool isForRecovery, ADObjectId serverId, IList<StorageGroup> storageGroups, bool isEnterprise) : base(serverId, storageGroups, null, null)
		{
			this.isForRecovery = isForRecovery;
			this.isEnterprise = isEnterprise;
		}

		public override bool Verify()
		{
			int num = this.isEnterprise ? 100 : 5;
			bool flag = false;
			foreach (StorageGroup storageGroup in base.StorageGroups)
			{
				if (storageGroup.Recovery)
				{
					flag = true;
				}
			}
			bool flag2;
			if (this.isForRecovery)
			{
				flag2 = !flag;
			}
			else
			{
				flag2 = (base.StorageGroups.Count < (flag ? (num + 1) : num));
			}
			TaskLogger.Trace("StorageGroupNumberLessThanMaxLimitCondition.Verify() returns {0}: <Server '{1}'>", new object[]
			{
				flag2,
				base.ServerId.ToString()
			});
			return flag2;
		}

		internal const int MaxStorageGroupNumberForEnterpriseEdition = 100;

		internal const int MaxStorageGroupNumberForStandardOrCoexistenceEdition = 5;

		private readonly bool isForRecovery;

		private readonly bool isEnterprise;
	}
}
