using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class ConversationFeedLoader
	{
		public ConversationFeedLoader(MailboxSession mailboxSession, ExTimeZone requestTimeZone)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("requestTimeZone", requestTimeZone);
			this.mailboxSession = mailboxSession;
			this.requestTimeZone = requestTimeZone;
		}

		public void LoadConversationFeedItemsIfNecessary(PropertyListForViewRowDeterminer classDeterminer, IList<ConversationType> conversations)
		{
			ToServiceObjectForPropertyBagPropertyList toServiceObjectPropertyListForConversation = classDeterminer.GetToServiceObjectPropertyListForConversation();
			if (toServiceObjectPropertyListForConversation == null || toServiceObjectPropertyListForConversation.ResponseShape == null || toServiceObjectPropertyListForConversation.ResponseShape.AdditionalProperties == null)
			{
				return;
			}
			bool flag = false;
			foreach (PropertyPath propertyPath in toServiceObjectPropertyListForConversation.ResponseShape.AdditionalProperties)
			{
				PropertyUri propertyUri = propertyPath as PropertyUri;
				if (propertyUri != null && ConversationFeedLoader.ConversationFeedProperties.Contains(propertyUri.Uri))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			this.LoadConversationFeedItems(conversations);
		}

		public void LoadConversationFeedItems(ConversationType conversation, int? initialMemberDocumentId, int[] memberDocumentIds)
		{
			conversation.InternalInitialPost = initialMemberDocumentId;
			conversation.InternalRecentReplys = memberDocumentIds;
			this.LoadConversationFeedItems(new ConversationType[]
			{
				conversation
			});
		}

		private void LoadConversationFeedItems(IList<ConversationType> conversations)
		{
			HashSet<int> hashSet = this.LoadFeedItemsDocumentIds(conversations);
			if (hashSet.Count > 0)
			{
				Dictionary<int, IStorePropertyBag> storeItems = this.LoadStoreItemsForGivenDocumentIds(hashSet);
				this.LoadConversationsWithFeedItems(conversations, storeItems);
			}
		}

		private void LoadConversationsWithFeedItems(IList<ConversationType> conversations, Dictionary<int, IStorePropertyBag> storeItems)
		{
			foreach (ConversationType conversationType in conversations)
			{
				if (conversationType.InternalInitialPost != null)
				{
					conversationType.InitialPost = this.LoadMessageForGivenDocumentId((int)conversationType.InternalInitialPost, storeItems);
				}
				if (conversationType.InternalRecentReplys != null)
				{
					List<MessageType> list = this.LoadMessagesForGivenDocumentIds((int[])conversationType.InternalRecentReplys, storeItems);
					if (list.Count > 0)
					{
						conversationType.RecentReplys = list.ToArray();
					}
					else
					{
						conversationType.RecentReplys = null;
					}
				}
			}
		}

		private MessageType LoadMessageForGivenDocumentId(int documentId, Dictionary<int, IStorePropertyBag> storeItems)
		{
			IStorePropertyBag storePropertyBag = null;
			if (!storeItems.TryGetValue(documentId, out storePropertyBag))
			{
				return null;
			}
			return this.LoadMessageType(storePropertyBag);
		}

		private List<MessageType> LoadMessagesForGivenDocumentIds(int[] documentIds, Dictionary<int, IStorePropertyBag> storeItems)
		{
			List<MessageType> list = new List<MessageType>(documentIds.Length);
			foreach (int documentId in documentIds)
			{
				MessageType messageType = this.LoadMessageForGivenDocumentId(documentId, storeItems);
				if (messageType != null)
				{
					list.Add(messageType);
				}
			}
			return list;
		}

		private HashSet<int> LoadFeedItemsDocumentIds(IList<ConversationType> conversations)
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (ConversationType conversation in conversations)
			{
				int? initialPostDocumentId = this.LoadInitialPostDocumentId(conversation);
				List<int> list = this.LoadRecentReplysDocumentId(conversation, initialPostDocumentId);
				if (initialPostDocumentId != null)
				{
					hashSet.Add(initialPostDocumentId.Value);
				}
				foreach (int item in list)
				{
					hashSet.Add(item);
				}
			}
			return hashSet;
		}

		private List<int> LoadRecentReplysDocumentId(ConversationType conversation, int? initialPostDocumentId)
		{
			if (conversation.InternalRecentReplys != null && conversation.InternalRecentReplys is int[])
			{
				int[] array = (int[])conversation.InternalRecentReplys;
				List<int> list = new List<int>(array.Length);
				foreach (int num in array)
				{
					if (list.Count == ConversationFeedLoader.NumberOfRecentReplysToLoad)
					{
						break;
					}
					if (initialPostDocumentId == null || initialPostDocumentId.Value != num)
					{
						list.Add(num);
					}
				}
				if (list.Count > 0)
				{
					list.Reverse();
					conversation.InternalRecentReplys = list.ToArray();
					return list;
				}
			}
			conversation.InternalRecentReplys = null;
			return new List<int>(0);
		}

		private int? LoadInitialPostDocumentId(ConversationType conversation)
		{
			int? result = null;
			if (conversation.InternalInitialPost != null && conversation.InternalInitialPost is int)
			{
				result = new int?((int)conversation.InternalInitialPost);
			}
			else
			{
				conversation.InternalInitialPost = null;
			}
			return result;
		}

		private Dictionary<int, IStorePropertyBag> LoadStoreItemsForGivenDocumentIds(HashSet<int> documentIds)
		{
			Dictionary<int, IStorePropertyBag> result;
			using (Folder folder = Folder.Bind(this.mailboxSession, DefaultFolderType.Configuration))
			{
				QueryFilter queryFilter = this.BuildQueryFilter(documentIds);
				PropertyDefinition[] documentViewStorePropertiesToLoad = ConversationFeedLoader.DocumentViewStorePropertiesToLoad;
				using (QueryResult queryResult = folder.ItemQuery(ConversationFeedLoader.DocumentViewItemQueryType, queryFilter, null, documentViewStorePropertiesToLoad))
				{
					Dictionary<int, IStorePropertyBag> dictionary = new Dictionary<int, IStorePropertyBag>(documentIds.Count);
					IStorePropertyBag[] propertyBags;
					do
					{
						propertyBags = queryResult.GetPropertyBags(documentIds.Count);
						foreach (IStorePropertyBag storePropertyBag in propertyBags)
						{
							int valueOrDefault = storePropertyBag.GetValueOrDefault<int>(ItemSchema.DocumentId, -1);
							if (valueOrDefault != -1)
							{
								dictionary.Add(valueOrDefault, storePropertyBag);
							}
						}
					}
					while (propertyBags.Length > 0 && dictionary.Count < documentIds.Count);
					result = dictionary;
				}
			}
			return result;
		}

		private QueryFilter BuildQueryFilter(HashSet<int> documentIds)
		{
			List<QueryFilter> list = new List<QueryFilter>(documentIds.Count);
			foreach (int num in documentIds)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.DocumentId, num));
			}
			return new OrFilter(list.ToArray());
		}

		private MessageType LoadMessageType(IStorePropertyBag storePropertyBag)
		{
			VersionedId versionedId = (VersionedId)storePropertyBag.TryGetProperty(ItemSchema.Id);
			MessageType messageType = MessageType.CreateFromStoreObjectType((versionedId != null) ? versionedId.ObjectId.ObjectType : StoreObjectType.Message);
			if (messageType is MeetingMessageType)
			{
				this.AddMeetingMessageProperties(messageType as MeetingMessageType, storePropertyBag);
			}
			messageType.Preview = storePropertyBag.GetValueOrDefault<string>(ItemSchema.Preview, null);
			messageType.DateTimeReceived = this.GetDateTimeString(storePropertyBag, ItemSchema.ReceivedTime);
			messageType.From = this.GetParticipant(storePropertyBag, ItemSchema.From);
			messageType.LikeCount = storePropertyBag.GetValueOrDefault<int>(MessageItemSchema.LikeCount, 0);
			return messageType;
		}

		private void AddMeetingMessageProperties(MeetingMessageType meetingMessageType, IStorePropertyBag storePropertyBag)
		{
			if (meetingMessageType is MeetingRequestMessageType)
			{
				(meetingMessageType as MeetingRequestMessageType).MeetingRequestType = storePropertyBag.GetValueOrDefault<RequestType>(CalendarItemBaseSchema.MeetingRequestType, RequestType.None);
			}
			meetingMessageType.Sender = this.GetParticipant(storePropertyBag, ItemSchema.Sender);
		}

		private string GetDateTimeString(IStorePropertyBag storePropertyBag, PropertyDefinition dateTimeProperty)
		{
			ExDateTime valueOrDefault = storePropertyBag.GetValueOrDefault<ExDateTime>(dateTimeProperty, ExDateTime.MinValue);
			if (valueOrDefault != ExDateTime.MinValue)
			{
				return ExDateTimeConverter.ToOffsetXsdDateTime(valueOrDefault, this.requestTimeZone);
			}
			return null;
		}

		private SingleRecipientType GetParticipant(IStorePropertyBag storePropertyBag, PropertyDefinition property)
		{
			Participant valueOrDefault = storePropertyBag.GetValueOrDefault<Participant>(property, null);
			if (valueOrDefault != null)
			{
				return StaticParticipantResolver.DefaultInstance.ResolveToSingleRecipientType(valueOrDefault);
			}
			return null;
		}

		private static readonly Trace Tracer = ExTraceGlobals.ServiceCommandBaseCallTracer;

		private static readonly int NumberOfRecentReplysToLoad = 2;

		private static readonly ItemQueryType DocumentViewItemQueryType = ItemQueryType.DocumentIdView | ItemQueryType.PrereadExtendedProperties;

		public static readonly HashSet<PropertyUriEnum> ConversationFeedProperties = new HashSet<PropertyUriEnum>
		{
			PropertyUriEnum.ConversationInitialPost,
			PropertyUriEnum.ConversationRecentReplys
		};

		private static readonly PropertyDefinition[] DocumentViewStorePropertiesToLoad = new PropertyDefinition[]
		{
			ItemSchema.DocumentId,
			ItemSchema.ReceivedTime,
			ItemSchema.Preview,
			ItemSchema.Id,
			ItemSchema.From,
			ItemSchema.Sender,
			CalendarItemBaseSchema.MeetingRequestType,
			MessageItemSchema.LikeCount
		};

		private readonly MailboxSession mailboxSession;

		private readonly ExTimeZone requestTimeZone;
	}
}
