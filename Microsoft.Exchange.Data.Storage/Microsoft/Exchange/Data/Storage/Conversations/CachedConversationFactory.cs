using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CachedConversationFactory : ConversationFactory
	{
		public CachedConversationFactory(IMailboxSession session) : base(session)
		{
			this.conversationMap = new Dictionary<string, Conversation>();
		}

		public CachedConversationFactory(IMailboxSession session, IConversationTreeFactory treeFactory, IConversationMembersQuery membersQuery, IConversationDataExtractorFactory dataExtractorFactory) : base(session, treeFactory, membersQuery, dataExtractorFactory)
		{
			this.conversationMap = new Dictionary<string, Conversation>();
		}

		public override Conversation CreateConversation(ConversationId conversationId, IList<StoreObjectId> folderIds, bool useFolderIdsAsExclusionList, bool isIrmEnabled, params PropertyDefinition[] requestedProperties)
		{
			string key = this.CalculateConversationMapKey(conversationId, folderIds, useFolderIdsAsExclusionList, isIrmEnabled, requestedProperties);
			Conversation conversation;
			if (!this.conversationMap.TryGetValue(key, out conversation))
			{
				conversation = base.CreateConversation(conversationId, folderIds, useFolderIdsAsExclusionList, isIrmEnabled, requestedProperties);
				this.conversationMap.Add(key, conversation);
			}
			return conversation;
		}

		private string CalculateConversationMapKey(ConversationId conversationId, IList<StoreObjectId> folderIds, bool useFolderIdsAsExclusionList, bool isIrmEnabled, PropertyDefinition[] requestedProperties)
		{
			string text = null;
			if (folderIds != null)
			{
				List<string> list = new List<string>(from id in folderIds
				select id.ToString().ToLower());
				list.Sort(new Comparison<string>(string.CompareOrdinal));
				text = string.Join(",", list);
			}
			string text2 = null;
			if (requestedProperties != null)
			{
				List<string> list2 = new List<string>(from prop in requestedProperties
				select prop.Name.ToLower());
				list2.Sort(new Comparison<string>(string.CompareOrdinal));
				text2 = string.Join(",", list2);
			}
			return string.Format("{0}-{1}-{2}-{3}-{4}", new object[]
			{
				conversationId,
				text ?? "<null>",
				useFolderIdsAsExclusionList,
				text2 ?? "<null>",
				isIrmEnabled
			});
		}

		private readonly Dictionary<string, Conversation> conversationMap;
	}
}
