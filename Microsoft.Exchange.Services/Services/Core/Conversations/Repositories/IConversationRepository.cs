using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Conversations.Repositories
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IConversationRepository<out T> where T : ICoreConversation
	{
		T Load(ConversationId conversationId, IMailboxSession mailboxSession, BaseFolderId[] folderIds, bool useFolderIdsAsExclusionList = true, bool additionalPropertiesOnLoadItemParts = true, params PropertyDefinition[] requestedProperties);

		void PrefetchAndLoadItemParts(IMailboxSession mailboxSession, ICoreConversation conversation, HashSet<IConversationTreeNode> nodesToLoadItemPart, bool isSyncScenario);
	}
}
