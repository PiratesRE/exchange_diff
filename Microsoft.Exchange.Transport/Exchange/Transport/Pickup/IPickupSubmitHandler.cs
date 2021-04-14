using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.Pickup
{
	internal interface IPickupSubmitHandler
	{
		void OnSubmit(TransportMailItem item, MailDirectionality direction, PickupType pickupType);
	}
}
