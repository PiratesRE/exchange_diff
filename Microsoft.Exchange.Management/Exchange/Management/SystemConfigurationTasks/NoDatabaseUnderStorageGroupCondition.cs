using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class NoDatabaseUnderStorageGroupCondition : StorageGroupCondition
	{
		public NoDatabaseUnderStorageGroupCondition(ADObjectId storageGroupId, IConfigurationSession session) : base(storageGroupId, null, session)
		{
		}

		public override bool Verify()
		{
			bool flag = base.Session.Find<Database>(base.StorageGroupId, QueryScope.OneLevel, null, null, 1).Length == 0;
			TaskLogger.Trace("NoDatabaseUnderStorageGroupCondition.Verify() returns {0}: <StorageGroup '{1}'>", new object[]
			{
				flag,
				base.StorageGroupId.ToString()
			});
			return flag;
		}
	}
}
