using System;

namespace Microsoft.Exchange.Migration
{
	internal interface IAsyncNotificationAdapter
	{
		Guid? CreateNotification(IMigrationDataProvider dataProvider, MigrationJob job);

		void UpdateNotification(IMigrationDataProvider dataProvider, MigrationJob job);

		void RemoveNotification(IMigrationDataProvider dataProvider, MigrationJob job);
	}
}
