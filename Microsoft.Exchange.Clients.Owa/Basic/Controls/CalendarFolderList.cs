using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class CalendarFolderList : FolderList
	{
		public CalendarFolderList(UserContext userContext, StoreObjectId selectedFolderId) : base(userContext, userContext.MailboxSession, NavigationModule.Calendar, CalendarFolderList.MaxCalendars, false, StoreObjectSchema.CreationTime, new PropertyDefinition[]
		{
			FolderSchema.ExtendedFolderFlags
		})
		{
			this.selectedFolderId = selectedFolderId;
		}

		public static int MaxCalendars
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
			writer.Write("<table cellspacing=0 cellpadding=0 class=\"snt\">");
			writer.Write("<tr><td class=\"clhdr\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-583426935));
			writer.Write("</td></tr>");
			bool flag = true;
			for (int i = 0; i < base.Count; i++)
			{
				if (!Utilities.IsExternalSharedInFolder(base.GetPropertyValue(i, FolderSchema.ExtendedFolderFlags)))
				{
					string text = (string)base.GetPropertyValue(i, FolderSchema.DisplayName);
					VersionedId versionedId = (VersionedId)base.GetPropertyValue(i, FolderSchema.Id);
					StoreObjectId objectId = versionedId.ObjectId;
					writer.Write("<tr><td nowrap class=\"fld");
					bool flag2 = objectId.Equals(this.selectedFolderId);
					if (flag2)
					{
						writer.Write(" sl");
						flag = false;
					}
					writer.Write("\"><a href=\"?ae=Folder&t=IPF.Appointment&id=");
					Utilities.HtmlEncode(Utilities.UrlEncode(objectId.ToBase64String()), writer);
					writer.Write("\" title=\"");
					Utilities.HtmlEncode(text, writer);
					writer.Write("\">");
					writer.Write("<img src=\"");
					base.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Appointment);
					writer.Write("\" alt=\"\">");
					Utilities.CropAndRenderText(writer, text, 24);
					writer.Write(" </a>");
					writer.Write("</td></tr>");
				}
			}
			if (flag)
			{
				using (Folder folder = Folder.Bind(base.UserContext.MailboxSession, this.selectedFolderId))
				{
					writer.Write("<tr><td class=\"fter\"><img src=\"");
					base.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Clear);
					writer.Write("\" alt=\"\"></td></tr>");
					writer.Write("<tr><td class=\"clhdr\">");
					writer.Write(LocalizedStrings.GetHtmlEncoded(352017519));
					writer.Write("</td></tr><tr><td class=\"fld sl\"><a href=\"#\" onClick=\"return false;\"><img src=\"");
					base.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Appointment);
					writer.Write("\">");
					Utilities.CropAndRenderText(writer, folder.DisplayName, 24);
					writer.Write("</a></td></tr>");
				}
			}
			writer.Write("</table>");
		}

		private StoreObjectId selectedFolderId;
	}
}
