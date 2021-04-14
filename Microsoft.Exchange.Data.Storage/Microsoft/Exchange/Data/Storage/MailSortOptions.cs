using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailSortOptions
	{
		public MailSortOptions(FolderViewColumnId columnId, PropertyDefinition itemSortProperty, PropertyDefinition conversationSortProperty)
		{
			this.ColumnId = columnId;
			this.ItemSortProperty = itemSortProperty;
			this.CoversationSortProperty = conversationSortProperty;
		}

		public FolderViewColumnId ColumnId { get; private set; }

		public PropertyDefinition ItemSortProperty { get; private set; }

		public PropertyDefinition CoversationSortProperty { get; private set; }

		public bool NeedsDeliveryTimeSecondarySortKey
		{
			get
			{
				return this.ColumnId != FolderViewColumnId.DateTime;
			}
		}

		public static ICollection<FolderViewColumnId> SupportedFolderViewColumnIds
		{
			get
			{
				return MailSortOptions.columnMap.Keys;
			}
		}

		public static SortBy[] GetSortByForFolderViewState(FolderViewState folderViewState)
		{
			MailSortOptions mailSortOptions;
			if (MailSortOptions.columnMap.TryGetValue(folderViewState.SortColumn, out mailSortOptions))
			{
				return mailSortOptions.AsSortBy(folderViewState.View, folderViewState.SortOrder);
			}
			return null;
		}

		public SortBy[] AsSortBy(FolderViewType viewType, SortOrder sortOrder)
		{
			PropertyDefinition columnDefinition = (viewType == FolderViewType.ConversationView) ? this.CoversationSortProperty : this.ItemSortProperty;
			SortBy[] array = new SortBy[this.NeedsDeliveryTimeSecondarySortKey ? 2 : 1];
			array[0] = new SortBy(columnDefinition, sortOrder);
			if (this.NeedsDeliveryTimeSecondarySortKey)
			{
				PropertyDefinition columnDefinition2 = (viewType == FolderViewType.ConversationView) ? ConversationItemSchema.ConversationLastDeliveryTime : ItemSchema.ReceivedTime;
				array[1] = new SortBy(columnDefinition2, SortOrder.Descending);
			}
			return array;
		}

		private static Dictionary<FolderViewColumnId, MailSortOptions> columnMap = new Dictionary<FolderViewColumnId, MailSortOptions>
		{
			{
				FolderViewColumnId.DateTime,
				new MailSortOptions(FolderViewColumnId.DateTime, ItemSchema.ReceivedTime, ConversationItemSchema.ConversationLastDeliveryTime)
			},
			{
				FolderViewColumnId.From,
				new MailSortOptions(FolderViewColumnId.From, ItemSchema.SentRepresentingDisplayName, ConversationItemSchema.ConversationMVFrom)
			},
			{
				FolderViewColumnId.Size,
				new MailSortOptions(FolderViewColumnId.Size, ItemSchema.Size, ConversationItemSchema.ConversationMessageSize)
			},
			{
				FolderViewColumnId.Subject,
				new MailSortOptions(FolderViewColumnId.Subject, ItemSchema.Subject, ConversationItemSchema.ConversationTopic)
			},
			{
				FolderViewColumnId.HasAttachment,
				new MailSortOptions(FolderViewColumnId.HasAttachment, ItemSchema.HasAttachment, ConversationItemSchema.ConversationHasAttach)
			},
			{
				FolderViewColumnId.Importance,
				new MailSortOptions(FolderViewColumnId.Importance, ItemSchema.Importance, ConversationItemSchema.ConversationImportance)
			},
			{
				FolderViewColumnId.Flagged,
				new MailSortOptions(FolderViewColumnId.Flagged, ItemSchema.FlagStatus, ConversationItemSchema.ConversationFlagStatus)
			},
			{
				FolderViewColumnId.To,
				new MailSortOptions(FolderViewColumnId.To, ItemSchema.DisplayTo, ConversationItemSchema.ConversationMVTo)
			},
			{
				FolderViewColumnId.ItemClass,
				new MailSortOptions(FolderViewColumnId.ItemClass, StoreObjectSchema.ItemClass, ConversationItemSchema.ConversationMessageClasses)
			}
		};
	}
}
