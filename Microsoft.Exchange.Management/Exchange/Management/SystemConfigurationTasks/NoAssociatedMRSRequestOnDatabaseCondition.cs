using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class NoAssociatedMRSRequestOnDatabaseCondition : DatabaseCondition
	{
		public NoAssociatedMRSRequestOnDatabaseCondition(Database database) : base(database, null)
		{
		}

		public override bool Verify()
		{
			MRSRequest mrsrequest;
			return this.Verify(out mrsrequest);
		}

		public bool Verify(out MRSRequest matchingMRSObject)
		{
			matchingMRSObject = PartitionDataAggregator.FindFirstMRSRequestLinkedToDatabase((ADObjectId)base.Database.Identity);
			bool flag;
			if (matchingMRSObject != null)
			{
				this.type = matchingMRSObject.RequestType;
				flag = false;
			}
			else
			{
				flag = true;
			}
			TaskLogger.Trace("NoAssociatedMRSRequestOnDatabaseCondition.Verify(Database '{0}') returns {1}.", new object[]
			{
				base.Database.Identity,
				flag
			});
			return flag;
		}

		internal MRSRequestType Type
		{
			get
			{
				return this.type;
			}
		}

		private MRSRequestType type;
	}
}
