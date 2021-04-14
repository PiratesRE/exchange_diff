using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Notifications.Broker
{
	internal interface IBrokerMailboxData
	{
		string DisplayName { get; }

		Guid DatabaseGuid { get; }

		Guid MailboxGuid { get; }

		TenantPartitionHint TenantPartitionHint { get; }

		IDictionary<Guid, BrokerSubscription> Subscriptions { get; }

		IBrokerDatabaseData DatabaseData { get; }
	}
}
