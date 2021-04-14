using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.DocumentLibrary;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class DocumentViewListToolbar : Toolbar
	{
		internal DocumentViewListToolbar(IDocumentLibraryFolder libraryFolder, UriFlags libraryType, bool isRootFolder) : base("divTBL")
		{
			this.libraryFolder = libraryFolder;
			this.libraryType = libraryType;
			this.isRootFolder = isRootFolder;
			this.contextMenu = new DocumentBreadcrumbBarContextMenu(base.UserContext);
		}

		internal DocumentViewListToolbar(IDocumentLibrary documentLibrary, UriFlags libraryType, bool isRootFolder) : base("divTBL")
		{
			this.documentLibrary = documentLibrary;
			this.libraryType = libraryType;
			this.isRootFolder = isRootFolder;
			this.contextMenu = new DocumentBreadcrumbBarContextMenu(base.UserContext);
		}

		internal DocumentViewListToolbar(SharepointSession sharepointSession, UriFlags libraryType, bool isRootFolder) : base("divTBL")
		{
			this.sharepointSession = sharepointSession;
			this.libraryType = libraryType;
			this.isRootFolder = isRootFolder;
			this.contextMenu = new DocumentBreadcrumbBarContextMenu(base.UserContext);
		}

		internal DocumentViewListToolbar(UncSession uncSession, UriFlags libraryType, bool isRootFolder) : base("divTBL")
		{
			this.uncSession = uncSession;
			this.libraryType = libraryType;
			this.isRootFolder = isRootFolder;
			this.contextMenu = new DocumentBreadcrumbBarContextMenu(base.UserContext);
		}

		protected override void RenderButtons()
		{
			if (this.isRootFolder)
			{
				base.RenderButton(ToolbarButtons.ParentFolder, ToolbarButtonFlags.Disabled);
			}
			else
			{
				base.RenderButton(ToolbarButtons.ParentFolder);
			}
			if (this.CanAddLibraryToFavorites())
			{
				base.RenderButton(ToolbarButtons.AddToFavorites);
			}
			this.RenderBreadcrumbBar();
		}

		private bool CanAddLibraryToFavorites()
		{
			UriFlags uriFlags = this.libraryType;
			switch (uriFlags)
			{
			case UriFlags.Sharepoint:
			case UriFlags.Unc:
			case UriFlags.SharepointDocumentLibrary:
			case UriFlags.UncDocumentLibrary:
				break;
			case UriFlags.Sharepoint | UriFlags.Unc:
			case UriFlags.DocumentLibrary:
				return false;
			default:
				switch (uriFlags)
				{
				case UriFlags.SharepointFolder:
				case UriFlags.UncFolder:
					break;
				default:
					return false;
				}
				break;
			}
			return true;
		}

		private void RenderBreadcrumbBar()
		{
			base.Writer.Write("<td class=\"w100 lnkbr\"><img align=absmiddle height=16 width=0>&nbsp;");
			UriFlags uriFlags = this.libraryType;
			switch (uriFlags)
			{
			case UriFlags.Sharepoint:
				this.RenderSharepointSiteBreadcrumbs();
				break;
			case UriFlags.Unc:
				this.RenderUncSiteBreadcrumbs();
				break;
			case UriFlags.Sharepoint | UriFlags.Unc:
			case UriFlags.DocumentLibrary:
				break;
			case UriFlags.SharepointDocumentLibrary:
				this.RenderSharepointBreadcrumbs(this.documentLibrary.GetHierarchy());
				break;
			case UriFlags.UncDocumentLibrary:
				this.RenderUncBreadcrumbs();
				break;
			default:
				switch (uriFlags)
				{
				case UriFlags.SharepointFolder:
					this.RenderSharepointBreadcrumbs(this.libraryFolder.GetHierarchy());
					break;
				case UriFlags.UncFolder:
					this.RenderUncBreadcrumbs();
					break;
				}
				break;
			}
			this.contextMenu.Render(base.Writer);
			base.Writer.Write("</td>");
		}

		private void RenderSharepointSiteBreadcrumbs()
		{
			this.RenderCrumb(this.sharepointSession.DisplayName, new Strings.IDs?(-527057840), this.sharepointSession.Uri.ToString(), UriFlags.Sharepoint, true);
		}

		private void RenderUncSiteBreadcrumbs()
		{
			base.Writer.Write("\\\\ ");
			this.RenderCrumb(this.uncSession.Title, this.uncSession.Uri.ToString(), UriFlags.Unc, true);
		}

		private void RenderSharepointBreadcrumbs(List<KeyValuePair<string, Uri>> crumbs)
		{
			bool flag = false;
			UriFlags[] array = new UriFlags[]
			{
				UriFlags.Sharepoint,
				UriFlags.SharepointDocumentLibrary,
				UriFlags.SharepointFolder
			};
			for (int i = 0; i < crumbs.Count; i++)
			{
				if (flag)
				{
					base.Writer.Write(" / ");
				}
				else
				{
					flag = true;
				}
				this.RenderCrumb(crumbs[i].Key, crumbs[i].Value.AbsoluteUri, (i < array.Length) ? array[i] : UriFlags.SharepointFolder, false);
			}
			if (flag)
			{
				base.Writer.Write(" / ");
			}
			if (this.libraryFolder != null)
			{
				this.RenderCrumb(this.libraryFolder.DisplayName, new Strings.IDs?(-527057840), this.libraryFolder.Uri.ToString(), UriFlags.SharepointFolder, true);
				return;
			}
			if (this.documentLibrary != null)
			{
				this.RenderCrumb(this.documentLibrary.Title, new Strings.IDs?(477016274), this.documentLibrary.Uri.ToString(), UriFlags.SharepointDocumentLibrary, true);
			}
		}

		private void RenderUncBreadcrumbs()
		{
			List<KeyValuePair<string, Uri>> list = (this.libraryFolder != null) ? this.libraryFolder.GetHierarchy() : this.documentLibrary.GetHierarchy();
			bool flag = false;
			UriFlags[] array = new UriFlags[]
			{
				UriFlags.Unc,
				UriFlags.UncDocumentLibrary,
				UriFlags.UncFolder
			};
			base.Writer.Write("<span dir=\"ltr\" class=\"nolnk\">\\\\</span> ");
			for (int i = 0; i < list.Count; i++)
			{
				if (flag)
				{
					base.Writer.Write(" \\ ");
				}
				else
				{
					flag = true;
				}
				this.RenderCrumb(list[i].Key.TrimEnd(new char[]
				{
					'/'
				}), list[i].Value.LocalPath.TrimEnd(new char[]
				{
					'\\'
				}), (i < array.Length) ? array[i] : UriFlags.UncFolder, false);
			}
			if (flag)
			{
				base.Writer.Write(" \\ ");
			}
			if (this.libraryFolder != null)
			{
				this.RenderCrumb(this.libraryFolder.DisplayName, new Strings.IDs?(-527057840), this.libraryFolder.Uri.ToString(), (list.Count == 2) ? UriFlags.UncDocumentLibrary : UriFlags.UncFolder, true);
				return;
			}
			this.RenderCrumb(this.documentLibrary.Title, new Strings.IDs?(477016274), this.documentLibrary.Uri.ToString(), (list.Count == 2) ? UriFlags.UncDocumentLibrary : UriFlags.UncFolder, true);
		}

		private void RenderCrumb(string displayName, string uri, UriFlags libraryType, bool refreshViewer)
		{
			this.RenderCrumb(displayName, null, uri, libraryType, refreshViewer);
		}

		private void RenderCrumb(string displayName, Strings.IDs? untitledName, string uri, UriFlags libraryType, bool refreshViewer)
		{
			if (untitledName != null && string.IsNullOrEmpty(displayName))
			{
				displayName = LocalizedStrings.GetNonEncoded(untitledName.Value);
			}
			base.Writer.Write("<span dir=\"ltr\" ");
			Utilities.RenderScriptHandler(base.Writer, "oncontextmenu", "shwBrCrMnu(_this);");
			base.Writer.Write(" ");
			Utilities.RenderScriptHandler(base.Writer, "onclick", "opnDocFldr(_this.uri," + (refreshViewer ? "1" : "0") + ");");
			base.Writer.Write(" uri=\"");
			Utilities.HtmlEncode(uri, base.Writer);
			base.Writer.Write("\" uf=");
			base.Writer.Write((int)libraryType);
			base.Writer.Write("\">");
			Utilities.HtmlEncode(displayName, base.Writer);
			base.Writer.Write("</span>");
		}

		private const string ToolbarId = "divTBL";

		private const string BeginBreadcrumbBar = "<td class=\"w100 lnkbr\"><img align=absmiddle height=16 width=0>&nbsp;";

		private const string EndBreadcrumbBar = "</td>";

		private const string SharepointDivider = " / ";

		private const string UncDivider = " \\ ";

		private SharepointSession sharepointSession;

		private UncSession uncSession;

		private IDocumentLibrary documentLibrary;

		private IDocumentLibraryFolder libraryFolder;

		private UriFlags libraryType = UriFlags.DocumentLibrary;

		private bool isRootFolder;

		private DocumentBreadcrumbBarContextMenu contextMenu;
	}
}
