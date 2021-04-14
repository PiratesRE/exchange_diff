using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationDataExtractorFactory : IConversationDataExtractorFactory
	{
		public ConversationDataExtractorFactory(IXSOFactory xsoFactory, IMailboxSession session)
		{
			this.session = session;
			this.xsoFactory = xsoFactory;
		}

		public ConversationDataExtractor Create(bool isIrmEnabled, HashSet<PropertyDefinition> propertiesLoadedForExtractedItems, ConversationId conversationId, IConversationTree tree)
		{
			return this.Create(isIrmEnabled, propertiesLoadedForExtractedItems, conversationId, tree, false, null);
		}

		public ConversationDataExtractor Create(bool isIrmEnabled, HashSet<PropertyDefinition> propertiesLoadedForExtractedItems, ConversationId conversationId, IConversationTree tree, bool isSmimeSupported, string domainName)
		{
			PropertyDefinition[] queriedPropertyDefinitions = (propertiesLoadedForExtractedItems != null) ? propertiesLoadedForExtractedItems.ToArray<PropertyDefinition>() : Array<PropertyDefinition>.Empty;
			int totalNodeCount = (tree == null) ? 0 : tree.Count;
			ItemPartLoader itemPartLoader = new ItemPartLoader(this.xsoFactory, this.session, isIrmEnabled, queriedPropertyDefinitions);
			OptimizationInfo optimizationInfo = new OptimizationInfo(conversationId, totalNodeCount);
			return new ConversationDataExtractor(itemPartLoader, isIrmEnabled, optimizationInfo, isSmimeSupported, domainName);
		}

		private readonly IMailboxSession session;

		private readonly IXSOFactory xsoFactory;
	}
}
