using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConversationDataExtractorFactory
	{
		ConversationDataExtractor Create(bool isIrmEnabled, HashSet<PropertyDefinition> propertiesLoadedForExtractedItems, ConversationId conversationId, IConversationTree tree);

		ConversationDataExtractor Create(bool isIrmEnabled, HashSet<PropertyDefinition> propertiesLoadedForExtractedItems, ConversationId conversationId, IConversationTree tree, bool isSmimeSupported, string domainName);
	}
}
