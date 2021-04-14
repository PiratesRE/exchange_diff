using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class RecoverDeletedItems : ListViewSubPage, IRegistryOnlyForm
	{
		public RecoverDeletedItems() : base(ExTraceGlobals.MailCallTracer, ExTraceGlobals.MailTracer)
		{
		}

		protected DumpsterViewArrangeByMenu ArrangeByMenu
		{
			get
			{
				return this.arrangeByMenu;
			}
		}

		protected override int ViewWidth
		{
			get
			{
				return this.viewWidth;
			}
		}

		protected override int ViewHeight
		{
			get
			{
				return this.viewHeight;
			}
		}

		protected DumpsterContextMenu ContextMenu
		{
			get
			{
				if (this.contextMenu == null)
				{
					this.contextMenu = new DumpsterContextMenu(base.UserContext);
				}
				return this.contextMenu;
			}
		}

		protected override SortOrder DefaultSortOrder
		{
			get
			{
				return SortOrder.Descending;
			}
		}

		protected override SortOrder SortOrder
		{
			get
			{
				return this.DefaultSortOrder;
			}
		}

		protected override ColumnId DefaultSortedColumn
		{
			get
			{
				return ColumnId.DeletedOnTime;
			}
		}

		protected override ColumnId SortedColumn
		{
			get
			{
				return this.DefaultSortedColumn;
			}
		}

		protected override ReadingPanePosition DefaultReadingPanePosition
		{
			get
			{
				return ReadingPanePosition.Min;
			}
		}

		protected override ReadingPanePosition ReadingPanePosition
		{
			get
			{
				return ReadingPanePosition.Min;
			}
		}

		protected override bool DefaultMultiLineSetting
		{
			get
			{
				return true;
			}
		}

		protected override bool IsMultiLine
		{
			get
			{
				return true;
			}
		}

		protected override bool FindBarOn
		{
			get
			{
				return true;
			}
		}

		protected override bool AllowAdvancedSearch
		{
			get
			{
				return false;
			}
		}

		protected override bool ShouldRenderToolbar
		{
			get
			{
				return true;
			}
		}

		protected override bool RenderSearchDropDown
		{
			get
			{
				return false;
			}
		}

		protected override void LoadViewState()
		{
			OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromString(Utilities.GetQueryStringParameter(base.Request, "id"));
			MailboxSession mailboxSession = (MailboxSession)owaStoreObjectId.GetSession(base.UserContext);
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.RecoverableItemsDeletions);
			if (defaultFolderId == null)
			{
				DumpsterFolderHelper.CheckAndCreateFolder(mailboxSession);
				defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.RecoverableItemsDeletions);
			}
			OwaStoreObjectId folderId = OwaStoreObjectId.CreateFromStoreObjectId(defaultFolderId, owaStoreObjectId);
			this.folder = Utilities.GetFolderForContent<Folder>(base.UserContext, folderId, RecoverDeletedItems.folderProperties);
			this.viewWidth = Utilities.GetFolderProperty<int>(this.folder, ViewStateProperties.ViewWidth, 450);
			this.viewHeight = Utilities.GetFolderProperty<int>(this.folder, ViewStateProperties.ViewHeight, 250);
		}

		protected override IListView CreateListView(ColumnId sortedColumn, SortOrder sortOrder)
		{
			DumpsterVirtualListView dumpsterVirtualListView = new DumpsterVirtualListView(base.UserContext, "divVLV", sortedColumn, sortOrder, this.folder);
			dumpsterVirtualListView.LoadData(0, 50);
			return dumpsterVirtualListView;
		}

		protected override IListViewDataSource CreateDataSource(ListView listView)
		{
			SortBy[] sortByProperties = listView.GetSortByProperties();
			return new FolderListViewDataSource(base.UserContext, listView.Properties, this.folder, sortByProperties);
		}

		protected void RenderFolderCount()
		{
			string text = string.Format(CultureInfo.InvariantCulture, "<span id=spnIC>{0}</span>", new object[]
			{
				this.folder.ItemCount
			});
			text = string.Format(LocalizedStrings.GetHtmlEncoded(1648193418), text);
			base.Writer.Write("<span class=spnFST>");
			base.Writer.Write("<span id=spnDIC>");
			base.Writer.Write(text);
			base.Writer.Write("</span>");
			base.Writer.Write("</span>");
		}

		protected void RenderDumpsterListView()
		{
			base.Writer.Write("\t\t<div id=divLVContainer class=dmpLvContainer style=\"width:100%\">\r\n");
			base.Writer.Write("\t\t\t<div id=\"divToolbarStrip\" class=\"dmpTs\">");
			Toolbar toolbar = this.CreateListToolbar();
			toolbar.Render(base.Writer);
			base.Writer.Write("</div>");
			base.RenderSearch(this.folder);
			base.Writer.Write("\r\n\t\t\t<div id=divLV class=dmpLvPos style=\"top:");
			base.Writer.Write(this.ListViewTop);
			base.Writer.Write("px;\">");
			base.RenderListView();
			base.Writer.Write("</div>\r\n");
			base.Writer.Write("\t\t</div>\r\n");
			base.Writer.Write("<div class=\"dmpIC\">");
			this.RenderFolderCount();
			base.Writer.Write("</div>\r\n");
		}

		protected void RenderJavascriptEncodedFolderId()
		{
			Utilities.JavascriptEncode(Utilities.GetIdAsString(this.folder), base.Response.Output);
		}

		protected override Toolbar CreateListToolbar()
		{
			return new DumpsterViewListToolbar();
		}

		protected override Toolbar CreateActionToolbar()
		{
			return null;
		}

		protected override void OnUnload(EventArgs e)
		{
			if (this.folder != null)
			{
				this.folder.Dispose();
				this.folder = null;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.arrangeByMenu = new DumpsterViewArrangeByMenu(this.folder);
		}

		public override IEnumerable<string> ExternalScriptFiles
		{
			get
			{
				return this.externalScriptFiles;
			}
		}

		public override SanitizedHtmlString Title
		{
			get
			{
				return SanitizedHtmlString.FromStringId(369288321);
			}
		}

		public override string PageType
		{
			get
			{
				return "DumpsterViewPage";
			}
		}

		private static readonly PropertyDefinition[] folderProperties = new PropertyDefinition[]
		{
			FolderSchema.DisplayName,
			FolderSchema.ItemCount,
			FolderSchema.UnreadCount,
			ViewStateProperties.ViewWidth,
			ViewStateProperties.ViewHeight,
			ViewStateProperties.SortColumn,
			ViewStateProperties.SortOrder
		};

		private int viewWidth = 450;

		private int viewHeight = 250;

		private Folder folder;

		private DumpsterViewArrangeByMenu arrangeByMenu;

		private DumpsterContextMenu contextMenu;

		private string[] externalScriptFiles = new string[]
		{
			"uview.js",
			"dumpstervw.js",
			"vlv.js"
		};
	}
}
