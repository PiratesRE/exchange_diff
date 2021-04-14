using System;

namespace Microsoft.Exchange.SenderId
{
	internal enum SenderIdFailReason
	{
		None = 1,
		NotPermitted,
		MalformedDomain,
		DomainDoesNotExist
	}
}
