using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class MessageMultiLineList2 : ItemList2
	{
		public MessageMultiLineList2(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext, SearchScope folderScope) : this(viewDescriptor, sortedColumn, sortOrder, userContext, folderScope, false)
		{
		}

		public MessageMultiLineList2(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext, SearchScope folderScope, bool renderLastModifiedTime) : base(viewDescriptor, sortedColumn, sortOrder, userContext, folderScope, renderLastModifiedTime)
		{
			base.AddProperty(ItemSchema.InstanceKey);
			base.AddProperty(ItemSchema.Id);
			base.AddProperty(StoreObjectSchema.ItemClass);
			base.AddProperty(MessageItemSchema.IsRead);
			base.AddProperty(MessageItemSchema.IsDraft);
			base.AddProperty(MessageItemSchema.HasBeenSubmitted);
			base.AddProperty(ItemSchema.IconIndex);
			base.AddProperty(MessageItemSchema.MessageInConflict);
			base.AddProperty(ItemSchema.IsResponseRequested);
			base.AddProperty(MessageItemSchema.ReceivedRepresentingEmailAddress);
			base.AddProperty(CalendarItemBaseSchema.OrganizerEmailAddress);
			base.AddProperty(ItemSchema.EdgePcl);
			base.AddProperty(ItemSchema.LinkEnabled);
			base.AddProperty(StoreObjectSchema.IsRestricted);
			base.AddProperty(MessageItemSchema.DRMRights);
			base.AddProperty(MessageItemSchema.RequireProtectedPlayOnPhone);
		}

		protected override void ValidatedRender(TextWriter writer, int startRange, int endRange)
		{
			bool showFlag = this.ViewDescriptor.ContainsColumn(ColumnId.FlagDueDate);
			bool showCategories = this.ViewDescriptor.ContainsColumn(ColumnId.Categories);
			writer.Write("<div class=\"baseIL multiIL\" id=\"");
			writer.Write("divVLVIL");
			writer.Write("\">");
			this.DataSource.MoveToItem(startRange);
			while (this.DataSource.CurrentItem <= endRange)
			{
				this.RenderRow(writer, showFlag, showCategories, ListViewContents2.ListViewRowType.RenderAllRows, true);
				this.DataSource.MoveNext();
			}
			writer.Write("</div>");
		}

		internal override void RenderRow(TextWriter writer, bool showFlag, ListViewContents2.ListViewRowType rowTypeToRender, bool renderContainer)
		{
			this.RenderRow(writer, showFlag, true, rowTypeToRender, renderContainer);
		}

		private void RenderRow(TextWriter writer, bool showFlag, bool showCategories, ListViewContents2.ListViewRowType rowTypeToRender, bool renderContainer)
		{
			if (rowTypeToRender != ListViewContents2.ListViewRowType.RenderOnlyRow2)
			{
				if (renderContainer)
				{
					writer.Write("<div id=\"");
					writer.Write("vr");
					writer.Write("\"");
					base.RenderItemTooltip(writer);
					bool itemProperty = this.DataSource.GetItemProperty<bool>(MessageItemSchema.IsRead, true);
					writer.Write(" class=\"r1");
					if (!itemProperty)
					{
						writer.Write(" ur");
					}
					writer.Write("\">");
				}
				writer.Write("<div class=\"cData\"");
				ItemList2.RenderRowId(writer, this.DataSource.GetItemId());
				ItemList2.RenderInstanceKey(writer, Convert.ToBase64String(this.DataSource.GetItemProperty<byte[]>(ItemSchema.InstanceKey)));
				base.RenderMessageViewItemMetaDataExpandos(writer);
				writer.Write("></div><div class=\"c1\">");
				base.RenderColumn(writer, ColumnId.MailIcon);
				writer.Write("</div>");
				writer.Write("<div id=\"divSenderList\" class=\"c2 pd nowrap\">");
				if (base.Properties.ContainsKey(ItemSchema.SentRepresentingDisplayName))
				{
					base.RenderColumn(writer, ColumnId.From);
				}
				else if (base.Properties.ContainsKey(ItemSchema.DisplayTo))
				{
					base.RenderColumn(writer, ColumnId.To);
				}
				writer.Write("</div>");
				if (showFlag)
				{
					FlagStatus itemProperty2 = this.DataSource.GetItemProperty<FlagStatus>(ItemSchema.FlagStatus, FlagStatus.NotFlagged);
					writer.Write("<div id=\"");
					writer.Write("divFlg");
					writer.Write("\" class=\"c5");
					if (itemProperty2 != FlagStatus.NotFlagged)
					{
						writer.Write(" stky");
					}
					writer.Write("\">");
					base.RenderColumn(writer, ColumnId.FlagDueDate);
					writer.Write("</div>");
				}
				if (showCategories)
				{
					string[] itemProperty3 = this.DataSource.GetItemProperty<string[]>(ItemSchema.Categories, null);
					writer.Write("<div class=\"c4 taR");
					if (itemProperty3 != null && itemProperty3.Length > 0)
					{
						writer.Write(" stky");
					}
					writer.Write("\" id=\"");
					writer.Write("divCat");
					writer.Write("\">");
					base.RenderColumn(writer, ColumnId.Categories);
					writer.Write("</div>");
				}
				writer.Write("<div class=\"c3 pl nowrap\">");
				bool flag = base.RenderColumn(writer, ColumnId.Importance, false);
				base.RenderColumn(writer, ColumnId.HasAttachment, !flag);
				writer.Write("</div>");
				base.RenderSelectionImage(writer);
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
					writer.Write("<div id=\"");
					writer.Write("sr");
					writer.Write("\" class=\"r2\">");
				}
				writer.Write("<div id=\"divSubject\" class=\"c2 pd nowrap\">");
				base.RenderColumn(writer, ColumnId.Subject);
				writer.Write("</div>");
				writer.Write("<div class=\"nowrap taR c3\">");
				if (base.Properties.ContainsKey(ItemSchema.ReceivedTime))
				{
					base.RenderColumn(writer, ColumnId.DeliveryTime);
				}
				else if (base.Properties.ContainsKey(ItemSchema.SentTime))
				{
					base.RenderColumn(writer, ColumnId.SentTime);
				}
				else if (base.Properties.ContainsKey(StoreObjectSchema.LastModifiedTime))
				{
					base.RenderColumn(writer, ColumnId.DeletedOnTime);
				}
				writer.Write("</div>");
				base.RenderSelectionImage(writer);
				if (renderContainer)
				{
					writer.Write("</div>");
				}
			}
		}

		protected override bool RenderIcon(TextWriter writer)
		{
			return base.RenderMessageIcon(writer);
		}
	}
}
