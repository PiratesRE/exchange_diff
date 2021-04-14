using System;
using System.Security.Principal;

namespace Microsoft.Exchange.Data.Transport
{
	public abstract class AddressBookEntry
	{
		internal AddressBookEntry()
		{
		}

		public abstract RoutingAddress PrimaryAddress { get; }

		public abstract bool RequiresAuthentication { get; }

		public abstract bool AntispamBypass { get; }

		public abstract RecipientType RecipientType { get; }

		public abstract SecurityIdentifier UserAccountSid { get; }

		public abstract SecurityIdentifier MasterAccountSid { get; }

		public abstract string WindowsLiveId { get; }

		public abstract int GetSpamConfidenceLevelThreshold(SpamAction action, int defaultValue);

		public abstract bool IsSafeSender(RoutingAddress senderAddress);

		public abstract bool IsSafeRecipient(RoutingAddress recipientAddress);

		public abstract bool IsBlockedSender(RoutingAddress senderAddress);
	}
}
