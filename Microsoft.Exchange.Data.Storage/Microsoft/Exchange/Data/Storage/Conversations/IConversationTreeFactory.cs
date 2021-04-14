using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConversationTreeFactory
	{
		IConversationTree Create(IEnumerable<IStorePropertyBag> queryResult, IEnumerable<PropertyDefinition> propertyDefinitions);

		IConversationTree GetNewestSubTree(IConversationTree conversationTree, int count);

		HashSet<PropertyDefinition> CalculatePropertyDefinitionsToBeLoaded(ICollection<PropertyDefinition> requestedProperties);
	}
}
