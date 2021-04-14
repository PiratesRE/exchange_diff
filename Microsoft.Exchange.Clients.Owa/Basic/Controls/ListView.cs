using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public abstract class ListView
	{
		protected ListView(UserContext userContext, ColumnId sortedColumn, SortOrder sortOrder, ListView.ViewType viewType)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			this.userContext = userContext;
			this.sortedColumn = sortedColumn;
			this.sortOrder = sortOrder;
			this.viewType = viewType;
		}

		protected UserContext UserContext
		{
			get
			{
				return this.userContext;
			}
		}

		protected ViewDescriptor ViewDescriptor
		{
			get
			{
				return this.viewDescriptor;
			}
			set
			{
				this.viewDescriptor = value;
			}
		}

		protected ListViewContents Contents
		{
			get
			{
				return this.contents;
			}
			set
			{
				this.contents = value;
			}
		}

		protected SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		protected ColumnId SortedColumn
		{
			get
			{
				return this.sortedColumn;
			}
		}

		internal ListViewDataSource DataSource
		{
			get
			{
				return this.dataSource;
			}
			set
			{
				this.dataSource = value;
			}
		}

		public int StartRange
		{
			get
			{
				if (this.dataSource == null)
				{
					return int.MinValue;
				}
				return this.dataSource.StartRange;
			}
		}

		public int EndRange
		{
			get
			{
				if (this.dataSource == null)
				{
					return int.MinValue;
				}
				return this.dataSource.EndRange;
			}
		}

		public int RangeCount
		{
			get
			{
				if (this.dataSource != null)
				{
					return this.dataSource.RangeCount;
				}
				return 0;
			}
		}

		public int TotalCount
		{
			get
			{
				if (this.dataSource != null)
				{
					return this.dataSource.TotalCount;
				}
				return 0;
			}
		}

		protected bool FilteredView
		{
			get
			{
				return this.filteredView;
			}
			set
			{
				this.filteredView = value;
			}
		}

		protected ListView(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			this.userContext = userContext;
		}

		protected abstract void InitializeListViewContents();

		protected abstract void InitializeDataSource();

		public virtual void Initialize(int startRange, int endRange)
		{
			if (startRange < 1)
			{
				throw new ArgumentOutOfRangeException("startRange", "startRange must be greater than or equal to 1");
			}
			if (endRange < startRange)
			{
				throw new ArgumentOutOfRangeException("endRange", "endRange must be greater than or equal to startRange");
			}
			this.PerformInitialization();
			this.dataSource.LoadData(startRange, endRange);
		}

		internal int Initialize(StoreObjectId storeObjectId, int itemsPerPage)
		{
			if (storeObjectId == null)
			{
				throw new ArgumentNullException("storeObjectId");
			}
			if (itemsPerPage <= 0)
			{
				throw new ArgumentOutOfRangeException("itemsPerPage", "itemsPerPage has to be greater than zero");
			}
			this.PerformInitialization();
			return this.dataSource.LoadData(storeObjectId, itemsPerPage);
		}

		public void Render(TextWriter writer, string errorMessage)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (errorMessage == null)
			{
				throw new ArgumentNullException("errorMessage");
			}
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ListView.Render");
			writer.Write("<table class=\"lvw\" cellpadding=0 cellspacing=0><caption>");
			writer.Write(LocalizedStrings.GetHtmlEncoded(573876176));
			writer.Write("</caption>");
			if (errorMessage.Length != 0)
			{
				writer.Write("<tr><td colspan=8 class=\"errMsg\"><img src=\"");
				this.userContext.RenderThemeFileUrl(writer, ThemeFileId.Error);
				writer.Write("\">");
				writer.Write(errorMessage);
				writer.Write("</td></tr>");
			}
			ExTraceGlobals.MailTracer.TraceDebug((long)this.GetHashCode(), "Rendering ListView headers");
			ListViewHeaders listViewHeaders = new ListViewHeaders(this.viewDescriptor, this.sortedColumn, this.sortOrder, this.userContext, this.viewType);
			listViewHeaders.Render(writer);
			ExTraceGlobals.MailTracer.TraceDebug((long)this.GetHashCode(), "Rendering ListView contents");
			if (this.dataSource.RangeCount > 0)
			{
				this.contents.DataSource = this.dataSource;
				this.contents.Render(writer);
			}
			else
			{
				writer.Write("<tr><td class=\"ni\" colspan=");
				writer.Write(this.viewDescriptor.ColumnCount);
				writer.Write(" align=\"center\" valign=\"middle\"><br>");
				writer.Write(LocalizedStrings.GetHtmlEncoded(this.filteredView ? 417836457 : -474826895));
				writer.Write("<br><br></td></tr>");
			}
			writer.Write("</table>");
		}

		public static void RenderPageNumbers(TextWriter writer, int pageNumber, int totalNumberOfPages)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (pageNumber < 1)
			{
				throw new ArgumentOutOfRangeException("pageNumber", "pageNumber must be greater than or equal to 1");
			}
			if (totalNumberOfPages < 0)
			{
				throw new ArgumentOutOfRangeException("totalNumberOfPages", "totalNumberOfPages must be greater than or equal to 0");
			}
			int num;
			if (pageNumber % 5 == 0)
			{
				num = pageNumber / 5;
			}
			else
			{
				num = pageNumber / 5 + 1;
			}
			int num2 = (num - 1) * 5 + 1;
			int num3 = num2 + 5 - 1;
			if (num3 > totalNumberOfPages)
			{
				num3 = totalNumberOfPages;
			}
			if (num3 >= num2)
			{
				writer.Write("<td class=\"pl\" nowrap>");
				writer.Write(LocalizedStrings.GetHtmlEncoded(-2042236200));
				writer.Write("</td>");
			}
			if (num > 1)
			{
				ListView.RenderPageLink(writer, (num - 1) * 5, true);
			}
			for (int i = num2; i <= num3; i++)
			{
				if (i != pageNumber)
				{
					ListView.RenderPageLink(writer, i, false);
				}
				else
				{
					writer.Write("<td class=\"pTxt\">");
					writer.Write(i);
					writer.Write("</td>");
				}
			}
			if (totalNumberOfPages % 5 == 0)
			{
				if (num < totalNumberOfPages / 5)
				{
					ListView.RenderPageLink(writer, num * 5 + 1, true);
					return;
				}
			}
			else if (num < totalNumberOfPages / 5 + 1)
			{
				ListView.RenderPageLink(writer, num * 5 + 1, true);
			}
		}

		internal void RenderHeaderPagingControls(TextWriter writer, int pageNumber, int totalNumberOfPages)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (pageNumber < 1)
			{
				throw new ArgumentOutOfRangeException("pageNumber", "pageNumber must be greater than or equal to 1");
			}
			if (totalNumberOfPages < 0)
			{
				throw new ArgumentOutOfRangeException("totalNumberOfPages", "totalNumberOfPages must be greater than or equal to 0");
			}
			if (pageNumber > 1)
			{
				this.RenderPageControlImage(writer, ThemeFileId.FirstPage, 1, -946066775, "lnkFrstPgHdr");
				this.RenderPageControlImage(writer, ThemeFileId.PreviousPage, pageNumber - 1, -1907861992, "lnkPrvPgHdr");
			}
			else
			{
				this.RenderPageControlImage(writer, ThemeFileId.FirstPageGray, 0, -946066775, "lnkFrstPgHdr");
				this.RenderPageControlImage(writer, ThemeFileId.PreviousPageGray, 0, -1907861992, "lnkPrvPgHdr");
			}
			if (pageNumber < totalNumberOfPages)
			{
				this.RenderPageControlImage(writer, ThemeFileId.NextPage, pageNumber + 1, 1548165396, "lnkNxtPgHdr");
				this.RenderPageControlImage(writer, ThemeFileId.LastPage, totalNumberOfPages, -991618511, "lnkLstPgHdr");
				return;
			}
			this.RenderPageControlImage(writer, ThemeFileId.NextPageGray, 0, 1548165396, "lnkNxtPgHdr");
			this.RenderPageControlImage(writer, ThemeFileId.LastPageGray, 0, -991618511, "lnkLstPgHdr");
		}

		public void RenderPagingControls(TextWriter writer, int pageNumber, int totalNumberOfPages)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (pageNumber < 1)
			{
				throw new ArgumentOutOfRangeException("pageNumber", "pageNumber must be greater than or equal to 1");
			}
			if (totalNumberOfPages < 0)
			{
				throw new ArgumentOutOfRangeException("totalNumberOfPages", "totalNumberOfPages must be greater than or equal to 0");
			}
			if (pageNumber > 1)
			{
				this.RenderPageControlImage(writer, ThemeFileId.FirstPage, 1, -946066775, "lnkFrstPg");
				this.RenderPageControlImage(writer, ThemeFileId.PreviousPage, pageNumber - 1, -1907861992, "lnkPrvPg");
			}
			else
			{
				this.RenderPageControlImage(writer, ThemeFileId.FirstPageGray, 0, -946066775, "lnkFrstPg");
				this.RenderPageControlImage(writer, ThemeFileId.PreviousPageGray, 0, -1907861992, "lnkPrvPg");
			}
			if (pageNumber < totalNumberOfPages)
			{
				this.RenderPageControlImage(writer, ThemeFileId.NextPage, pageNumber + 1, 1548165396, "lnkNxtPg");
				this.RenderPageControlImage(writer, ThemeFileId.LastPage, totalNumberOfPages, -991618511, "lnkLstPg");
				return;
			}
			this.RenderPageControlImage(writer, ThemeFileId.NextPageGray, 0, 1548165396, "lnkNxtPg");
			this.RenderPageControlImage(writer, ThemeFileId.LastPageGray, 0, -991618511, "lnkLstPg");
		}

		private static void RenderPageLink(TextWriter writer, int pageNumber, bool isPageSetLink)
		{
			writer.Write("<td class=\"pTxt\"><a href=\"#\" id=\"lnkPgNm");
			writer.Write(pageNumber);
			writer.Write("\" onClick=\"return onClkPg('");
			writer.Write(pageNumber);
			writer.Write("');\">");
			if (isPageSetLink)
			{
				writer.Write("...");
			}
			else
			{
				writer.Write(pageNumber);
			}
			writer.Write("</a></td>");
		}

		private void RenderPageControlImage(TextWriter writer, ThemeFileId image, int pageNumber, Strings.IDs title, string controlId)
		{
			writer.Write("<td class=\"pImg\">");
			if (pageNumber > 0)
			{
				writer.Write("<a href=\"#\" id=\"");
				writer.Write(controlId);
				writer.Write("\" onClick=\"return onClkPg('{0}');\">", pageNumber);
			}
			writer.Write("<img border=\"0\" src=\"");
			this.userContext.RenderThemeFileUrl(writer, image);
			writer.Write("\" title=\"");
			writer.Write(LocalizedStrings.GetHtmlEncoded(title));
			writer.Write("\" ");
			Utilities.RenderImageAltAttribute(writer, this.UserContext, image);
			writer.Write(">");
			if (pageNumber > 0)
			{
				writer.Write("</a>");
			}
			writer.Write("</td>");
		}

		private void PerformInitialization()
		{
			this.InitializeListViewContents();
			this.InitializeDataSource();
		}

		internal void RenderADContentsHeaderPaging(TextWriter writer, int pageNumber)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (pageNumber > 1)
			{
				this.RenderPageControlImage(writer, ThemeFileId.PreviousPage, pageNumber - 1, -1907861992, "lnkPrvPgHdr");
			}
			else
			{
				this.RenderPageControlImage(writer, ThemeFileId.PreviousPageGray, 0, -1907861992, "lnkPrvPgHdr");
			}
			if (string.IsNullOrEmpty(this.dataSource.Cookie))
			{
				this.RenderPageControlImage(writer, ThemeFileId.NextPageGray, 0, 1548165396, "lnkNxtPgHdr");
				return;
			}
			this.RenderPageControlImage(writer, ThemeFileId.NextPage, pageNumber + 1, 1548165396, "lnkNxtPgHdr");
		}

		internal void RenderADContentsPaging(TextWriter writer, int pageNumber)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (pageNumber > 1)
			{
				this.RenderPageControlImage(writer, ThemeFileId.PreviousPage, pageNumber - 1, -1907861992, "lnkPrvPg");
			}
			else
			{
				this.RenderPageControlImage(writer, ThemeFileId.PreviousPageGray, 0, -1907861992, "lnkPrvPg");
			}
			if (string.IsNullOrEmpty(this.dataSource.Cookie))
			{
				this.RenderPageControlImage(writer, ThemeFileId.NextPageGray, 0, 1548165396, "lnkNxtPg");
				return;
			}
			this.RenderPageControlImage(writer, ThemeFileId.NextPage, pageNumber + 1, 1548165396, "lnkNxtPg");
		}

		private const string PreviousPageHeaderId = "lnkPrvPgHdr";

		private const string NextPageHeaderId = "lnkNxtPgHdr";

		private const string FirstPageHeaderId = "lnkFrstPgHdr";

		private const string LastPageHeaderId = "lnkLstPgHdr";

		private const string PreviousPageId = "lnkPrvPg";

		private const string NextPageId = "lnkNxtPg";

		private const string FirstPageId = "lnkFrstPg";

		private const string LastPageId = "lnkLstPg";

		private ListView.ViewType viewType;

		private ListViewContents contents;

		private ViewDescriptor viewDescriptor;

		private SortOrder sortOrder;

		private ColumnId sortedColumn;

		private ListViewDataSource dataSource;

		private UserContext userContext;

		private bool filteredView;

		public enum ViewType
		{
			None,
			MessageListView,
			ADContentsListView,
			ContactsListView
		}
	}
}
