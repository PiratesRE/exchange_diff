using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISyncProvider : IDisposeTrackable, IDisposable
	{
		ISyncLogger SyncLogger { get; }

		ISyncItem GetItem(ISyncItemId id, params PropertyDefinition[] prefetchProperties);

		ISyncWatermark CreateNewWatermark();

		void BindToFolderSync(FolderSync folderSync);

		bool GetNewOperations(ISyncWatermark minWatermark, ISyncWatermark maxSyncWatermark, bool enumerateDeletes, int numOperations, QueryFilter filter, Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest);

		void DisposeNewOperationsCachedData();

		ISyncWatermark GetMaxItemWatermark(ISyncWatermark currentWatermark);

		OperationResult DeleteItems(params ISyncItemId[] syncIds);

		List<IConversationTreeNode> GetInFolderItemsForConversation(ConversationId conversationId);

		ISyncItemId CreateISyncItemIdForNewItem(StoreObjectId itemId);
	}
}
