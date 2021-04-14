using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class SingleLineItemContentsForVirtualList : ItemList2
	{
		protected virtual bool IsRenderColumnShadow
		{
			get
			{
				return true;
			}
		}

		public SingleLineItemContentsForVirtualList(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext) : this(viewDescriptor, sortedColumn, sortOrder, userContext, SearchScope.SelectedFolder)
		{
		}

		public SingleLineItemContentsForVirtualList(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext, SearchScope folderScope) : base(viewDescriptor, sortedColumn, sortOrder, userContext, folderScope)
		{
		}

		protected override void ValidatedRender(TextWriter writer, int startRange, int endRange)
		{
			writer.Write("<div class=\"baseIL\" id=\"");
			writer.Write("divVLVIL");
			writer.Write("\">");
			this.DataSource.MoveToItem(startRange);
			while (this.DataSource.CurrentItem <= endRange)
			{
				writer.Write("<div class=\"listItemRow");
				this.RenderItemRowStyle(writer);
				writer.Write("\"");
				base.RenderItemTooltip(writer);
				writer.Write(" id=\"");
				writer.Write(base.IsForVirtualListView ? "vr" : "us");
				writer.Write("\">");
				writer.Write("<div class=\"cData\"");
				ItemList2.RenderRowId(writer, this.DataSource.GetItemId());
				this.RenderItemMetaDataExpandos(writer);
				writer.Write("></div>");
				for (int i = 0; i < this.ViewDescriptor.ColumnCount; i++)
				{
					ColumnId column = this.ViewDescriptor.GetColumn(i);
					writer.Write("<div class=\"listColumn ");
					writer.Write(this.ColumnClassPrefix);
					writer.Write(column.ToString());
					writer.Write("\"");
					this.RenderTableCellAttributes(writer, column);
					if (column == ColumnId.FlagDueDate || column == ColumnId.ContactFlagDueDate || column == ColumnId.TaskFlag)
					{
						writer.Write(" id=");
						writer.Write("divFlg");
					}
					if (column == ColumnId.Categories || column == ColumnId.ContactCategories)
					{
						writer.Write(" id=");
						writer.Write("divCat");
					}
					writer.Write(">");
					base.RenderColumn(writer, column, false);
					writer.Write("</div>");
				}
				base.RenderSelectionImage(writer);
				base.RenderRowDivider(writer);
				writer.Write("</div>");
				this.DataSource.MoveNext();
			}
			writer.Write("</div>");
		}

		protected virtual void RenderItemRowStyle(TextWriter writer)
		{
		}

		protected virtual void RenderTableCellAttributes(TextWriter writer, ColumnId columnId)
		{
		}

		protected virtual string ColumnClassPrefix
		{
			get
			{
				return string.Empty;
			}
		}
	}
}
