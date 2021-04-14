using System;
using System.Globalization;
using System.IO;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class ListViewHeaders
	{
		public ListViewHeaders(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext, ListView.ViewType viewType)
		{
			if (viewDescriptor == null)
			{
				throw new ArgumentNullException("viewDescriptor");
			}
			if (sortedColumn < ColumnId.MailIcon)
			{
				throw new ArgumentOutOfRangeException("sortedColumn", "sortedColumn must not be less than zero");
			}
			if (sortOrder < SortOrder.Ascending || sortOrder > SortOrder.Descending)
			{
				throw new ArgumentOutOfRangeException("sortOrder", "sortOrder must be either 0 or 1");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			this.viewDescriptor = viewDescriptor;
			this.sortedColumn = ListViewColumns.GetColumn(sortedColumn);
			this.sortOrder = sortOrder;
			this.userContext = userContext;
			this.viewType = viewType;
		}

		private Column SortedColumn
		{
			get
			{
				return this.sortedColumn;
			}
		}

		private SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		private UserContext UserContext
		{
			get
			{
				return this.userContext;
			}
		}

		public void Render(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.RenderHeaders(writer);
		}

		private void RenderHeaders(TextWriter writer)
		{
			writer.Write("<tr>");
			int columnCount = this.viewDescriptor.ColumnCount;
			for (int i = 0; i < columnCount; i++)
			{
				ColumnId column = this.viewDescriptor.GetColumn(i);
				Column column2 = ListViewColumns.GetColumn(column);
				writer.Write("<th");
				switch (column2.HorizontalAlign)
				{
				case HorizontalAlign.Center:
					writer.Write(" align=\"center\"");
					break;
				case HorizontalAlign.Right:
					writer.Write(" align=\"right\"");
					break;
				default:
					writer.Write(" align=\"left\"");
					break;
				}
				writer.Write(" class=\"");
				if (column2 == this.SortedColumn)
				{
					writer.Write("shd");
				}
				else
				{
					writer.Write("chd");
				}
				if (i == 0)
				{
					writer.Write(" lt");
				}
				else if (i == columnCount - 1)
				{
					writer.Write(" rt");
				}
				writer.Write("\"");
				writer.Write(" style=\"width:");
				writer.Write(this.viewDescriptor.GetColumnWidth(i).ToString(CultureInfo.InvariantCulture));
				if (this.viewDescriptor.IsFixedWidth || !column2.IsFixedWidth)
				{
					writer.Write("%");
				}
				writer.Write(";\"");
				if (!column2.Header.IsImageHeader || !column2.Header.IsCheckBoxHeader)
				{
					writer.Write(" nowrap");
				}
				writer.Write(">");
				if ((this.viewType == ListView.ViewType.MessageListView || this.viewType == ListView.ViewType.ContactsListView) && !column2.Header.IsCheckBoxHeader && column != ColumnId.ContactIcon)
				{
					writer.Write("<a href=\"#\" id=\"lnkCol");
					writer.Write(column);
					writer.Write("\" onClick=\"return onClkSrt('{0}','", (int)column);
					if ((column2 == this.SortedColumn && this.SortOrder == SortOrder.Ascending) || (column2 != this.SortedColumn && column2.DefaultSortOrder == SortOrder.Descending))
					{
						writer.Write(ListViewHeaders.SortDescending);
					}
					else
					{
						writer.Write(ListViewHeaders.SortAscending);
					}
					writer.Write("');\" title=\"");
					if (!column2.Header.IsImageHeader && !column2.Header.IsCheckBoxHeader)
					{
						writer.Write(string.Format(LocalizedStrings.GetHtmlEncoded(691069624), LocalizedStrings.GetHtmlEncoded(column2.Header.TextID)));
					}
					writer.Write("\">");
				}
				if (column2.Header.IsImageHeader)
				{
					writer.Write("<img src=\"");
					this.userContext.RenderThemeFileUrl(writer, column2.Header.Image);
					writer.Write("\" class=\"");
					ColumnId id = column2.Id;
					if (id != ColumnId.MailIcon)
					{
						switch (id)
						{
						case ColumnId.HasAttachment:
							writer.Write("attch\" alt=\"");
							writer.Write(string.Format(LocalizedStrings.GetHtmlEncoded(691069624), LocalizedStrings.GetHtmlEncoded(796893232)));
							break;
						case ColumnId.Importance:
							writer.Write("ih\" alt=\"");
							writer.Write(string.Format(LocalizedStrings.GetHtmlEncoded(691069624), LocalizedStrings.GetHtmlEncoded(-731116444)));
							break;
						}
					}
					else
					{
						writer.Write("eml\" alt=\"");
						writer.Write(string.Format(LocalizedStrings.GetHtmlEncoded(691069624), LocalizedStrings.GetHtmlEncoded(635522515)));
					}
					writer.Write("\">");
				}
				else if (column2.Header.IsCheckBoxHeader)
				{
					writer.Write("<input type=\"checkbox\" name=\"chkhd\" onClick=\"onClkSlAll();\" title=\"");
					writer.Write(LocalizedStrings.GetHtmlEncoded(-1899576627));
					writer.Write("\">");
				}
				else
				{
					writer.Write(LocalizedStrings.GetHtmlEncoded(column2.Header.TextID));
				}
				if (column2 == this.SortedColumn && !column2.Header.IsImageHeader)
				{
					writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;");
					this.RenderSortIcon(writer);
				}
				writer.Write("&nbsp;");
				if ((this.viewType == ListView.ViewType.MessageListView || this.viewType == ListView.ViewType.ContactsListView) && !column2.Header.IsCheckBoxHeader)
				{
					writer.Write("</a>");
				}
				writer.Write("</th>");
			}
			writer.Write("</tr>");
			writer.Write("<tr><td class=\"ohdv\" colspan={0}>", columnCount);
			RenderingUtilities.RenderHorizontalDivider(this.UserContext, writer);
			writer.Write("</td></tr>");
		}

		private void RenderSortIcon(TextWriter writer)
		{
			writer.Write("<img class=\"srt\" src=\"");
			switch (this.sortOrder)
			{
			case SortOrder.Ascending:
				this.userContext.RenderThemeFileUrl(writer, ThemeFileId.BasicSortAscending);
				writer.Write("\" alt=\"");
				writer.Write(LocalizedStrings.GetHtmlEncoded(-904532892));
				goto IL_76;
			}
			this.userContext.RenderThemeFileUrl(writer, ThemeFileId.BasicSortDescending);
			writer.Write("\" alt=\"");
			writer.Write(LocalizedStrings.GetHtmlEncoded(1777112844));
			IL_76:
			writer.Write("\">&nbsp;");
		}

		private static readonly string SortAscending = 0.ToString(CultureInfo.InvariantCulture);

		private static readonly string SortDescending = 1.ToString(CultureInfo.InvariantCulture);

		private Column sortedColumn;

		private SortOrder sortOrder;

		private ViewDescriptor viewDescriptor;

		private UserContext userContext;

		private ListView.ViewType viewType;
	}
}
