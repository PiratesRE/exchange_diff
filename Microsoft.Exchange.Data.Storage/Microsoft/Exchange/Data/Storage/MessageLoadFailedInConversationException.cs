using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MessageLoadFailedInConversationException : StoragePermanentException
	{
		internal MessageLoadFailedInConversationException(LocalizedString message) : base(message)
		{
		}

		internal MessageLoadFailedInConversationException(LocalizedString message, Exception exception) : base(message, exception)
		{
		}
	}
}
