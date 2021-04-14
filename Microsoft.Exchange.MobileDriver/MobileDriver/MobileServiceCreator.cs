using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.TextMessaging.MobileDriver.Resources;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal static class MobileServiceCreator
	{
		internal static IMobileService Create(ExchangePrincipal principal, DeliveryPoint dp)
		{
			if (principal == null)
			{
				throw new ArgumentNullException("principal");
			}
			switch (dp.Type)
			{
			case DeliveryPointType.ExchangeActiveSync:
				return MobileServiceCreator.Create(new EasSelector(principal, dp));
			case DeliveryPointType.SmtpToSmsGateway:
				return MobileServiceCreator.Create(new SmtpToSmsGatewaySelector(principal, dp));
			default:
				throw new ArgumentOutOfRangeException("dp");
			}
		}

		public static IMobileService Create(IMobileServiceSelector selector)
		{
			if (selector == null)
			{
				throw new ArgumentNullException("selector");
			}
			switch (selector.Type)
			{
			case MobileServiceType.Eas:
				return new Eas((EasSelector)selector);
			case MobileServiceType.SmtpToSmsGateway:
				return new SmtpToSmsGateway((SmtpToSmsGatewaySelector)selector);
			default:
				throw new MobileDriverDataException(Strings.ErrorServiceUnsupported(selector.Type.ToString()));
			}
		}
	}
}
