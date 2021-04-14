using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregationBySmsItemClassProcessor
	{
		public AggregationBySmsItemClassProcessor(IXSOFactory xsoFactory, IMailboxSession session, ConversationIndexTrackingEx indexTrackingEx)
		{
			Util.ThrowOnNullArgument(session, "session");
			this.session = session;
			this.indexTrackingEx = indexTrackingEx;
			this.xsoFactory = xsoFactory;
		}

		public static void ChunkSmsConversation(IXSOFactory xsoFactory, IMailboxSession session, ConversationIndex conversationIndex, ConversationIndexTrackingEx conversationIndexTrackingEx)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(conversationIndex, "conversationIndex");
			byte[] conversationIdBytes = ConversationId.Create(conversationIndex.Guid).GetBytes();
			int conversationIdHash = (int)AllItemsFolderHelper.GetHashValue(conversationIdBytes);
			Stopwatch stopwatch = Stopwatch.StartNew();
			List<IStorePropertyBag> list = xsoFactory.RunQueryOnAllItemsFolder<List<IStorePropertyBag>>(session, AllItemsFolderHelper.SupportedSortBy.ConversationIdHash, conversationIdHash, null, delegate(QueryResult queryResult)
			{
				List<IStorePropertyBag> list2 = null;
				bool flag = false;
				while (!flag)
				{
					IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(10);
					if (propertyBags == null || propertyBags.Length <= 0)
					{
						break;
					}
					for (int i = 0; i < propertyBags.Length; i++)
					{
						int? num2 = propertyBags[i].TryGetProperty(ItemSchema.ConversationIdHash) as int?;
						if (num2 == null)
						{
							break;
						}
						if (num2.Value != conversationIdHash)
						{
							flag = true;
							break;
						}
						byte[] array = propertyBags[i].TryGetProperty(InternalSchema.MapiConversationId) as byte[];
						if (array != null && Util.CompareByteArray(conversationIdBytes, array))
						{
							if (list2 == null)
							{
								list2 = new List<IStorePropertyBag>(10);
							}
							list2.Add(propertyBags[i]);
						}
					}
				}
				return list2;
			}, new StorePropertyDefinition[]
			{
				ItemSchema.Id,
				ItemSchema.ConversationIdHash,
				ItemSchema.ReceivedTime,
				InternalSchema.MapiConversationId
			});
			stopwatch.Stop();
			if (conversationIndexTrackingEx != null)
			{
				conversationIndexTrackingEx.Trace("SBSMSCID", stopwatch.ElapsedMilliseconds.ToString());
			}
			if (list == null || list.Count < 79)
			{
				return;
			}
			list.Sort(delegate(IStorePropertyBag x, IStorePropertyBag y)
			{
				ExDateTime? exDateTime = x.TryGetProperty(ItemSchema.ReceivedTime) as ExDateTime?;
				ExDateTime? exDateTime2 = y.TryGetProperty(ItemSchema.ReceivedTime) as ExDateTime?;
				if (exDateTime == null && exDateTime2 == null)
				{
					return 0;
				}
				if (exDateTime == null)
				{
					return 1;
				}
				if (exDateTime2 == null)
				{
					return -1;
				}
				return exDateTime.Value.CompareTo(exDateTime2.Value);
			});
			string str = conversationIndex.Guid.ToString("D").Substring("472e2878-19b1-4ac1-a21a-".Length);
			string text;
			do
			{
				text = Guid.NewGuid().ToString("D").Substring(0, "472e2878-19b1-4ac1-a21a-".Length);
			}
			while (string.Equals(text, "472e2878-19b1-4ac1-a21a-"));
			ConversationIndex conversationIndex2 = ConversationIndex.Create(new Guid(text + str));
			int num = 0;
			while (num < list.Count && num < 50)
			{
				VersionedId versionedId = (VersionedId)list[num].TryGetProperty(ItemSchema.Id);
				if (versionedId != null)
				{
					StoreObjectId objectId = versionedId.ObjectId;
					if (objectId != null)
					{
						using (IMessageItem messageItem = xsoFactory.BindToMessage(session, objectId, null))
						{
							messageItem.OpenAsReadWrite();
							messageItem[ItemSchema.ConversationIndex] = conversationIndex2.ToByteArray();
							messageItem[ItemSchema.ConversationIndexTracking] = true;
							messageItem.Save(SaveMode.ResolveConflicts);
						}
					}
				}
				num++;
			}
		}

		public bool Aggregate(ICorePropertyBag propertyBag, out ConversationIndex newIndex, out ConversationIndex.FixupStage stage)
		{
			newIndex = ConversationIndex.Empty;
			Participant valueOrDefault = propertyBag.GetValueOrDefault<Participant>(InternalSchema.From);
			if (valueOrDefault != null && string.Equals("MOBILE", valueOrDefault.RoutingType, StringComparison.OrdinalIgnoreCase))
			{
				newIndex = ConversationIndex.GenerateFromPhoneNumber(valueOrDefault.EmailAddress);
			}
			if (newIndex != ConversationIndex.Empty)
			{
				stage = ConversationIndex.FixupStage.S1;
				AggregationBySmsItemClassProcessor.ChunkSmsConversation(this.xsoFactory, this.session, newIndex, this.indexTrackingEx);
			}
			else
			{
				newIndex = ConversationIndex.CreateNew();
				stage = ConversationIndex.FixupStage.S2;
			}
			return false;
		}

		public const string SmsConversationIndexGuidPrefix = "472e2878-19b1-4ac1-a21a-";

		private const int PastConversationItemCount = 50;

		private const string MobileRoutingType = "MOBILE";

		private const int LatestConversationMaxItemCount = 80;

		private const int CountOfRowsFetchedWhenSearchingByIndex = 10;

		private readonly ConversationIndexTrackingEx indexTrackingEx;

		private readonly IMailboxSession session;

		private readonly IXSOFactory xsoFactory;
	}
}
