using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal interface IAppConfiguration
	{
		bool IsFolderPickupEnabled { get; }

		int PoisonRegistryEntryMaxCount { get; }

		void Load();
	}
}
