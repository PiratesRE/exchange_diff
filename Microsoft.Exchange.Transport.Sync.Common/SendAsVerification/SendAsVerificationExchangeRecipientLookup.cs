using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Common.SendAsVerification
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SendAsVerificationExchangeRecipientLookup
	{
		internal string ExchangeRecipientFor(ADUser user, SyncLogSession syncLogSession)
		{
			SyncUtilities.ThrowIfArgumentNull("user", user);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			string result;
			if (!EmailGenerationUtilities.TryGetMicrosoftExchangeRecipientSmtpAddress(user.Session.SessionSettings, syncLogSession, out result))
			{
				throw new FailedToGenerateVerificationEmailException();
			}
			return result;
		}
	}
}
