using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class ArrangeByHeaders2 : ListViewHeaders2
	{
		public ArrangeByHeaders2(ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext) : base(sortedColumn, sortOrder, userContext)
		{
		}

		protected override void RenderHeaders(TextWriter writer)
		{
			string value;
			Strings.IDs localizedID;
			switch (base.SortOrder)
			{
			case SortOrder.Ascending:
				value = ListViewHeaders2.SortDescending;
				localizedID = base.SortedColumn.SortBoundaries.AscendingID;
				goto IL_47;
			}
			value = ListViewHeaders2.SortAscending;
			localizedID = base.SortedColumn.SortBoundaries.DescendingID;
			IL_47:
			writer.Write("<table id=\"");
			writer.Write("tblH");
			writer.Write("\" class=\"abHdr\" cellspacing=\"0\" cellpadding=\"1\" dir=\"");
			writer.Write(base.UserContext.IsRtl ? "rtl" : "ltr");
			writer.Write("\">");
			writer.Write("<tr>");
			writer.Write("<td nowrap id=\"tdAB\">");
			Strings.IDs localizedID2 = -1809061181;
			if (ConversationUtilities.IsConversationSortColumn(base.SortedColumn.Id))
			{
				localizedID2 = 1872724609;
			}
			writer.Write(string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetHtmlEncoded(localizedID2), new object[]
			{
				LocalizedStrings.GetHtmlEncoded(base.SortedColumn.SortBoundaries.TextID)
			}));
			writer.Write("&nbsp;&nbsp;");
			this.userContext.RenderThemeImage(writer, ThemeFileId.DownButton3, null, new object[0]);
			writer.Write("</td>");
			writer.Write("<td id=\"tdSO\" class=\"v ");
			writer.Write(base.UserContext.IsRtl ? "l" : "r");
			writer.Write("\" nowrap sO=\"");
			writer.Write(value);
			writer.Write("\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(localizedID));
			writer.Write("</td>");
			writer.Write("</tr>");
			writer.Write("</table>");
		}
	}
}
