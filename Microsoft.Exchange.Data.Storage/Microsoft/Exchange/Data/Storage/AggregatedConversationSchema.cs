using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregatedConversationSchema : Schema
	{
		public new static AggregatedConversationSchema Instance
		{
			get
			{
				return AggregatedConversationSchema.instance;
			}
		}

		public const string IdPropertyName = "Id";

		public const string LastDeliveryTimePropertyName = "LastDeliveryTime";

		public const string TopicPropertyName = "ConversationTopic";

		public const string PreviewPropertyName = "Preview";

		public const string HasAttachmentsPropertyName = "HasAttachments";

		public const string HasIrmPropertyName = "HasIrm";

		public const string ItemCountPropertyName = "ItemCount";

		public const string SizePropertyName = "Size";

		public const string ImportancePropertyName = "Importance";

		public const string TotalItemLikesPropertyName = "TotalItemLikes";

		public const string DirectParticipantsPropertyName = "DirectParticipants";

		public const string ItemIdsPropertyName = "ItemIds";

		public const string DraftItemIdsPropertyName = "DraftItemIds";

		public const string ItemClassesPropertyName = "ItemClasses";

		public const string InstanceKeyPropertyName = "InstanceKey";

		public const string ConversationLikesPropertyName = "ConversationLikes";

		public const string IconIndexPropertyName = "IconIndex";

		public const string FlagStatusPropertyName = "FlagStatus";

		public const string UnreadCountPropertyName = "UnreadCount";

		public const string RichContentPropertyName = "RichContent";

		public static readonly ApplicationAggregatedProperty Id = new ApplicationAggregatedProperty("Id", typeof(ConversationId), PropertyFlags.None, ConversationPropertyAggregationStrategy.ConversationIdProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty LastDeliveryTime = new ApplicationAggregatedProperty("LastDeliveryTime", typeof(ExDateTime), PropertyFlags.None, ConversationPropertyAggregationStrategy.LastDeliveryTimeProperty, SortByAndFilterStrategy.CreateSimpleSort(InternalSchema.InternalConversationGlobalLastDeliveryTime));

		public static readonly ApplicationAggregatedProperty Topic = new ApplicationAggregatedProperty("ConversationTopic", typeof(string), PropertyFlags.None, ConversationPropertyAggregationStrategy.ConversationTopicProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Preview = new ApplicationAggregatedProperty("Preview", typeof(string), PropertyFlags.None, ConversationPropertyAggregationStrategy.PreviewProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty HasAttachments = new ApplicationAggregatedProperty("HasAttachments", typeof(bool), PropertyFlags.None, ConversationPropertyAggregationStrategy.HasAttachmentsProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty HasIrm = new ApplicationAggregatedProperty("HasIrm", typeof(bool), PropertyFlags.None, ConversationPropertyAggregationStrategy.HasIrmProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty ItemCount = new ApplicationAggregatedProperty("ItemCount", typeof(int), PropertyFlags.None, ConversationPropertyAggregationStrategy.ItemCountProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Size = new ApplicationAggregatedProperty("Size", typeof(int), PropertyFlags.None, ConversationPropertyAggregationStrategy.SizeProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Importance = new ApplicationAggregatedProperty("Importance", typeof(Importance), PropertyFlags.None, ConversationPropertyAggregationStrategy.ImportanceProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty TotalItemLikes = new ApplicationAggregatedProperty("TotalItemLikes", typeof(int), PropertyFlags.None, ConversationPropertyAggregationStrategy.TotalItemLikesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty ConversationLikes = new ApplicationAggregatedProperty("ConversationLikes", typeof(int), PropertyFlags.None, ConversationPropertyAggregationStrategy.ConversationLikesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty DirectParticipants = new ApplicationAggregatedProperty("DirectParticipants", typeof(Participant[]), PropertyFlags.Multivalued, ConversationPropertyAggregationStrategy.DirectParticipantsProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty ItemIds = new ApplicationAggregatedProperty("ItemIds", typeof(StoreObjectId[]), PropertyFlags.Multivalued, PropertyAggregationStrategy.EntryIdsProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty DraftItemIds = new ApplicationAggregatedProperty("DraftItemIds", typeof(StoreObjectId[]), PropertyFlags.Multivalued, ConversationPropertyAggregationStrategy.DraftItemIdsProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty ItemClasses = new ApplicationAggregatedProperty("ItemClasses", typeof(string[]), PropertyFlags.Multivalued, PropertyAggregationStrategy.ItemClassesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty IconIndex = new ApplicationAggregatedProperty("IconIndex", typeof(IconIndex), PropertyFlags.None, ConversationPropertyAggregationStrategy.IconIndexProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty FlagStatus = new ApplicationAggregatedProperty("FlagStatus", typeof(FlagStatus), PropertyFlags.None, ConversationPropertyAggregationStrategy.FlagStatusProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty UnreadCount = new ApplicationAggregatedProperty("UnreadCount", typeof(int), PropertyFlags.None, ConversationPropertyAggregationStrategy.UnreadCountProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty RichContent = new ApplicationAggregatedProperty("RichContent", typeof(short[]), PropertyFlags.Multivalued, ConversationPropertyAggregationStrategy.RichContentProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty InstanceKey = new ApplicationAggregatedProperty("InstanceKey", typeof(byte[]), PropertyFlags.Multivalued, ConversationPropertyAggregationStrategy.InstanceKeyProperty, SortByAndFilterStrategy.None);

		private static readonly AggregatedConversationSchema instance = new AggregatedConversationSchema();
	}
}
