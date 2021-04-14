using System;
using Microsoft.Exchange.Data.Transport.Delivery;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	public class TextMessagingDeliveryAgentManager : DeliveryAgentManager
	{
		public override string SupportedDeliveryProtocol
		{
			get
			{
				return "MOBILE";
			}
		}
	}
}
