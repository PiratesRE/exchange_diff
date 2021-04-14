using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.AutoDiscoverV2
{
	public interface IAutodMiniRecipient
	{
		SmtpProxyAddress ExternalEmailAddress { get; }

		SmtpAddress PrimarySmtpAddress { get; }

		string UserPrincipalName { get; }

		RecipientType RecipientType { get; }
	}
}
