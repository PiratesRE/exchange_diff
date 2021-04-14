using System;
using System.Globalization;
using System.IO;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class ColumnHeaders : LegacyListViewHeaders
	{
		public ColumnHeaders(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext) : base(sortedColumn, sortOrder, userContext)
		{
			this.viewDescriptor = viewDescriptor;
		}

		protected override void RenderHeaders(TextWriter writer)
		{
			writer.Write("<table id=\"");
			writer.Write("tblH");
			writer.Write("\"");
			if (this.viewDescriptor.IsFixedWidth)
			{
				writer.Write(" style=\"width:");
				writer.Write(this.viewDescriptor.Width);
				writer.Write("em\"");
				writer.Write(" fw=\"1\"");
			}
			writer.Write(" class=\"cHdr\" cellspacing=\"0\" cellpadding=\"1\"><tr>");
			for (int i = 0; i < this.viewDescriptor.ColumnCount; i++)
			{
				ColumnId column = this.viewDescriptor.GetColumn(i);
				Column column2 = ListViewColumns.GetColumn(column);
				writer.Write("<td id=\"col\" class=\"");
				if (column2 == base.SortedColumn)
				{
					writer.Write("hcs");
				}
				else
				{
					writer.Write("hc");
				}
				if (!column2.Header.IsImageHeader)
				{
					writer.Write(" d");
				}
				if (column2.Header.IsImageHeader)
				{
					writer.Write(" c");
				}
				else if (column2.HorizontalAlign != HorizontalAlign.NotSet)
				{
					switch (column2.HorizontalAlign)
					{
					case HorizontalAlign.Center:
						writer.Write(" c");
						break;
					case HorizontalAlign.Right:
						writer.Write(" r");
						break;
					}
				}
				writer.Write("\"");
				writer.Write(" cId=\"");
				writer.Write((int)column);
				writer.Write("\"");
				if (column2.IsSortable)
				{
					writer.Write(" sO=\"");
					if ((column2 == base.SortedColumn && base.SortOrder == SortOrder.Ascending) || (column2 != base.SortedColumn && column2.DefaultSortOrder == SortOrder.Descending))
					{
						writer.Write(LegacyListViewHeaders.SortDescending);
					}
					else
					{
						writer.Write(LegacyListViewHeaders.SortAscending);
					}
					writer.Write("\"");
					writer.Write(" tD=");
					writer.Write(column2.IsTypeDownCapable ? "1" : "0");
				}
				writer.Write(" style=\"width:");
				writer.Write(this.viewDescriptor.GetColumnWidth(i).ToString(CultureInfo.InvariantCulture));
				if (this.viewDescriptor.IsFixedWidth || !column2.IsFixedWidth)
				{
					writer.Write("%");
				}
				else
				{
					writer.Write("px");
				}
				writer.Write(";\"");
				if (!column2.Header.IsImageHeader)
				{
					writer.Write(" nowrap");
				}
				writer.Write(">");
				if (column2.Header.IsImageHeader)
				{
					this.userContext.RenderThemeImage(writer, column2.Header.Image);
				}
				else
				{
					writer.Write(LocalizedStrings.GetHtmlEncoded(column2.Header.TextID));
				}
				if (column2 == base.SortedColumn && !column2.Header.IsImageHeader)
				{
					writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;");
					base.RenderSortIcon(writer);
				}
				writer.Write("</td>");
			}
			writer.Write("</tr>");
			writer.Write("</table>");
		}

		private ViewDescriptor viewDescriptor;
	}
}
