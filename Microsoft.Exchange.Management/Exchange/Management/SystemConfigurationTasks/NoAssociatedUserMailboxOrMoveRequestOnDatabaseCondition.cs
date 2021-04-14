using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class NoAssociatedUserMailboxOrMoveRequestOnDatabaseCondition : DatabaseCondition
	{
		public NoAssociatedUserMailboxOrMoveRequestOnDatabaseCondition(Database database) : base(database, null)
		{
		}

		public override bool Verify()
		{
			ADUser aduser;
			return this.Verify(out aduser);
		}

		public bool Verify(out ADUser matchingObject)
		{
			matchingObject = PartitionDataAggregator.FindFirstUserOrMoveRequestLinkedToDatabase((ADObjectId)base.Database.Identity);
			bool flag = null == matchingObject;
			TaskLogger.Trace("NoAssociatedUserMailboxOnDatabaseCondition.Verify(Database '{0}') returns {1}.", new object[]
			{
				base.Database.Identity,
				flag
			});
			return flag;
		}
	}
}
