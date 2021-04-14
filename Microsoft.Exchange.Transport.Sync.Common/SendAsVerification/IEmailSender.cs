using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.SendAsVerification
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IEmailSender
	{
		bool SendAttempted { get; }

		bool SendSuccessful { get; }

		string MessageId { get; }

		void SendWith(Guid sharedSecret);
	}
}
