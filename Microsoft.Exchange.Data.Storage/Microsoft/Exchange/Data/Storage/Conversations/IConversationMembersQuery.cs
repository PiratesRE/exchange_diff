using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConversationMembersQuery
	{
		List<IStorePropertyBag> Query(ConversationId conversationId, ICollection<PropertyDefinition> headerPropertyDefinition, IList<StoreObjectId> folderIds, bool useFolderIdsAsExclusionList);

		Dictionary<object, List<IStorePropertyBag>> AggregateMembersPerField(PropertyDefinition field, object defaultValue, List<IStorePropertyBag> members);
	}
}
