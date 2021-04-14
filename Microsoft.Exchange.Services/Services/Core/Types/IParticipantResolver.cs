using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal interface IParticipantResolver
	{
		EmailAddressWrapper[] ResolveToEmailAddressWrapper(IEnumerable<IParticipant> participants);

		SingleRecipientType ResolveToSingleRecipientType(IParticipant participant);

		SmtpAddress ResolveToSmtpAddress(IParticipant participant);

		SingleRecipientType[] ResolveToSingleRecipientType(IEnumerable<IParticipant> participants);

		void LoadAdDataIfNeeded(IEnumerable<IParticipant> pregatherParticipants);
	}
}
