using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IMessageRecipient : IEquatable<IMessageRecipient>
	{
		IIdentity Identity { get; }

		string DisplayName { get; }

		string EmailAddress { get; }

		string SmtpAddress { get; }

		string SipUri { get; }

		string RoutingType { get; }

		bool IsDistributionList { get; }

		RecipientDisplayType RecipientDisplayType { get; }

		string Alias { get; }

		void UpdateFromRecipient(IMessageRecipient recipient);
	}
}
