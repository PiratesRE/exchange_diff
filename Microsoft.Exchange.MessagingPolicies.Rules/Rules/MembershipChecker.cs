using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class MembershipChecker : IStringComparer
	{
		public MembershipChecker(AddressBook addressBook)
		{
			this.addressBook = addressBook;
		}

		public bool Equals(string recipientAddress, string groupAddress)
		{
			return this.addressBook != null && recipientAddress != null && groupAddress != null && this.addressBook.IsMemberOf((RoutingAddress)recipientAddress, (RoutingAddress)groupAddress);
		}

		private AddressBook addressBook;
	}
}
