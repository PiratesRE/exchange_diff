using System;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.Data.Transport
{
	public abstract class AddressBook
	{
		internal AddressBook()
		{
		}

		internal object RecipientCache
		{
			get
			{
				return this.recipientCache;
			}
			set
			{
				this.recipientCache = value;
			}
		}

		public abstract bool Contains(RoutingAddress smtpAddress);

		public abstract AddressBookEntry Find(RoutingAddress smtpAddress);

		public abstract ReadOnlyCollection<AddressBookEntry> Find(params RoutingAddress[] smtpAddresses);

		public abstract ReadOnlyCollection<AddressBookEntry> Find(EnvelopeRecipientCollection recipients);

		public abstract bool IsMemberOf(RoutingAddress recipientSmtpAddress, RoutingAddress groupSmtpAddress);

		public abstract bool IsSameRecipient(RoutingAddress proxyAddressA, RoutingAddress proxyAddressB);

		public abstract bool IsInternal(RoutingAddress address);

		public abstract bool IsInternal(RoutingDomain domain);

		public abstract bool IsInternalTo(RoutingAddress address, RoutingAddress organizationAddress);

		public abstract bool IsInternalTo(RoutingAddress address, RoutingDomain organizationDomain);

		private object recipientCache;
	}
}
