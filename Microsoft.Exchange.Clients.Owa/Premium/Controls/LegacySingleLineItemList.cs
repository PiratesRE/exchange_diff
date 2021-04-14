using System;
using System.Globalization;
using System.IO;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class LegacySingleLineItemList : LegacyItemList
	{
		protected virtual bool IsRenderColumnShadow
		{
			get
			{
				return true;
			}
		}

		protected virtual string ListViewStyle
		{
			get
			{
				return "slIL";
			}
		}

		public LegacySingleLineItemList(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext) : this(viewDescriptor, sortedColumn, sortOrder, userContext, SearchScope.SelectedFolder)
		{
		}

		public LegacySingleLineItemList(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext, SearchScope folderScope) : base(viewDescriptor, sortedColumn, sortOrder, userContext, folderScope)
		{
		}

		protected override void ValidatedRender(TextWriter writer, int startRange, int endRange)
		{
			writer.Write("<table class=\"");
			writer.Write(this.ListViewStyle);
			writer.Write("\" id=\"");
			writer.Write("tblIL");
			writer.Write("\"");
			if (this.ViewDescriptor.IsFixedWidth)
			{
				writer.Write(" style=\"width:");
				writer.Write(this.ViewDescriptor.Width);
				writer.Write("em\"");
			}
			writer.Write(">");
			writer.Write("<col style=\"width:0px\">");
			for (int i = 0; i < this.ViewDescriptor.ColumnCount; i++)
			{
				ColumnId column = this.ViewDescriptor.GetColumn(i);
				Column column2 = ListViewColumns.GetColumn(column);
				writer.Write("<col style=\"width:");
				writer.Write(this.ViewDescriptor.GetColumnWidth(i).ToString(CultureInfo.InvariantCulture));
				if (this.ViewDescriptor.IsFixedWidth || !column2.IsFixedWidth)
				{
					writer.Write("%");
				}
				else
				{
					writer.Write("px");
				}
				writer.Write(";\"");
				writer.Write(" class=\"");
				switch (column2.HorizontalAlign)
				{
				case HorizontalAlign.Center:
					writer.Write("c");
					break;
				case HorizontalAlign.Right:
					writer.Write("r d");
					break;
				default:
					writer.Write(" d");
					break;
				}
				if (this.IsRenderColumnShadow && base.SortedColumn.Id == column)
				{
					writer.Write(" s");
				}
				writer.Write("\"");
				writer.Write(">");
			}
			this.DataSource.MoveToItem(startRange);
			while (this.DataSource.CurrentItem <= endRange)
			{
				writer.Write("<tr");
				base.RenderItemTooltip(writer);
				this.RenderItemRowStyle(writer);
				writer.Write(" id=\"");
				writer.Write(base.IsForVirtualListView ? "vr" : "us");
				writer.Write("\"><td style=\"width:0px\"");
				this.RenderItemMetaDataExpandos(writer);
				writer.Write("></td>");
				this.RenderInnerRowContainerStart(writer);
				for (int j = 0; j < this.ViewDescriptor.ColumnCount; j++)
				{
					ColumnId column3 = this.ViewDescriptor.GetColumn(j);
					writer.Write("<td nowrap");
					this.RenderTableCellAttributes(writer, column3);
					if (column3 == ColumnId.FlagDueDate || column3 == ColumnId.ContactFlagDueDate || column3 == ColumnId.TaskFlag)
					{
						writer.Write(" id=");
						writer.Write("tdFlg");
					}
					if (column3 == ColumnId.Categories || column3 == ColumnId.ContactCategories)
					{
						writer.Write(" id=");
						writer.Write("tdCat");
					}
					writer.Write(">");
					base.RenderColumn(writer, column3);
					writer.Write("</td>");
				}
				this.RenderInnerRowContainerEnd(writer);
				writer.Write("</tr>");
				this.DataSource.MoveNext();
			}
			writer.Write("</table>");
		}

		protected virtual void RenderItemRowStyle(TextWriter writer)
		{
		}

		protected virtual void RenderInnerRowContainerStart(TextWriter writer)
		{
		}

		protected virtual void RenderInnerRowContainerEnd(TextWriter writer)
		{
		}

		protected virtual void RenderTableCellAttributes(TextWriter writer, ColumnId columnId)
		{
		}
	}
}
