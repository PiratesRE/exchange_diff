using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConversation : ICoreConversation, IConversationData
	{
		byte[] ConversationCreatorSID { get; }

		EffectiveRights EffectiveRights { get; }
	}
}
