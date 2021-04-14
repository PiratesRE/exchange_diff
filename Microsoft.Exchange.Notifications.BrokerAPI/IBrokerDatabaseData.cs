using System;

namespace Microsoft.Exchange.Notifications.Broker
{
	internal interface IBrokerDatabaseData
	{
		string Name { get; }

		Guid DatabaseGuid { get; }
	}
}
