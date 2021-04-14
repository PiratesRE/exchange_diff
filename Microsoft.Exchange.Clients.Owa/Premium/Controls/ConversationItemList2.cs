using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class ConversationItemList2 : ItemList2
	{
		public ConversationItemList2(ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext, SearchScope folderScope, DefaultFolderType folderType, OwaStoreObjectId parentFolderId, bool renderLastModifiedTime) : base((folderType == DefaultFolderType.SentItems) ? ConversationItemList2.conversationToViewDescriptor : ConversationItemList2.conversationViewDescriptor, sortedColumn, sortOrder, userContext, folderScope, renderLastModifiedTime)
		{
			this.parentFolderId = parentFolderId;
			this.participantColumnId = ((folderType == DefaultFolderType.SentItems) ? ColumnId.ConversationToList : ColumnId.ConversationSenderList);
			base.AddProperty(ConversationItemSchema.ConversationId);
			base.AddProperty(ConversationItemSchema.ConversationGlobalMessageCount);
			base.AddProperty(ConversationItemSchema.ConversationGlobalItemIds);
			base.AddProperty(ConversationItemSchema.ConversationMessageClasses);
			base.AddProperty(ConversationItemSchema.ConversationGlobalMessageClasses);
			base.AddProperty(ConversationItemSchema.ConversationReplyForwardState);
			base.AddProperty(ConversationItemSchema.ConversationHasIrm);
			base.AddProperty(ItemSchema.InstanceKey);
			base.AddProperty(ConversationItemSchema.ConversationItemIds);
		}

		internal IList<StoreObjectId> ExcludedFolderIds
		{
			get
			{
				if (this.excludedFolderIds == null)
				{
					this.excludedFolderIds = ConversationUtilities.GetExcludedFolderIds((MailboxSession)this.parentFolderId.GetSession(base.UserContext), this.parentFolderId.StoreObjectId);
				}
				return this.excludedFolderIds;
			}
		}

		protected override void ValidatedRender(TextWriter writer, int startRange, int endRange)
		{
			writer.Write("<div class=\"baseIL cvIL\" id=\"");
			writer.Write("divVLVIL");
			writer.Write("\">");
			this.DataSource.MoveToItem(startRange);
			while (this.DataSource.CurrentItem <= endRange)
			{
				this.RenderCurrentConversationRow(writer);
				this.DataSource.MoveNext();
			}
			writer.Write("</div>");
		}

		protected override bool InternalRenderColumn(TextWriter writer, ColumnId columnId)
		{
			switch (columnId)
			{
			case ColumnId.ConversationLastDeliveryTime:
				return this.RenderLastDeliveryTime(writer);
			case ColumnId.ConversationIcon:
				return this.RenderConversationIcon(writer);
			case ColumnId.ConversationSubject:
				return this.RenderConversationSubject(writer);
			case ColumnId.ConversationUnreadCount:
				return this.RenderConversationUnreadCount(writer);
			case ColumnId.ConversationHasAttachment:
				return this.RenderConversationHasAttachment(writer);
			case ColumnId.ConversationSenderList:
				return this.RenderConversationRecipientList(writer, ConversationItemSchema.ConversationMVFrom);
			case ColumnId.ConversationFlagDueDate:
				return this.RenderConversationFlag(writer);
			case ColumnId.ConversationToList:
				return this.RenderConversationRecipientList(writer, ConversationItemSchema.ConversationMVTo);
			}
			return base.InternalRenderColumn(writer, columnId);
		}

		private void RenderCurrentConversationRow(TextWriter writer)
		{
			this.RenderRow(writer, false, ListViewContents2.ListViewRowType.RenderAllRows, true);
		}

		internal override void RenderRow(TextWriter writer, bool showFlag, ListViewContents2.ListViewRowType rowTypeToRender, bool renderContainer)
		{
			int itemProperty = this.DataSource.GetItemProperty<int>(ConversationItemSchema.ConversationUnreadMessageCount, 0);
			IList<StoreObjectId> list = this.DataSource.GetItemProperty<StoreObjectId[]>(ConversationItemSchema.ConversationGlobalItemIds, null);
			list = ConversationUtilities.ExcludeFolders(list, this.ExcludedFolderIds);
			int count = list.Count;
			this.currentRowGlobalCount = count;
			if (rowTypeToRender != ListViewContents2.ListViewRowType.RenderOnlyRow2)
			{
				if (renderContainer)
				{
					writer.Write("<div id=\"");
					writer.Write("vr");
					writer.Write("\">");
				}
				writer.Write("<div class=\"cData\"");
				ItemList2.RenderRowId(writer, this.DataSource.GetItemId());
				ItemList2.RenderInstanceKey(writer, Convert.ToBase64String(this.DataSource.GetItemProperty<byte[]>(ItemSchema.InstanceKey)));
				this.RenderConversationMetaDataExpandos(writer, count, list);
				writer.Write(">");
				writer.Write("</div>");
				writer.Write("<div class=c1 id=");
				writer.Write("divExp");
				writer.Write(">");
				if (count > 1)
				{
					if (base.UserContext.IsRtl)
					{
						base.UserContext.RenderThemeImage(writer, ThemeFileId.PlusRTL);
					}
					else
					{
						base.UserContext.RenderThemeImage(writer, ThemeFileId.Plus);
					}
				}
				writer.Write("<img id=imgPrg style=\"display:none\" src=\"");
				base.UserContext.RenderThemeFileUrl(writer, ThemeFileId.ProgressSmall);
				writer.Write("\">");
				writer.Write("</div>");
				writer.Write("<div class=c2>");
				base.RenderColumn(writer, ColumnId.ConversationIcon, true);
				base.RenderCheckbox(writer);
				writer.Write("</div>");
				writer.Write("<div id=\"");
				writer.Write("divSubject");
				writer.Write("\" class=\"c3");
				if (itemProperty > 0)
				{
					writer.Write(" ur");
				}
				writer.Write("\">");
				base.RenderColumn(writer, ColumnId.ConversationSubject, true);
				writer.Write("</div>");
				writer.Write("<div id=");
				writer.Write("divUC");
				writer.Write(" class=c4>");
				base.RenderColumn(writer, ColumnId.ConversationUnreadCount, true);
				writer.Write("</div>");
				FlagStatus itemProperty2 = (FlagStatus)this.DataSource.GetItemProperty<int>(ConversationItemSchema.ConversationFlagStatus, 0);
				writer.Write("<div class=\"c7");
				if (itemProperty2 != FlagStatus.NotFlagged)
				{
					writer.Write(" stky");
				}
				writer.Write("\" id=");
				writer.Write("divFlg");
				writer.Write(">");
				base.RenderColumn(writer, ColumnId.ConversationFlagDueDate, true);
				writer.Write("</div>");
				string[] itemProperty3 = this.DataSource.GetItemProperty<string[]>(ConversationItemSchema.ConversationCategories, null);
				writer.Write("<div class=\"r c6");
				if (itemProperty3 != null && itemProperty3.Length > 0)
				{
					writer.Write(" stky");
				}
				writer.Write("\" id=");
				writer.Write("divCat");
				writer.Write(">");
				base.RenderColumn(writer, ColumnId.ConversationCategories, true);
				writer.Write("</div>");
				writer.Write("<div class=c5 >");
				bool flag = base.RenderColumn(writer, ColumnId.ConversationImportance, false);
				base.RenderColumn(writer, ColumnId.ConversationHasAttachment, !flag);
				writer.Write("</div>");
				base.RenderSelectionImage(writer);
				base.UserContext.RenderThemeImage(writer, ThemeFileId.Clear1x1, "expSelBg", new object[0]);
				base.RenderRowDivider(writer);
				if (renderContainer)
				{
					writer.Write("</div>");
				}
			}
			if (rowTypeToRender != ListViewContents2.ListViewRowType.RenderOnlyRow1)
			{
				if (renderContainer)
				{
					writer.Write("<div id=");
					writer.Write("sr");
					writer.Write(">");
				}
				writer.Write("<div id=\"");
				writer.Write("divSenderList");
				writer.Write("\" class=c2>");
				base.RenderColumn(writer, this.participantColumnId, true);
				writer.Write("</div>");
				writer.Write("<div id=\"");
				writer.Write("divDateTime");
				writer.Write("\" class=\"c3\">");
				base.RenderColumn(writer, ColumnId.ConversationLastDeliveryTime, true);
				writer.Write("</div>");
				base.RenderSelectionImage(writer);
				if (renderContainer)
				{
					writer.Write("</div>");
				}
			}
		}

		private void RenderConversationMetaDataExpandos(TextWriter writer, int globalCount, IList<StoreObjectId> itemIds)
		{
			base.InternalRenderItemMetaDataExpandos(writer);
			int itemProperty = this.DataSource.GetItemProperty<int>(ConversationItemSchema.ConversationUnreadMessageCount, 0);
			writer.Write(" ");
			writer.Write("iUC");
			writer.Write("=");
			writer.Write(itemProperty);
			int itemProperty2 = this.DataSource.GetItemProperty<int>(ConversationItemSchema.ConversationMessageCount, 0);
			writer.Write(" ");
			writer.Write("iTC");
			writer.Write("=");
			writer.Write(itemProperty2);
			writer.Write(" ");
			writer.Write("iGC");
			writer.Write("=");
			writer.Write(globalCount);
			if (itemIds.Count > 0)
			{
				writer.Write(" ");
				writer.Write("sMID");
				writer.Write("=\"");
				Utilities.HtmlEncode(Utilities.GetItemIdString(itemIds[0], this.parentFolderId), writer);
				writer.Write("\"");
			}
			if (globalCount == 1)
			{
				string[] itemProperty3 = this.DataSource.GetItemProperty<string[]>(ConversationItemSchema.ConversationGlobalMessageClasses, null);
				if (base.UserContext.DraftsFolderId.Equals(this.parentFolderId.StoreObjectId))
				{
					writer.Write(" ");
					writer.Write("sMS");
					writer.Write("=\"Draft\"");
				}
				writer.Write(" ");
				writer.Write("sMT");
				writer.Write("=\"");
				Utilities.HtmlEncode(itemProperty3[0], writer);
				writer.Write("\"");
				if (ObjectClass.IsMeetingRequest(itemProperty3[0]))
				{
					writer.Write(" fMR=1");
					writer.Write(" fRR=1");
				}
			}
			if (itemProperty > 0)
			{
				writer.Write(" ");
				writer.Write("read");
				writer.Write("=\"0\"");
			}
			FlagStatus itemProperty4 = (FlagStatus)this.DataSource.GetItemProperty<int>(ConversationItemSchema.ConversationFlagStatus, 0);
			if (itemProperty4 != FlagStatus.NotFlagged)
			{
				ExDateTime itemProperty5 = this.DataSource.GetItemProperty<ExDateTime>(ConversationItemSchema.ConversationFlagCompleteTime, ExDateTime.MinValue);
				if (itemProperty5 != ExDateTime.MinValue)
				{
					writer.Write(" sFlgDt=\"");
					writer.Write(DateTimeUtilities.GetJavascriptDate(itemProperty5));
					writer.Write("\"");
				}
				FlagAction flagActionForItem = FlagContextMenu.GetFlagActionForItem(base.UserContext, itemProperty5, itemProperty4);
				writer.Write(" sFA=");
				writer.Write((int)flagActionForItem);
			}
			if (this.RenderLastModifiedTime)
			{
				base.RenderLastModifiedTimeExpando(writer);
			}
		}

		private bool RenderConversationFlag(TextWriter writer)
		{
			FlagStatus itemProperty = (FlagStatus)this.DataSource.GetItemProperty<int>(ConversationItemSchema.ConversationFlagStatus, 0);
			ThemeFileId themeFileId = ThemeFileId.FlagEmpty;
			if (itemProperty != FlagStatus.NotFlagged)
			{
				if (itemProperty == FlagStatus.Flagged)
				{
					themeFileId = ThemeFileId.Flag;
				}
				else
				{
					themeFileId = ThemeFileId.FlagComplete;
				}
			}
			base.UserContext.RenderThemeImage(writer, themeFileId, null, new object[]
			{
				"id=imgFlg"
			});
			return true;
		}

		private bool RenderConversationIcon(TextWriter writer)
		{
			this.DataSource.GetItemProperty<int>(ConversationItemSchema.ConversationMessageCount, 0);
			int itemProperty = this.DataSource.GetItemProperty<int>(ConversationItemSchema.ConversationUnreadMessageCount, 0);
			bool itemProperty2 = this.DataSource.GetItemProperty<bool>(ConversationItemSchema.ConversationHasIrm, false);
			string[] array = null;
			if (base.SortedColumn.Id == ColumnId.ConversationIcon)
			{
				string itemProperty3 = this.DataSource.GetItemProperty<string>(ConversationItemSchema.ConversationMessageClasses, null);
				if (!string.IsNullOrEmpty(itemProperty3))
				{
					array = new string[]
					{
						itemProperty3
					};
				}
			}
			else
			{
				array = this.DataSource.GetItemProperty<string[]>(ConversationItemSchema.ConversationMessageClasses, null);
			}
			int itemProperty4 = this.DataSource.GetItemProperty<int>(ConversationItemSchema.ConversationReplyForwardState, -1);
			if (this.currentRowGlobalCount == 1)
			{
				string itemClass = "IPM.Note";
				if (array != null && array.Length >= 1)
				{
					itemClass = array[0];
				}
				ListViewContentsRenderingUtilities.RenderMessageIcon(writer, base.UserContext, itemClass, itemProperty != 1, false, itemProperty4, itemProperty2);
				return true;
			}
			ThemeFileId themeFileId = itemProperty2 ? ThemeFileId.IrmConversationIconRead : ThemeFileId.ConversationIconRead;
			bool flag = false;
			if (array != null)
			{
				int num = 0;
				while (num < array.Length && ItemClassType.IsMeetingType(array[num]))
				{
					num++;
				}
				if (num >= array.Length)
				{
					flag = true;
				}
			}
			bool flag2 = false;
			if (array != null)
			{
				int num2 = 0;
				while (num2 < array.Length && ItemClassType.IsSmsType(array[num2]))
				{
					num2++;
				}
				if (num2 >= array.Length)
				{
					flag2 = true;
				}
			}
			if (flag)
			{
				themeFileId = ThemeFileId.ConversationIconMeeting;
			}
			else if (flag2)
			{
				if (itemProperty4 == 261)
				{
					themeFileId = ThemeFileId.ConversationIconSmsReply;
				}
				else if (itemProperty4 == 262)
				{
					themeFileId = ThemeFileId.ConversationIconSmsForward;
				}
				else
				{
					themeFileId = ThemeFileId.ConversationIconSmsReadAndUnread;
				}
			}
			else if (itemProperty4 == 261)
			{
				themeFileId = (itemProperty2 ? ThemeFileId.IrmConversationIconReply : ThemeFileId.ConversationIconReply);
			}
			else if (itemProperty4 == 262)
			{
				themeFileId = (itemProperty2 ? ThemeFileId.IrmConversationIconForward : ThemeFileId.ConversationIconForward);
			}
			else if (itemProperty > 0)
			{
				themeFileId = (itemProperty2 ? ThemeFileId.IrmConversationIconUnread : ThemeFileId.ConversationIconUnread);
			}
			return ListViewContentsRenderingUtilities.RenderItemIcon(writer, base.UserContext, themeFileId);
		}

		private bool RenderConversationSubject(TextWriter writer)
		{
			string itemProperty = this.DataSource.GetItemProperty<string>(ConversationItemSchema.ConversationTopic, string.Empty);
			Utilities.HtmlEncode(ConversationUtilities.MaskConversationSubject(itemProperty), writer);
			return true;
		}

		private void RenderConversationSubjectTooltip(TextWriter writer)
		{
			string itemProperty = this.DataSource.GetItemProperty<string>(ConversationItemSchema.ConversationTopic);
			if (!string.IsNullOrEmpty(itemProperty))
			{
				writer.Write(" title=\"");
				Utilities.HtmlEncode(itemProperty, writer);
				writer.Write("\"");
			}
		}

		private bool RenderConversationUnreadCount(TextWriter writer)
		{
			int itemProperty = this.DataSource.GetItemProperty<int>(ConversationItemSchema.ConversationMessageCount, 0);
			int itemProperty2 = this.DataSource.GetItemProperty<int>(ConversationItemSchema.ConversationUnreadMessageCount, 0);
			if (itemProperty2 > 1 && itemProperty > 1)
			{
				writer.Write("(");
				writer.Write("<span>");
				writer.Write(itemProperty2);
				writer.Write("</span>");
				writer.Write(")");
				return true;
			}
			return false;
		}

		private void RenderUnreadCountTooltip(TextWriter writer)
		{
			int itemProperty = this.DataSource.GetItemProperty<int>(ConversationItemSchema.ConversationMessageCount, 0);
			int itemProperty2 = this.DataSource.GetItemProperty<int>(ConversationItemSchema.ConversationUnreadMessageCount, 0);
			if (itemProperty2 > 0 && itemProperty > 1)
			{
				writer.Write(" title=\"");
				writer.Write(this.GetHtmlEncodedLocalizeString(-266594008), itemProperty, itemProperty2);
				writer.Write("\"");
			}
		}

		private bool IsSmsOnlyConversation()
		{
			string[] itemProperty = this.DataSource.GetItemProperty<string[]>(ConversationItemSchema.ConversationMessageClasses, null);
			if (itemProperty != null && itemProperty.Length != 0)
			{
				bool flag = false;
				foreach (string itemClass in itemProperty)
				{
					if (!ObjectClass.IsSmsMessage(itemClass))
					{
						flag = true;
						break;
					}
				}
				return !flag;
			}
			return false;
		}

		private bool RenderConversationHasAttachment(TextWriter writer)
		{
			bool flag = this.DataSource.GetItemProperty<bool>(ConversationItemSchema.ConversationHasAttach, false);
			if (flag && this.IsSmsOnlyConversation())
			{
				flag = false;
			}
			return ListViewContentsRenderingUtilities.RenderHasAttachments(writer, base.UserContext, flag);
		}

		private bool RenderConversationRecipientList(TextWriter writer, PropertyDefinition propertyDefinition)
		{
			string[] itemProperty = this.DataSource.GetItemProperty<string[]>(propertyDefinition, null);
			if (itemProperty == null)
			{
				string itemProperty2 = this.DataSource.GetItemProperty<string>(propertyDefinition, null);
				if (string.IsNullOrEmpty(itemProperty2))
				{
					return false;
				}
				Utilities.SanitizeHtmlEncode(itemProperty2, writer);
			}
			else
			{
				if (itemProperty.Length == 0)
				{
					return false;
				}
				for (int i = 0; i < itemProperty.Length; i++)
				{
					if (i != 0)
					{
						writer.Write("; ");
					}
					Utilities.SanitizeHtmlEncode(itemProperty[i], writer);
				}
			}
			return true;
		}

		private bool RenderLastDeliveryTime(TextWriter writer)
		{
			ExDateTime itemProperty = this.DataSource.GetItemProperty<ExDateTime>(ConversationItemSchema.ConversationLastDeliveryTime, ExDateTime.MinValue);
			return base.RenderSmartDate(writer, itemProperty);
		}

		private const string ExpandCollapseId = "divExp";

		private const string SubjectId = "divSubject";

		private const string UnreadCountId = "divUC";

		private const string ConversationUnreadCount = "iUC";

		private const string ConversationTotalCount = "iTC";

		private const string ConversationGlobalCount = "iGC";

		private const string FirstItemId = "sMID";

		private const string SendersId = "divSenderList";

		private const string DateTimeId = "divDateTime";

		private const string SingleMessageConversationItemType = "sMT";

		private const string SingleMessageConversationState = "sMS";

		private readonly OwaStoreObjectId parentFolderId;

		private static ViewDescriptor conversationViewDescriptor = new ViewDescriptor(ColumnId.ConversationLastDeliveryTime, false, new ColumnId[]
		{
			ColumnId.ConversationIcon,
			ColumnId.ConversationSubject,
			ColumnId.ConversationUnreadCount,
			ColumnId.ConversationHasAttachment,
			ColumnId.ConversationImportance,
			ColumnId.ConversationCategories,
			ColumnId.ConversationFlagDueDate,
			ColumnId.ConversationSenderList,
			ColumnId.ConversationLastDeliveryTime
		});

		private static ViewDescriptor conversationToViewDescriptor = new ViewDescriptor(ColumnId.ConversationLastDeliveryTime, false, new ColumnId[]
		{
			ColumnId.ConversationIcon,
			ColumnId.ConversationSubject,
			ColumnId.ConversationUnreadCount,
			ColumnId.ConversationHasAttachment,
			ColumnId.ConversationImportance,
			ColumnId.ConversationCategories,
			ColumnId.ConversationFlagDueDate,
			ColumnId.ConversationToList,
			ColumnId.ConversationLastDeliveryTime
		});

		private IList<StoreObjectId> excludedFolderIds;

		private int currentRowGlobalCount;

		private ColumnId participantColumnId = ColumnId.ConversationSenderList;
	}
}
