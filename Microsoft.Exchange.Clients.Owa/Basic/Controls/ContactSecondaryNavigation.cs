using System;
using System.IO;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class ContactSecondaryNavigation : SecondaryNavigation
	{
		public ContactSecondaryNavigation(OwaContext owaContext, StoreObjectId folderId, ContactFolderList contactFolderList) : base(owaContext, folderId)
		{
			HttpRequest request = owaContext.HttpContext.Request;
			UserContext userContext = owaContext.UserContext;
			this.contactFolderList = contactFolderList;
		}

		public void RenderContacts(TextWriter writer)
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ContactsSecondaryNavigation.Render()");
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"wh100\"><caption>");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-1286941817));
			writer.Write("</caption><tr><td>");
			UserContext userContext = this.owaContext.UserContext;
			if (this.contactFolderList == null)
			{
				this.contactFolderList = new ContactFolderList(userContext, this.selectedFolderId);
			}
			this.contactFolderList.Render(writer);
			base.RenderHorizontalDivider(writer);
			base.RenderManageFolderButton(1219371978, writer);
			writer.Write("</td></tr></table>");
		}

		private ContactFolderList contactFolderList;
	}
}
