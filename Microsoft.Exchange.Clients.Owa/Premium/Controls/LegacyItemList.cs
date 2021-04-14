using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal abstract class LegacyItemList : LegacyListViewContents
	{
		protected LegacyItemList(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext) : this(viewDescriptor, sortedColumn, sortOrder, userContext, SearchScope.SelectedFolder)
		{
		}

		protected LegacyItemList(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext, SearchScope folderScope) : base(userContext)
		{
			this.viewDescriptor = viewDescriptor;
			this.sortedColumn = ListViewColumns.GetColumn(sortedColumn);
			this.sortOrder = sortOrder;
			this.folderScope = folderScope;
			for (int i = 0; i < viewDescriptor.PropertyCount; i++)
			{
				base.AddProperty(viewDescriptor.GetProperty(i));
			}
		}

		public override ViewDescriptor ViewDescriptor
		{
			get
			{
				return this.viewDescriptor;
			}
		}

		protected SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		protected Column SortedColumn
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

		protected virtual void RenderItemMetaDataExpandos(TextWriter writer)
		{
			this.InternalRenderItemMetaDataExpandos(writer);
		}

		protected void InternalRenderItemMetaDataExpandos(TextWriter writer)
		{
			LegacyItemList.RenderRowId(writer, this.DataSource.GetItemId());
			writer.Write(" ");
			writer.Write("t");
			writer.Write("=\"");
			Utilities.HtmlEncode(Utilities.UrlEncode(this.DataSource.GetItemClass()), writer);
			writer.Write("\"");
		}

		protected void RenderItemTooltip(TextWriter writer)
		{
			if (this.folderScope == SearchScope.SelectedFolder)
			{
				return;
			}
			if (!(this.DataSource is FolderListViewDataSource))
			{
				throw new InvalidOperationException(string.Format("DataSource must be FolderListViewDataSource to show item tooltip. Actual Type {0}", this.DataSource.GetType()));
			}
			writer.Write(" title=\"");
			string itemProperty = this.DataSource.GetItemProperty<string>(ItemSchema.ParentDisplayName, string.Empty);
			string htmlEncoded = LocalizedStrings.GetHtmlEncoded(699235260);
			writer.Write(string.Format(htmlEncoded, Utilities.HtmlEncode(itemProperty)));
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
			bool itemProperty2 = this.DataSource.GetItemProperty<bool>(MessageItemSchema.HasBeenSubmitted, false);
			if (itemProperty && !itemProperty2)
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
			int itemProperty3 = this.DataSource.GetItemProperty<int>(ItemSchema.EdgePcl, 1);
			bool itemProperty4 = this.DataSource.GetItemProperty<bool>(ItemSchema.LinkEnabled, false);
			if (JunkEmailUtilities.IsSuspectedPhishingItem(itemProperty3) && !itemProperty4)
			{
				writer.Write("=1");
			}
			else
			{
				writer.Write("=0");
			}
			this.RenderMeetingRequestExpandos(writer);
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
				string mailboxOwnerLegacyDN = base.UserContext.MailboxSession.MailboxOwnerLegacyDN;
				bool flag = MeetingUtilities.CheckOrganizer(itemProperty2, itemProperty3, mailboxOwnerLegacyDN);
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

		protected const string SecondaryRow = "sr";

		protected const string ItemState = "s";

		protected const string ItemRead = "read";

		private const string ResponseRequested = "fRR";

		private const string IsOrganizer = "fDoR";

		protected const string FlagId = "tdFlg";

		protected const string CategoryId = "tdCat";

		private const string StoreObjectType = "t";

		private const string ItemId = "id";

		private const string ItemIdPrefix = "b";

		private const string IsSuspectedPhishingItemWithoutLinkEnabled = "fPhsh";

		private ViewDescriptor viewDescriptor;

		private Column sortedColumn;

		private SortOrder sortOrder;

		private SearchScope folderScope;
	}
}
