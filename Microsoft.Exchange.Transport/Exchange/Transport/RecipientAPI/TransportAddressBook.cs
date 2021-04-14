using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.RecipientAPI
{
	internal abstract class TransportAddressBook : AddressBook
	{
		public abstract ADOperationResult TryGetIsInternal(RoutingAddress address, bool acceptedDomainsOnly, out bool isInternal);
	}
}
