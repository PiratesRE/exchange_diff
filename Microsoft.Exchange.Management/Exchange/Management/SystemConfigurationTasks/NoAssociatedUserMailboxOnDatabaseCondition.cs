using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class NoAssociatedUserMailboxOnDatabaseCondition : DatabaseCondition
	{
		public NoAssociatedUserMailboxOnDatabaseCondition(Database database) : base(database, null)
		{
		}

		public override bool Verify()
		{
			bool flag = null == PartitionDataAggregator.FindFirstUserLinkedToDatabase((ADObjectId)base.Database.Identity);
			TaskLogger.Trace("NoAssociatedUserMailboxOnDatabaseCondition.Verify(Database '{0}') returns {1}.", new object[]
			{
				base.Database.Identity,
				flag
			});
			return flag;
		}
	}
}
