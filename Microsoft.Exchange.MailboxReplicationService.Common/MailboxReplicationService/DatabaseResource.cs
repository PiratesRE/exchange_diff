using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class DatabaseResource : WlmResource
	{
		public DatabaseResource(Guid mdbGuid, WorkloadType workloadType) : base(workloadType)
		{
			base.ResourceGuid = mdbGuid;
			base.ConfigContext = new DatabaseSettingsContext(base.ResourceGuid, base.ConfigContext);
		}

		public override string ResourceName
		{
			get
			{
				DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(base.ResourceGuid, null, null, FindServerFlags.AllowMissing);
				if (!string.IsNullOrEmpty(databaseInformation.DatabaseName))
				{
					return databaseInformation.DatabaseName;
				}
				return MrsStrings.MissingDatabaseName2(base.ResourceGuid, databaseInformation.ForestFqdn).ToString();
			}
		}
	}
}
