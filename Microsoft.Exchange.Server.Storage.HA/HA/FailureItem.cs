using System;
using Microsoft.Exchange.Common.HA;

namespace Microsoft.Exchange.Server.Storage.HA
{
	public class FailureItem
	{
		internal static void PublishHaFailure(Guid dbGuid, string dbName, FailureTag failureTag)
		{
			DatabaseFailureItem databaseFailureItem = new DatabaseFailureItem(FailureNameSpace.Store, failureTag, dbGuid)
			{
				ComponentName = "Store",
				InstanceName = dbName
			};
			databaseFailureItem.Publish();
		}
	}
}
