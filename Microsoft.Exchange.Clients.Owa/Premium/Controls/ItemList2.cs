using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal abstract class ItemList2 : ListViewContents2
	{
		protected ItemList2(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext) : this(viewDescriptor, sortedColumn, sortOrder, userContext, SearchScope.SelectedFolder, false)
		{
		}

		protected ItemList2(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext, SearchScope folderScope) : this(viewDescriptor, sortedColumn, sortOrder, userContext, folderScope, false)
		{
		}

		protected ItemList2(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext, SearchScope folderScope, bool renderLastModifiedTime) : base(userContext)
		{
			this.viewDescriptor = viewDescriptor;
			this.sortedColumn = ListViewColumns.GetColumn(sortedColumn);
			this.sortOrder = sortOrder;
			this.folderScope = folderScope;
			this.RenderLastModifiedTime = renderLastModifiedTime;
			if (folderScope != SearchScope.SelectedFolder && !(this is ConversationItemList2) && folderScope != SearchScope.SelectedFolder)
			{
				base.AddProperty(ItemSchema.ParentDisplayName);
			}
			for (int i = 0; i < viewDescriptor.PropertyCount; i++)
			{
				base.AddProperty(viewDescriptor.GetProperty(i));
			}
			if (this.RenderLastModifiedTime)
			{
				base.AddProperty(StoreObjectSchema.LastModifiedTime);
			}
		}

		public override ViewDescriptor ViewDescriptor
		{
			get
			{
				return this.viewDescriptor;
			}
		}

		public SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		public Column SortedColumn
		{
			get
			{
				return this.sortedColumn;
			}
		}

		protected static void RenderRowId(TextWriter writer, string id)
		{
			writer.Write(" ");
			writer.Write("id");
			writer.Write("=\"");
			writer.Write("b");
			Utilities.HtmlEncode(id, writer);
			writer.Write("\"");
		}

		protected static void RenderInstanceKey(TextWriter writer, string instanceKey)
		{
			writer.Write(" ");
			writer.Write("ik");
			writer.Write("=\"");
			Utilities.HtmlEncode(instanceKey, writer);
			writer.Write("\"");
		}

		protected virtual void RenderItemMetaDataExpandos(TextWriter writer)
		{
			this.InternalRenderItemMetaDataExpandos(writer);
		}

		protected void InternalRenderItemMetaDataExpandos(TextWriter writer)
		{
			writer.Write(" ");
			writer.Write("t");
			writer.Write("=\"");
			Utilities.HtmlEncode(Utilities.UrlEncode(this.DataSource.GetItemClass()), writer);
			writer.Write("\"");
		}

		protected void RenderLastModifiedTimeExpando(TextWriter writer)
		{
			ExDateTime itemProperty = this.DataSource.GetItemProperty<ExDateTime>(StoreObjectSchema.LastModifiedTime, ExDateTime.MinValue);
			if (itemProperty != ExDateTime.MinValue)
			{
				writer.Write(" sLstMdfyTm=\"");
				writer.Write(itemProperty.UtcTicks);
				writer.Write("\"");
			}
		}

		protected void RenderItemTooltip(TextWriter writer)
		{
			if (this.folderScope == SearchScope.SelectedFolder)
			{
				return;
			}
			if (!(this.DataSource is FolderListViewDataSource) && !(this.DataSource is ListViewNotificationDataSource))
			{
				throw new InvalidOperationException(string.Format("DataSource must be FolderListViewDataSource to show item tooltip. Actual Type {0}", this.DataSource.GetType()));
			}
			writer.Write(" title=\"");
			string itemProperty = this.DataSource.GetItemProperty<string>(ItemSchema.ParentDisplayName, string.Empty);
			string htmlEncodedLocalizeString = this.GetHtmlEncodedLocalizeString(699235260);
			writer.Write(string.Format(htmlEncodedLocalizeString, Utilities.HtmlEncode(itemProperty)));
			writer.Write("\"");
		}

		protected void RenderMessageViewItemMetaDataExpandos(TextWriter writer)
		{
			this.InternalRenderItemMetaDataExpandos(writer);
			if (!this.DataSource.GetItemProperty<bool>(MessageItemSchema.IsRead, true))
			{
				writer.Write(" ");
				writer.Write("read");
				writer.Write("=\"0\"");
			}
			bool itemProperty = this.DataSource.GetItemProperty<bool>(MessageItemSchema.IsDraft, false);
			if (itemProperty)
			{
				writer.Write(" ");
				writer.Write("s");
				writer.Write("=\"Draft\"");
			}
			if (this.viewDescriptor.ContainsColumn(ColumnId.FlagDueDate) || this.viewDescriptor.ContainsColumn(ColumnId.ContactFlagDueDate))
			{
				this.RenderFlagState(writer);
			}
			writer.Write(" ");
			writer.Write("fPhsh");
			int itemProperty2 = this.DataSource.GetItemProperty<int>(ItemSchema.EdgePcl, 1);
			bool itemProperty3 = this.DataSource.GetItemProperty<bool>(ItemSchema.LinkEnabled, false);
			if (JunkEmailUtilities.IsSuspectedPhishingItem(itemProperty2) && !itemProperty3)
			{
				writer.Write("=1");
			}
			else
			{
				writer.Write("=0");
			}
			bool itemProperty4 = this.DataSource.GetItemProperty<bool>(StoreObjectSchema.IsRestricted, false);
			if (itemProperty4 && base.UserContext.IsIrmEnabled)
			{
				ContentRight itemProperty5 = (ContentRight)this.DataSource.GetItemProperty<int>(MessageItemSchema.DRMRights, 0);
				RenderingUtilities.RenderExpando(writer, "fRplR", itemProperty5.IsUsageRightGranted(ContentRight.Reply) ? 0 : 1);
				RenderingUtilities.RenderExpando(writer, "fRAR", itemProperty5.IsUsageRightGranted(ContentRight.ReplyAll) ? 0 : 1);
				RenderingUtilities.RenderExpando(writer, "fFR", itemProperty5.IsUsageRightGranted(ContentRight.Forward) ? 0 : 1);
			}
			if (ConversationUtilities.IsConversationExcludedType(this.DataSource.GetItemClass()))
			{
				RenderingUtilities.RenderExpando(writer, "fExclCnv", 1);
			}
			this.RenderMeetingRequestExpandos(writer);
			if (this.RenderLastModifiedTime)
			{
				this.RenderLastModifiedTimeExpando(writer);
			}
		}

		protected void RenderMeetingRequestExpandos(TextWriter writer)
		{
			if (base.UserContext.IsFeatureEnabled(Feature.Calendar) && ObjectClass.IsMeetingRequest(this.DataSource.GetItemClass()))
			{
				writer.Write(" fMR=1");
				bool itemProperty = this.DataSource.GetItemProperty<bool>(ItemSchema.IsResponseRequested, false);
				writer.Write(" ");
				writer.Write("fRR");
				writer.Write("=");
				writer.Write(itemProperty ? "1" : "0");
				string itemProperty2 = this.DataSource.GetItemProperty<string>(MessageItemSchema.ReceivedRepresentingEmailAddress, string.Empty);
				string itemProperty3 = this.DataSource.GetItemProperty<string>(CalendarItemBaseSchema.OrganizerEmailAddress, string.Empty);
				bool flag = MeetingUtilities.CheckOrganizer(itemProperty2, itemProperty3, base.UserContext.MailboxOwnerLegacyDN);
				if (flag)
				{
					writer.Write(" ");
					writer.Write("fDoR");
					writer.Write("=");
					writer.Write(flag ? "1" : "0");
				}
			}
		}

		protected void RenderFlagState(TextWriter writer)
		{
			FlagStatus flagStatus = this.DataSource.GetItemProperty<FlagStatus>(ItemSchema.FlagStatus, FlagStatus.NotFlagged);
			int itemProperty = this.DataSource.GetItemProperty<int>(ItemSchema.ItemColor, int.MinValue);
			bool flag = ObjectClass.IsTask(this.DataSource.GetItemClass());
			if (flag)
			{
				if (this.DataSource.GetItemProperty<bool>(ItemSchema.IsComplete, false))
				{
					flagStatus = FlagStatus.Complete;
				}
				else
				{
					flagStatus = FlagStatus.Flagged;
				}
			}
			if (flagStatus != FlagStatus.NotFlagged)
			{
				ExDateTime itemProperty2 = this.DataSource.GetItemProperty<ExDateTime>(ItemSchema.UtcDueDate, ExDateTime.MinValue);
				if (itemProperty2 != ExDateTime.MinValue)
				{
					writer.Write(" sFlgDt=\"");
					writer.Write(DateTimeUtilities.GetJavascriptDate(itemProperty2));
					writer.Write("\"");
				}
				FlagAction flagActionForItem = FlagContextMenu.GetFlagActionForItem(base.UserContext, itemProperty2, flagStatus);
				writer.Write(" sFA=");
				writer.Write((int)flagActionForItem);
				if (itemProperty == -2147483648 && flagStatus == FlagStatus.Flagged && !flag)
				{
					writer.Write(" sFS=1");
				}
			}
		}

		internal const string IsExcludedFromConversations = "fExclCnv";

		protected const string SecondaryRow = "sr";

		protected const string ItemState = "s";

		protected const string ItemRead = "read";

		private const string ResponseRequested = "fRR";

		private const string IsOrganizer = "fDoR";

		protected const string FlagId = "divFlg";

		protected const string CategoryId = "divCat";

		private const string StoreObjectType = "t";

		private const string ItemId = "id";

		private const string InstanceKey = "ik";

		private const string ItemIdPrefix = "b";

		private const string IsSuspectedPhishingItemWithoutLinkEnabled = "fPhsh";

		private const string IsReplyRestricted = "fRplR";

		private const string IsReplyAllRestricted = "fRAR";

		private const string IsForwardRestricted = "fFR";

		protected readonly bool RenderLastModifiedTime;

		private ViewDescriptor viewDescriptor;

		private Column sortedColumn;

		private SortOrder sortOrder;

		private SearchScope folderScope;
	}
}
