using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal abstract class LegacyListViewHeaders
	{
		protected LegacyListViewHeaders(ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext)
		{
			this.sortedColumn = ListViewColumns.GetColumn(sortedColumn);
			this.sortOrder = sortOrder;
			this.userContext = userContext;
		}

		protected Column SortedColumn
		{
			get
			{
				return this.sortedColumn;
			}
		}

		protected SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		protected UserContext UserContext
		{
			get
			{
				return this.userContext;
			}
		}

		internal void Render(TextWriter writer)
		{
			this.RenderHeaders(writer);
		}

		protected abstract void RenderHeaders(TextWriter writer);

		protected void RenderSortIcon(TextWriter writer)
		{
			switch (this.sortOrder)
			{
			case SortOrder.Ascending:
				this.userContext.RenderThemeImage(writer, ThemeFileId.SortAscending, null, new object[]
				{
					"style=\"vertical-align:middle;\""
				});
				goto IL_5D;
			}
			this.userContext.RenderThemeImage(writer, ThemeFileId.SortDescending, null, new object[]
			{
				"style=\"vertical-align:middle;\""
			});
			IL_5D:
			writer.Write("&nbsp;");
		}

		protected const string HeadersTableId = "tblH";

		protected static readonly string SortAscending = 0.ToString(CultureInfo.InvariantCulture);

		protected static readonly string SortDescending = 1.ToString(CultureInfo.InvariantCulture);

		private Column sortedColumn;

		private SortOrder sortOrder;

		protected UserContext userContext;
	}
}
