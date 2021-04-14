using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal interface IDeliveryConfiguration
	{
		IAppConfiguration App { get; }

		DeliveryPoisonHandler PoisonHandler { get; }

		IThrottlingConfig Throttling { get; }

		void Load(IMbxDeliveryListener submitHandler);

		void Unload();

		void ConfigUpdate();
	}
}
