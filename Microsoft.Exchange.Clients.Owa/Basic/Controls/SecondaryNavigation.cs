using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal class SecondaryNavigation
	{
		public SecondaryNavigation(OwaContext owaContext, StoreObjectId folderId)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			this.owaContext = owaContext;
			this.selectedFolderId = folderId;
		}

		public void RenderManageFolderButton(Strings.IDs buttonLabel, TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table cellspacing=0 cellpadding=0 class=\"snfmb\"><tr><td nowrap>");
			writer.Write("<a href=\"#\" id=\"lnkMngFldr\" onClick=\"return onClkFM()\"><img src=\"");
			this.owaContext.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Root);
			writer.Write("\" alt=\"\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(buttonLabel));
			writer.Write("</a></td></tr></table>");
		}

		public void RenderHorizontalDivider(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table class=\"sntls\"><tr><td><img src=\"");
			this.owaContext.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Clear1x1);
			writer.Write("\" alt=\"\"></td></tr></table>");
		}

		protected OwaContext owaContext;

		protected StoreObjectId selectedFolderId;
	}
}
