using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class MailboxDatabaseCollectionManagerFactory
	{
		public static IMailboxDatabaseCollectionManager Create()
		{
			if (MailboxDatabaseCollectionManagerFactory.InstanceBuilder != null)
			{
				return MailboxDatabaseCollectionManagerFactory.InstanceBuilder();
			}
			return new MailboxDatabaseCollectionManager();
		}

		public static MailboxDatabaseCollectionManagerFactory.MailboxDatabaseCollectionManagerBuilder InstanceBuilder;

		public delegate IMailboxDatabaseCollectionManager MailboxDatabaseCollectionManagerBuilder();
	}
}
