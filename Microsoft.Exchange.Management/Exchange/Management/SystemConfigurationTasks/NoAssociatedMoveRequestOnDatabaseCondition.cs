using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class NoAssociatedMoveRequestOnDatabaseCondition : DatabaseCondition
	{
		public NoAssociatedMoveRequestOnDatabaseCondition(Database database) : base(database, null)
		{
		}

		public override bool Verify()
		{
			bool flag = null == PartitionDataAggregator.FindFirstMoveRequestLinkedToDatabase((ADObjectId)base.Database.Identity);
			TaskLogger.Trace("NoAssociatedMoveRequestOnDatabaseCondition.Verify(Database '{0}') returns {1}.", new object[]
			{
				base.Database.Identity,
				flag
			});
			return flag;
		}
	}
}
