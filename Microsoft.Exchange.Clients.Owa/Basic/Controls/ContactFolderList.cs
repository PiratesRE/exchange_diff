using System;
using System.IO;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class ContactFolderList : FolderList
	{
		public ContactFolderList(UserContext userContext, StoreObjectId selectedFolderId) : base(userContext, userContext.MailboxSession, NavigationModule.Contacts, ContactFolderList.MaxContactFolders, false, StoreObjectSchema.CreationTime, null)
		{
			this.selectedFolderId = selectedFolderId;
		}

		public static int MaxContactFolders
		{
			get
			{
				return 100;
			}
		}

		public void Render(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			bool flag = true;
			writer.Write("<table cellspacing=0 cellpadding=0 class=\"sncts\">");
			for (int i = 0; i < base.Count; i++)
			{
				VersionedId versionedId = (VersionedId)base.GetPropertyValue(i, FolderSchema.Id);
				string text = (string)base.GetPropertyValue(i, FolderSchema.DisplayName);
				writer.Write("<tr><td nowrap class=\"fld");
				if (versionedId.ObjectId.Equals(this.selectedFolderId))
				{
					writer.Write(" sl");
					flag = false;
				}
				writer.Write("\"><a href=\"#\" onClick=\"onClkCtsFdr(this, '");
				writer.Write(Utilities.JavascriptEncode(HttpUtility.UrlEncode(versionedId.ObjectId.ToBase64String())));
				writer.Write("')\" title=\"");
				Utilities.HtmlEncode(text, writer);
				writer.Write("\"><img src=\"");
				base.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Contact2Small);
				writer.Write("\" alt=\"\">");
				Utilities.CropAndRenderText(writer, text, 24);
				writer.Write("</a></td></tr>");
			}
			if (flag)
			{
				using (Folder folder = Folder.Bind(base.UserContext.MailboxSession, this.selectedFolderId))
				{
					writer.Write("<tr><td class=\"fter\"><img src=\"");
					base.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Clear);
					writer.Write("\" alt=\"\"></td></tr>");
					writer.Write("<tr><td class=\"mycts dsh\">");
					writer.Write(LocalizedStrings.GetHtmlEncoded(1171255778));
					writer.Write("</td></tr><tr><td class=\"fld sl\"><a href=\"#\" onClick=\"return false;\"><img src=\"");
					base.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Contact2Small);
					writer.Write("\" alt=\"\">");
					Utilities.CropAndRenderText(writer, folder.DisplayName, 24);
					writer.Write("</a></td></tr>");
				}
			}
			writer.Write("</table>");
		}

		private const int MaxContactFolderList = 100;

		private StoreObjectId selectedFolderId;
	}
}
