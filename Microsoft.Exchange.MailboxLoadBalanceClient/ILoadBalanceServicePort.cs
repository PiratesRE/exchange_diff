using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.CapacityData;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalanceClient
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ILoadBalanceServicePort
	{
		BatchCapacityDatum GetBatchCapacityForForest(int numberOfMailboxes);

		BatchCapacityDatum GetBatchCapacityForForest(int numberOfMailboxes, ByteQuantifiedSize expectedBatchSize);

		CapacitySummary GetCapacitySummary(DirectoryIdentity objectIdentity, bool refreshData);

		ADObjectId GetDatabaseForNewConsumerMailbox();
	}
}
