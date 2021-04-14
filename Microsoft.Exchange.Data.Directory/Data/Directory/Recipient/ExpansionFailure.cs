using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal enum ExpansionFailure
	{
		AlternateRecipientNotFound,
		LoopDetected,
		NoMembers,
		NotAuthorized,
		NotMailEnabled
	}
}
