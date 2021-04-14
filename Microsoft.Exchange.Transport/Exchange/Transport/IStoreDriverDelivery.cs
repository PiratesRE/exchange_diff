using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport
{
	internal interface IStoreDriverDelivery : IStartableTransportComponent, ITransportComponent
	{
		SmtpResponse DoLocalDelivery(TransportMailItem mailItem);

		void Retire();
	}
}
