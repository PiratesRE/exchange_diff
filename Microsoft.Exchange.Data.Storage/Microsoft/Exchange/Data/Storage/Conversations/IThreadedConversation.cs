using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IThreadedConversation : ICoreConversation
	{
		IEnumerable<IConversationThread> Threads { get; }
	}
}
