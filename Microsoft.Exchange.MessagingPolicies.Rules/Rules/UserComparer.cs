using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class UserComparer : IStringComparer
	{
		public UserComparer(AddressBook addressBook)
		{
			this.addressBook = addressBook;
		}

		public bool Equals(string userX, string userY)
		{
			if (this.addressBook == null || userX == null || userY == null)
			{
				return false;
			}
			bool result;
			try
			{
				result = this.addressBook.IsSameRecipient((RoutingAddress)userX, (RoutingAddress)userY);
			}
			catch (Exception ex)
			{
				throw new TransportRulePermanentException(string.Format("Error matching recipients. Recipient1: '{0}', Recipient2: '{1}'. Inner error message: '{2}'", userX, userY, ex.Message), ex);
			}
			return result;
		}

		private AddressBook addressBook;
	}
}
