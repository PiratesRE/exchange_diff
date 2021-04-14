using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public abstract class ListViewSubPage : OwaSubPage
	{
		protected ListViewSubPage(Trace callTracer, Trace algorithmTracer)
		{
			this.callTracer = callTracer;
			this.algorithmTracer = algorithmTracer;
		}

		protected TextWriter Writer
		{
			get
			{
				return base.Response.Output;
			}
		}

		protected Trace CallTracer
		{
			get
			{
				return this.callTracer;
			}
		}

		protected Trace AlgorithmTracer
		{
			get
			{
				return this.algorithmTracer;
			}
		}

		protected abstract int ViewWidth { get; }

		protected virtual int LVRPContainerTop
		{
			get
			{
				return 0;
			}
		}

		protected virtual int SearchControlTop
		{
			get
			{
				if (this.ShouldRenderToolbar)
				{
					return 27;
				}
				return 27 - this.ToolbarHeight;
			}
		}

		protected virtual int ToolbarHeight
		{
			get
			{
				return 25;
			}
		}

		protected virtual int ListViewTop
		{
			get
			{
				if (this.ShouldRenderToolbar)
				{
					return 60;
				}
				return 60 - this.ToolbarHeight;
			}
		}

		protected abstract int ViewHeight { get; }

		protected abstract SortOrder DefaultSortOrder { get; }

		protected abstract SortOrder SortOrder { get; }

		protected abstract ColumnId DefaultSortedColumn { get; }

		protected abstract ColumnId SortedColumn { get; }

		protected abstract ReadingPanePosition DefaultReadingPanePosition { get; }

		protected abstract ReadingPanePosition ReadingPanePosition { get; }

		protected abstract bool DefaultMultiLineSetting { get; }

		protected abstract bool IsMultiLine { get; }

		protected abstract bool FindBarOn { get; }

		protected virtual bool ShouldRenderSearch
		{
			get
			{
				return true;
			}
		}

		protected abstract bool AllowAdvancedSearch { get; }

		protected virtual bool RenderSearchDropDown
		{
			get
			{
				return true;
			}
		}

		protected virtual string ContainerName
		{
			get
			{
				return string.Empty;
			}
		}

		protected virtual bool ShouldRenderToolbar
		{
			get
			{
				return true;
			}
		}

		protected abstract void LoadViewState();

		protected abstract IListView CreateListView(ColumnId sortedColumn, SortOrder sortOrder);

		protected abstract IListViewDataSource CreateDataSource(ListView listView);

		protected abstract Toolbar CreateListToolbar();

		protected abstract Toolbar CreateActionToolbar();

		protected virtual void RenderViewInfobars()
		{
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.serializedContainerId = Utilities.GetQueryStringParameter(base.Request, "id", false);
			if (Utilities.GetQueryStringParameter(base.Request, "dl", false) != null)
			{
				this.isPaaPicker = true;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			this.CallTracer.TraceDebug((long)this.GetHashCode(), "FolderListViewPage.OnLoad");
			this.LoadViewState();
			this.AlgorithmTracer.TraceDebug<bool, int, int>((long)this.GetHashCode(), "Creating ListView with isMultiline={0}, sortedColumn={1}, and sortOrder={2}", this.IsMultiLine, (int)this.SortedColumn, (int)this.SortOrder);
			this.listView = this.CreateListView(this.SortedColumn, this.SortOrder);
			ListView listView = this.listView as ListView;
			if (listView != null)
			{
				IListViewDataSource listViewDataSource = this.CreateDataSource(listView);
				listViewDataSource.Load(0, base.UserContext.UserOptions.ViewRowCount);
				listView.DataSource = listViewDataSource;
			}
			base.OnLoad(e);
		}

		protected string SerializedContainerId
		{
			get
			{
				return this.serializedContainerId;
			}
		}

		protected bool IsPersonalAutoAttendantPicker
		{
			get
			{
				return this.isPaaPicker;
			}
		}

		protected virtual void RenderListViewPage()
		{
			this.Writer.Write("<div id=divVw>\r\n");
			this.RenderViewInfobars();
			this.Writer.Write("\t<div id=divLVRPContainer class=lvRPContainer style=\"top:");
			this.Writer.Write(this.LVRPContainerTop);
			this.Writer.Write("px;\">\r\n");
			this.RenderReadingPaneContainer();
			this.RenderListViewContainer();
			this.RenderListViewDivider();
			this.Writer.Write("\t</div>\r\n");
			this.Writer.Write("</div>\r\n");
		}

		protected void RenderListViewContainer()
		{
			if (this.ReadingPanePosition == ReadingPanePosition.Right)
			{
				this.Writer.Write("\t\t<div id=divLVContainer class=lvContainer style=\"height:100%; width:");
				this.Writer.Write(this.ViewWidth);
				this.Writer.Write("px;\">\r\n");
			}
			else if (this.ReadingPanePosition == ReadingPanePosition.Bottom)
			{
				this.Writer.Write("\t\t<div id=divLVContainer class=\"lvContainer rpBtm\" style=\"width:100%; height:");
				this.Writer.Write(this.ViewHeight);
				this.Writer.Write("px;\">\r\n");
			}
			else if (this.ReadingPanePosition == ReadingPanePosition.Off)
			{
				this.Writer.Write("\t\t<div id=divLVContainer class=\"lvContainer rpOff\" style=\"width:100%; height:100%;\">\r\n");
			}
			if (this.ShouldRenderToolbar)
			{
				this.Writer.Write("\t\t\t<div id=\"divToolbarStrip\" class=\"ts\">");
				Toolbar toolbar = this.CreateListToolbar();
				toolbar.Render(this.Writer);
				this.Writer.Write("</div>");
			}
			if (this.ShouldRenderSearch)
			{
				this.RenderSearch();
			}
			this.Writer.Write("\r\n\t\t\t<div id=divLV class=lvPos style=\"top:");
			this.Writer.Write(this.ListViewTop);
			this.Writer.Write("px; min-width:");
			this.Writer.Write(325);
			this.Writer.Write("px;\">");
			this.RenderListView();
			this.Writer.Write("</div>\r\n");
			this.Writer.Write("\t\t</div>\r\n");
		}

		protected void RenderListViewDivider()
		{
			this.Writer.Write("\t\t<div id=divLVDivider class=lvDivider style=\"");
			this.Writer.Write(base.UserContext.IsRtl ? "right:" : "left:");
			this.Writer.Write(this.ViewWidth + 1);
			this.Writer.Write("px; ");
			if (this.ReadingPanePosition == ReadingPanePosition.Off || this.ReadingPanePosition == ReadingPanePosition.Bottom)
			{
				this.Writer.Write("display:none");
			}
			this.Writer.Write("\">");
			base.UserContext.RenderThemeImage(this.Writer, base.UserContext.IsRtl ? ThemeFileId.VLVShadowTopRTL : ThemeFileId.VLVShadowTop, null, new object[]
			{
				"id=\"divVLVShadowTop\""
			});
			this.Writer.Write("<div id=divVLVShadowTile></div>");
			this.Writer.Write("<img id=imgVwDrag class=dividerImgNotDragging src=\"");
			base.UserContext.RenderThemeFileUrl(this.Writer, ThemeFileId.Clear1x1);
			this.Writer.Write("\" alt=\"\">");
			this.Writer.Write("</div>\r\n");
			this.Writer.Write("\t\t<div id=divLVDividerHorizontal class=lvDividerHorizontal style= \"top:");
			this.Writer.Write(this.ViewHeight);
			this.Writer.Write("px; ");
			if (this.ReadingPanePosition == ReadingPanePosition.Off || this.ReadingPanePosition == ReadingPanePosition.Right)
			{
				this.Writer.Write("display:none");
			}
			this.Writer.Write("\">");
			this.Writer.Write("<img id=imgVwDragHor class=dividerImgNotDraggingHorizontal src=\"");
			base.UserContext.RenderThemeFileUrl(this.Writer, ThemeFileId.Clear1x1);
			this.Writer.Write("\" alt=\"\">");
			this.Writer.Write("</div>\r\n");
		}

		protected void RenderReadingPaneContainer()
		{
			this.Writer.Write("\t\t<div id=divRPContainer class=rpContainer style=\"");
			if (this.ReadingPanePosition == ReadingPanePosition.Right)
			{
				this.Writer.Write(base.UserContext.IsRtl ? "right:" : "left:");
				this.Writer.Write(this.ViewWidth + 13);
				this.Writer.Write("px; top: 0px;\"");
			}
			else if (this.ReadingPanePosition == ReadingPanePosition.Bottom)
			{
				this.Writer.Write("left: 0px; right: 0px; top:");
				this.Writer.Write(this.ViewHeight + 4);
				this.Writer.Write("px;\"");
			}
			if (this.ReadingPanePosition == ReadingPanePosition.Off)
			{
				this.Writer.Write(" display:none\"");
			}
			this.Writer.Write(" >\r\n");
			Toolbar toolbar = this.CreateActionToolbar();
			if (toolbar != null)
			{
				this.Writer.Write("<div class=ts>");
				toolbar.Render(this.Writer);
				this.Writer.Write("</div>\r\n");
			}
			this.Writer.Write("\t\t\t<div id=divRP class=\"rp ");
			if (toolbar != null)
			{
				this.Writer.Write("rpTop");
			}
			this.Writer.Write("\">");
			this.RenderReadingPane();
			this.Writer.Write("</div>\r\n");
			this.Writer.Write("\t\t</div>\r\n");
		}

		protected void RenderReadingPane()
		{
			OwaQueryStringParameters defaultItemParameters = this.GetDefaultItemParameters();
			string text = null;
			string text2 = null;
			if (defaultItemParameters != null)
			{
				text = defaultItemParameters.ItemClass;
				FormValue formValue = RequestDispatcherUtilities.DoFormsRegistryLookup(base.UserContext, defaultItemParameters.ApplicationElement, defaultItemParameters.ItemClass, defaultItemParameters.Action, defaultItemParameters.State);
				if (formValue != null)
				{
					text2 = (formValue.Value as string);
				}
			}
			bool flag = this.ReadingPanePosition != ReadingPanePosition.Off && base.UserContext.IsFeatureEnabled(Feature.SMime) && base.UserContext.BrowserType == BrowserType.IE && text != null && ObjectClass.IsSmime(text);
			bool flag2 = this.listView.TotalCount < 1;
			if (!flag && this.ReadingPanePosition != ReadingPanePosition.Off && text2 != null && RequestDispatcher.DoesSubPageSupportSingleDocument(text2))
			{
				if (base.UserContext.IsEmbeddedReadingPaneDisabled)
				{
					this.readingPanePlaceHolder.Controls.Add(new LiteralControl("<div id=ifRP url=\"about:blank\"></div>"));
					this.readingPanePlaceHolder.RenderControl(new HtmlTextWriter(this.Writer));
					return;
				}
				try
				{
					OwaSubPage owaSubPage = (OwaSubPage)this.Page.LoadControl(Path.GetFileName(text2));
					Utilities.PutOwaSubPageIntoPlaceHolder(this.readingPanePlaceHolder, "ifRP", owaSubPage, defaultItemParameters, null, flag2);
					base.ChildSubPages.Add(owaSubPage);
					this.readingPanePlaceHolder.RenderControl(new HtmlTextWriter(this.Writer));
					return;
				}
				catch (Exception innerException)
				{
					throw new OwaRenderingEmbeddedReadingPaneException(innerException);
				}
			}
			string s;
			if (!flag && this.ReadingPanePosition != ReadingPanePosition.Off && defaultItemParameters != null)
			{
				s = defaultItemParameters.QueryString;
			}
			else
			{
				s = base.UserContext.GetBlankPage();
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<iframe id=\"ifRP\" frameborder=\"0\" allowtransparency src=\"");
			Utilities.HtmlEncode(s, stringBuilder);
			stringBuilder.Append("\"");
			if (flag2)
			{
				stringBuilder.Append(" style=\"display:none\"");
			}
			if (flag)
			{
				stringBuilder.Append(" _Loc=\"");
				Utilities.HtmlEncode(s, stringBuilder);
				stringBuilder.Append("\"");
				stringBuilder.Append(" onload=\"");
				stringBuilder.Append("var oMmCtVer = null;");
				stringBuilder.Append("try {oMmCtVer = new ActiveXObject('OwaSMime2.MimeCtlVer');}catch (e){};");
				stringBuilder.Append("if(!oMmCtVer && this._Loc && this.src != this._Loc){this._Loc='';this.onload='';this.src=this._Loc;}\"");
			}
			stringBuilder.Append("></iframe>");
			this.readingPanePlaceHolder.Controls.Add(new LiteralControl(stringBuilder.ToString()));
			this.readingPanePlaceHolder.RenderControl(new HtmlTextWriter(this.Writer));
		}

		protected virtual string GetDefaultItemClass()
		{
			return null;
		}

		protected virtual OwaQueryStringParameters GetDefaultItemParameters()
		{
			return null;
		}

		protected void RenderListView(ReadingPanePosition readingPanePosition)
		{
			if (this.ReadingPanePosition != readingPanePosition)
			{
				return;
			}
			this.listView.Render(this.Writer);
		}

		protected void RenderListView()
		{
			this.listView.RenderForCompactWebPart(this.Writer);
		}

		protected virtual void RenderSearch()
		{
			this.RenderSearch(null);
		}

		internal void RenderSearch(Folder folder)
		{
			OutlookModule outlookModule = OutlookModule.None;
			SearchScope value = SearchScope.SelectedFolder;
			bool flag = false;
			bool flag2 = false;
			MailboxSession mailboxSession = base.UserContext.MailboxSession;
			if (folder != null)
			{
				outlookModule = Utilities.GetModuleForFolder(folder, base.UserContext);
				value = base.UserContext.UserOptions.GetSearchScope(outlookModule);
				mailboxSession = (folder.Session as MailboxSession);
				if (mailboxSession != null && mailboxSession.Mailbox.IsContentIndexingEnabled && folder.Id.ObjectId.Equals(base.UserContext.GetRootFolderId(mailboxSession)))
				{
					value = SearchScope.AllFoldersAndItems;
				}
				flag = Utilities.IsPublic(folder);
				if (!flag)
				{
					flag2 = Utilities.IsOtherMailbox(folder);
				}
			}
			this.Writer.Write("<div id=divSearchBox class=iactv ");
			this.Writer.Write("style=\"top:");
			this.Writer.Write(this.SearchControlTop);
			this.Writer.Write("px;\">");
			this.Writer.Write("<div id=divBasicSearch");
			if (!this.FindBarOn)
			{
				this.Writer.Write(" style=\"display:none\"");
			}
			this.Writer.Write(">");
			this.Writer.Write("<div id=divSearch");
			this.Writer.Write(" iScp=\"");
			this.Writer.Write((int)value);
			this.Writer.Write("\"");
			if (this.RenderSearchDropDown)
			{
				this.Writer.Write(" sFldNm=\"");
				Utilities.HtmlEncode(this.ContainerName, this.Writer);
				this.Writer.Write("\" L_Fld=\"");
				this.Writer.Write(LocalizedStrings.GetHtmlEncoded(-41655958));
				this.Writer.Write("\" L_FldSub=\"");
				this.Writer.Write(LocalizedStrings.GetHtmlEncoded(-444616176));
				this.Writer.Write("\"");
				if (outlookModule == OutlookModule.Contacts || outlookModule == OutlookModule.Tasks)
				{
					this.Writer.Write(" L_Mod=\"");
					if (outlookModule == OutlookModule.Contacts)
					{
						this.Writer.Write(LocalizedStrings.GetHtmlEncoded(-1237143503));
					}
					else
					{
						this.Writer.Write(LocalizedStrings.GetHtmlEncoded(-464657744));
					}
					this.Writer.Write("\"");
				}
				else
				{
					this.Writer.Write(" L_FldAll=\"");
					if (Utilities.IsInArchiveMailbox(folder))
					{
						string s = string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(1451900553), new object[]
						{
							Utilities.GetMailboxOwnerDisplayName((MailboxSession)folder.Session)
						});
						Utilities.HtmlEncode(s, this.Writer);
					}
					else
					{
						this.Writer.Write(LocalizedStrings.GetHtmlEncoded(-622081149));
					}
					this.Writer.Write("\"");
				}
			}
			if (!this.AllowAdvancedSearch)
			{
				this.Writer.Write(" style=\"");
				this.Writer.Write(base.UserContext.IsRtl ? "left:" : "right:");
				this.Writer.Write("5px;\">");
			}
			this.Writer.Write(">");
			this.Writer.Write("<div id=divTxt class=\"");
			this.Writer.Write((this.RenderSearchDropDown && folder != null && !Utilities.IsOtherMailbox(folder)) ? "txtBox" : "txtBoxNoScope");
			this.Writer.Write("\">");
			this.Writer.Write("<input type=text maxlength=256 id=txtS");
			if (this.RenderSearchDropDown)
			{
				string s2;
				switch (value)
				{
				case SearchScope.SelectedFolder:
					s2 = string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(-41655958), new object[]
					{
						this.ContainerName
					});
					goto IL_442;
				case SearchScope.SelectedAndSubfolders:
					s2 = string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(-444616176), new object[]
					{
						this.ContainerName
					});
					goto IL_442;
				case SearchScope.AllItemsInModule:
					if (outlookModule == OutlookModule.Contacts)
					{
						s2 = LocalizedStrings.GetNonEncoded(-1237143503);
						goto IL_442;
					}
					s2 = LocalizedStrings.GetNonEncoded(-464657744);
					goto IL_442;
				}
				if (Utilities.IsInArchiveMailbox(folder))
				{
					s2 = string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(1451900553), new object[]
					{
						Utilities.GetMailboxOwnerDisplayName((MailboxSession)folder.Session)
					});
				}
				else
				{
					s2 = LocalizedStrings.GetNonEncoded(-622081149);
				}
				IL_442:
				this.Writer.Write(" class=inactv fScp=1 value=\"");
				Utilities.HtmlEncode(s2, this.Writer);
				this.Writer.Write("\"");
			}
			this.Writer.Write("></div>");
			this.Writer.Write("<div ");
			if (folder != null && Utilities.IsOtherMailbox(folder))
			{
				this.Writer.Write("class=\"sImgb iactv\" ");
			}
			else
			{
				this.Writer.Write("class=\"sImg iactv\" ");
			}
			if (!this.RenderSearchDropDown)
			{
				this.Writer.Write(" style=\"");
				this.Writer.Write(base.UserContext.IsRtl ? "left:" : "right:");
				this.Writer.Write("0px;\"");
			}
			this.Writer.Write("id=divSIcon>");
			base.UserContext.RenderThemeImage(this.Writer, ThemeFileId.Search, null, new object[]
			{
				"id=imgS"
			});
			this.Writer.Write("</div>");
			if (folder == null || !Utilities.IsOtherMailbox(folder))
			{
				this.Writer.Write("<div id=divSScp class=iactv");
				this.Writer.Write(this.RenderSearchDropDown ? ">" : " style=\"display:none\">");
				base.UserContext.RenderThemeImage(this.Writer, ThemeFileId.DownButton3);
				this.Writer.Write("</div>");
			}
			this.Writer.Write("</div>");
			if (this.AllowAdvancedSearch)
			{
				this.Writer.Write("<div id=divASChevron>");
				base.UserContext.RenderThemeImage(this.Writer, ThemeFileId.Expand, null, new object[]
				{
					"id=imgSAS",
					"title=\"" + LocalizedStrings.GetHtmlEncoded(903934295) + "\""
				});
				base.UserContext.RenderThemeImage(this.Writer, ThemeFileId.Collapse, null, new object[]
				{
					"id=imgHAS",
					"style=display:none",
					"title=\"" + LocalizedStrings.GetHtmlEncoded(-5515128) + "\""
				});
				this.Writer.Write("</div>");
			}
			this.Writer.Write("</div>");
			this.Writer.Write("<div id=divAS style=\"display:none;\">");
			if (this is FolderListViewSubPage && !flag && !mailboxSession.Mailbox.IsContentIndexingEnabled && !flag2)
			{
				this.Writer.Write("<div id=divDsblCI style=\"display:none\">");
				RenderingUtilities.RenderError(base.UserContext, this.Writer, -485624509);
				this.Writer.Write("</div>");
			}
			this.Writer.Write("</div>");
			this.Writer.Write("</div>");
		}

		public override string BodyCssClass
		{
			get
			{
				if (base.UserContext.IsWebPartRequest)
				{
					return "view webpart";
				}
				return "view";
			}
		}

		private const string ContainerIdQueryParameter = "id";

		private const string IsPaaPickerQueryParameter = "dl";

		protected const int ListViewTopWithSearch = 60;

		protected const int ListViewTopWithoutSearch = 31;

		protected const int DefaultSearchControlTop = 27;

		protected const int DefaultToolbarHeight = 25;

		protected const int VlvMinWidth = 325;

		protected const int VlvMinHeight = 230;

		protected IListView listView;

		protected PlaceHolder readingPanePlaceHolder;

		private string serializedContainerId;

		private bool isPaaPicker;

		private Trace callTracer;

		private Trace algorithmTracer;
	}
}
