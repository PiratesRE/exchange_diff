using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public abstract class ListView : IListView
	{
		protected ListView(UserContext userContext, ColumnId sortedColumn, SortOrder sortOrder, bool isFilteredView)
		{
			this.userContext = userContext;
			this.sortedColumn = sortedColumn;
			this.sortOrder = sortOrder;
			this.isFilteredView = isFilteredView;
		}

		protected ListView(UserContext userContext) : this(userContext, ColumnId.Count, SortOrder.Ascending, false)
		{
		}

		protected abstract ViewType ViewTypeId { get; }

		protected abstract string EventNamespace { get; }

		public virtual string Cookie
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual int CookieLcid
		{
			get
			{
				return Culture.GetUserCulture().LCID;
			}
		}

		public virtual string PreferredDC
		{
			get
			{
				return string.Empty;
			}
		}

		protected virtual bool IsSortable
		{
			get
			{
				return true;
			}
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

		protected LegacyListViewContents Contents
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

		protected virtual bool IsMultipleRequestAllowed
		{
			get
			{
				return true;
			}
		}

		public IListViewDataSource DataSource
		{
			get
			{
				return this.contents.DataSource;
			}
			set
			{
				if (!this.IsValidDataSource(value))
				{
					throw new ArgumentException("DataSource " + value + " cannot be used to render this view");
				}
				this.contents.DataSource = value;
			}
		}

		protected abstract bool IsValidDataSource(IListViewDataSource dataSource);

		public Hashtable Properties
		{
			get
			{
				return this.contents.Properties;
			}
		}

		public bool IsFilteredView
		{
			get
			{
				return this.isFilteredView;
			}
		}

		public int StartRange
		{
			get
			{
				if (this.DataSource == null)
				{
					return int.MinValue;
				}
				return this.DataSource.StartRange;
			}
		}

		public int EndRange
		{
			get
			{
				if (this.DataSource == null)
				{
					return int.MinValue;
				}
				return this.DataSource.EndRange;
			}
		}

		public int RangeCount
		{
			get
			{
				if (this.DataSource == null)
				{
					return 0;
				}
				return this.DataSource.RangeCount;
			}
		}

		public int TotalCount
		{
			get
			{
				if (this.DataSource == null)
				{
					return int.MinValue;
				}
				return this.DataSource.TotalCount;
			}
		}

		public int UnreadCount
		{
			get
			{
				if (this.DataSource == null)
				{
					return int.MinValue;
				}
				return this.DataSource.UnreadCount;
			}
		}

		protected abstract void InitializeListViewContents();

		protected void Initialize()
		{
			this.InitializeListViewContents();
		}

		public abstract SortBy[] GetSortByProperties();

		public void Render(TextWriter writer)
		{
			this.Render(writer, ListView.RenderFlags.All);
		}

		public void RenderForCompactWebPart(TextWriter writer)
		{
			this.Render(writer, ListView.RenderFlags.All);
		}

		public void Render(TextWriter writer, ListView.RenderFlags renderFlags)
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ListView.Render");
			if ((renderFlags & ListView.RenderFlags.Behavior) > (ListView.RenderFlags)0)
			{
				ExTraceGlobals.MailTracer.TraceDebug((long)this.GetHashCode(), "Rendering ListView behavior");
				writer.Write("<div id=\"divLstV\" tabIndex=\"0\" hidefocus=\"true\"");
				if (this.userContext.IsRtl)
				{
					ListView.RenderBehaviorAttribute(writer, "rtl", 1);
					ListView.RenderBehaviorAttribute(writer, "class", "rtl");
				}
				ListView.RenderBehaviorAttribute(writer, "L_Ldng", LocalizedStrings.GetNonEncoded(-695375226));
				ListView.RenderBehaviorAttribute(writer, "L_Srchng", LocalizedStrings.GetNonEncoded(-1057914178));
				ListView.RenderBehaviorAttribute(writer, "sSid", this.DataSource.ContainerId);
				ListView.RenderBehaviorAttribute(writer, "iT", (int)this.ViewTypeId);
				if ((renderFlags & ListView.RenderFlags.CompactWebPart) > (ListView.RenderFlags)0)
				{
					ListView.RenderBehaviorAttribute(writer, "iWP", 1);
				}
				ListView.RenderBehaviorAttribute(writer, "sEvtNS", this.EventNamespace);
				ListView.RenderBehaviorAttribute(writer, "iTC", this.DataSource.TotalCount);
				ListView.RenderBehaviorAttribute(writer, "iUC", this.DataSource.UnreadCount);
				ListView.RenderBehaviorAttribute(writer, "iRC", this.userContext.UserOptions.ViewRowCount);
				ListView.RenderBehaviorAttribute(writer, "iML", 0);
				ListView.RenderBehaviorAttribute(writer, "iSC", (int)this.sortedColumn);
				ListView.RenderBehaviorAttribute(writer, "iSO", (this.sortOrder == SortOrder.Ascending) ? ListView.sortAscending : ListView.sortDescending);
				ListView.RenderBehaviorAttribute(writer, "fQR", (!this.IsMultipleRequestAllowed) ? 1 : 0);
				Column column = ListViewColumns.GetColumn(this.sortedColumn);
				ListView.RenderBehaviorAttribute(writer, "iTD", column.IsTypeDownCapable ? 1 : 0);
				ListView.RenderBehaviorAttribute(writer, "fSrt", this.IsSortable ? 1 : 0);
				ListView.RenderBehaviorAttribute(writer, "iNsDir", (int)this.userContext.UserOptions.NextSelection);
				ListView.RenderBehaviorAttribute(writer, "sCki", this.Cookie);
				ListView.RenderBehaviorAttribute(writer, "iLcid", this.CookieLcid);
				ListView.RenderBehaviorAttribute(writer, "sPfdDC", this.PreferredDC);
				ListView.RenderBehaviorAttribute(writer, "fROLv", (this.ViewTypeId == ViewType.Documents) ? 1 : 0);
				foreach (string text in this.extraAttributes.Keys)
				{
					ListView.RenderBehaviorAttribute(writer, text, this.extraAttributes[text]);
				}
				writer.Write(">");
				writer.Write("<table class=\"lyt\" cellpadding=\"0\"><tr><td id=\"tdHdr\"><table id=\"tblHdrLyt\" cellpadding=\"0\"><tr><td id=\"tdHdrCtnt\"><div id=\"divHS\">");
			}
			if ((renderFlags & ListView.RenderFlags.Headers) > (ListView.RenderFlags)0)
			{
				ExTraceGlobals.MailTracer.TraceDebug((long)this.GetHashCode(), "Rendering ListView headers");
				LegacyListViewHeaders legacyListViewHeaders = new ColumnHeaders(this.viewDescriptor, this.sortedColumn, this.sortOrder, this.userContext);
				legacyListViewHeaders.Render(writer);
			}
			if ((renderFlags & ListView.RenderFlags.Behavior) > (ListView.RenderFlags)0)
			{
				writer.Write("</div></td>");
				writer.Write("<td id=\"tdFill\">&nbsp;</td>");
				writer.Write("</tr>");
				writer.Write("</table>");
				writer.Write("</td></tr>");
				if (this.HasInlineControl)
				{
					writer.Write("<tr><td id=tdILC>");
					this.RenderInlineControl(writer);
					writer.Write("</td></tr>");
				}
				writer.Write("<tr><td id=tdIL>");
				writer.Write("<div id=divPrgrs style=\"display:none\">");
				this.userContext.RenderThemeImage(writer, ThemeFileId.ProgressSmall);
				writer.Write(" <span id=spnTxt></span>");
				writer.Write("</div>");
			}
			if ((renderFlags & ListView.RenderFlags.Contents) > (ListView.RenderFlags)0)
			{
				ExTraceGlobals.MailTracer.TraceDebug((long)this.GetHashCode(), "Rendering ListView contents");
				writer.Write("<div id=divIL>");
				if (this.isFilteredView)
				{
					writer.Write("<div class=fltrBg></div>");
				}
				if (!this.DataSource.UserHasRightToLoad || this.DataSource.RangeCount == 0)
				{
					writer.Write("<div id=divNI>");
					if (!this.DataSource.UserHasRightToLoad)
					{
						writer.Write(LocalizedStrings.GetHtmlEncoded(-593658721));
					}
					else if (this.isFilteredView)
					{
						writer.Write(LocalizedStrings.GetHtmlEncoded(417836457));
					}
					else
					{
						writer.Write(LocalizedStrings.GetHtmlEncoded(-474826895));
					}
					writer.Write("</div>");
				}
				else
				{
					this.contents.Render(writer);
				}
				writer.Write("</div>");
			}
			if ((renderFlags & ListView.RenderFlags.Behavior) > (ListView.RenderFlags)0)
			{
				writer.Write("</td></tr>");
				if ((renderFlags & ListView.RenderFlags.Paging) > (ListView.RenderFlags)0)
				{
					writer.Write("<tr><td>");
				}
				else
				{
					writer.Write("<tr style=\"display:none\"><td>");
				}
				this.RenderPagingUI(writer);
				writer.Write("</td></tr>");
				writer.Write("</table>");
				writer.Write("</div>");
			}
		}

		private static void RenderBehaviorAttribute(TextWriter writer, string name, string value)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name cannot be null or empty.");
			}
			if (string.IsNullOrEmpty(value))
			{
				return;
			}
			writer.Write(" ");
			writer.Write(name);
			writer.Write("=\"");
			Utilities.HtmlEncode(value, writer);
			writer.Write("\"");
		}

		private static void RenderBehaviorAttribute(TextWriter writer, string name, int value)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name cannot be null or empty.");
			}
			writer.Write(" ");
			writer.Write(name);
			writer.Write("=");
			writer.Write(value.ToString(CultureInfo.InvariantCulture));
		}

		private static void RenderPagingItems(TextWriter writer)
		{
			writer.Write(LocalizedStrings.GetHtmlEncoded(-577737662));
		}

		private void RenderPagingUI(TextWriter writer)
		{
			writer.Write("<table class=\"pgLyt\">");
			writer.Write("<tr>");
			writer.Write("<td id=\"tdCT\" class=\"");
			writer.Write(this.userContext.IsRtl ? "r" : "l");
			writer.Write("\" nowrap>");
			ListView.RenderPagingItems(writer);
			this.RenderPagingRangeStart(writer);
			this.RenderPagingEndRange(writer);
			writer.Write("</td>");
			writer.Write("<td id=\"tdPF\"></td>");
			writer.Write("<td id=\"tdPI\"");
			if (this.DataSource.RangeCount == 0 || this.DataSource.RangeCount == this.DataSource.TotalCount)
			{
				writer.Write(" style=\"filter:alpha(opacity=35);\"");
			}
			writer.Write("><div>");
			this.userContext.RenderThemeImage(writer, this.userContext.IsRtl ? ThemeFileId.LastPage : ThemeFileId.FirstPage, null, new object[]
			{
				"id=fP",
				"title=\"" + LocalizedStrings.GetHtmlEncoded(-946066775) + "\""
			});
			this.userContext.RenderThemeImage(writer, this.userContext.IsRtl ? ThemeFileId.NextPage : ThemeFileId.PreviousPage, null, new object[]
			{
				"id=pP",
				"title=\"" + LocalizedStrings.GetHtmlEncoded(-1907861992) + "\""
			});
			this.userContext.RenderThemeImage(writer, this.userContext.IsRtl ? ThemeFileId.PreviousPage : ThemeFileId.NextPage, null, new object[]
			{
				"id=nP",
				"title=\"" + LocalizedStrings.GetHtmlEncoded(1548165396) + "\""
			});
			this.userContext.RenderThemeImage(writer, this.userContext.IsRtl ? ThemeFileId.FirstPage : ThemeFileId.LastPage, null, new object[]
			{
				"id=lP",
				"title=\"" + LocalizedStrings.GetHtmlEncoded(-991618511) + "\""
			});
			writer.Write("</div></td>");
			writer.Write("</tr>");
			writer.Write("</table>");
		}

		private void RenderPagingRangeStart(TextWriter writer)
		{
			int value = (this.DataSource.RangeCount == 0) ? 0 : (this.DataSource.StartRange + 1);
			writer.Write("&nbsp;");
			writer.Write("<input id=\"txtSR\" type=\"text\" value=\"");
			writer.Write(value);
			writer.Write("\" sR=\"");
			writer.Write(value);
			writer.Write("\"");
			if (this.DataSource.RangeCount == 0 || this.DataSource.RangeCount == this.DataSource.TotalCount)
			{
				writer.Write(" disabled=\"true\"");
			}
			writer.Write("> ");
			writer.Write("&nbsp;");
		}

		private void RenderPagingEndRange(TextWriter writer)
		{
			int num;
			if (this.DataSource.RangeCount == 0)
			{
				num = 0;
			}
			else
			{
				num = 1 + this.DataSource.EndRange;
			}
			if (this.DataSource is ADListViewDataSource)
			{
				writer.Write(string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetHtmlEncoded(-727972566), new object[]
				{
					"<span id=spnER>" + num + "</span>"
				}));
				return;
			}
			writer.Write(string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetHtmlEncoded(-948999625), new object[]
			{
				"<span id=spnER>" + num + "</span>",
				"<span id=spnTC>" + this.DataSource.TotalCount + "</span>"
			}));
		}

		protected virtual void RenderInlineControl(TextWriter writer)
		{
		}

		protected virtual bool HasInlineControl
		{
			get
			{
				return false;
			}
		}

		public void AddAttribute(string name, string value)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name is null or empty.");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.extraAttributes.Add(name, value);
		}

		private static readonly string sortAscending = 0.ToString(CultureInfo.InvariantCulture);

		private static readonly string sortDescending = 1.ToString(CultureInfo.InvariantCulture);

		private LegacyListViewContents contents;

		private ViewDescriptor viewDescriptor;

		private SortOrder sortOrder;

		private ColumnId sortedColumn = ColumnId.Count;

		private UserContext userContext;

		private bool isFilteredView;

		private Dictionary<string, string> extraAttributes = new Dictionary<string, string>();

		[Flags]
		public enum RenderFlags
		{
			Behavior = 1,
			Contents = 2,
			Headers = 4,
			Paging = 8,
			CompactWebPart = 13,
			All = 15
		}
	}
}
