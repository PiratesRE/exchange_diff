using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class SharedCalendarItemInfobar
	{
		internal SharedCalendarItemInfobar(UserContext userContext, CalendarFolder folder, int colorIndex, bool renderNotifyForOtherUser)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			this.userContext = userContext;
			this.folder = folder;
			this.colorIndex = colorIndex;
			this.renderNotifyForOtherUser = renderNotifyForOtherUser;
			this.isSharedOutFolder = Utilities.IsFolderSharedOut(folder);
			OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromStoreObject(folder);
			if (owaStoreObjectId.GetSession(userContext) is MailboxSession)
			{
				this.folderEncodedDisplayName = Utilities.SanitizeHtmlEncode(string.Format(LocalizedStrings.GetNonEncoded(-83764036), folder.DisplayName, Utilities.GetMailboxOwnerDisplayName((MailboxSession)owaStoreObjectId.GetSession(userContext))));
			}
			else
			{
				this.folderEncodedDisplayName = Utilities.SanitizeHtmlEncode(folder.DisplayName);
			}
			this.isSharedFolder = (owaStoreObjectId != null && owaStoreObjectId.IsOtherMailbox);
			this.isPublicFolder = owaStoreObjectId.IsPublic;
			if (this.isSharedFolder)
			{
				this.folderOwnerEncodedName = Utilities.SanitizeHtmlEncode(Utilities.GetFolderOwnerExchangePrincipal(owaStoreObjectId, userContext).MailboxInfo.DisplayName);
			}
			this.containerClass = folder.GetValueOrDefault<string>(StoreObjectSchema.ContainerClass, "IPF.Appointment");
		}

		internal void Build(Infobar infobar)
		{
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			sanitizingStringBuilder.Append("<div id=divSFIB ");
			if (this.colorIndex != -2)
			{
				sanitizingStringBuilder.Append("class=bcal");
				sanitizingStringBuilder.Append<int>(CalendarColorManager.GetClientColorIndex(this.colorIndex));
			}
			else
			{
				sanitizingStringBuilder.Append("class=calNoClr");
			}
			sanitizingStringBuilder.Append(">");
			string str = this.userContext.IsRtl ? "<div class=\"fltRight\"" : "<div class=\"fltLeft\"";
			sanitizingStringBuilder.Append(str);
			if (this.isSharedFolder && this.renderNotifyForOtherUser)
			{
				sanitizingStringBuilder.Append("><input type=\"checkbox\" id=\"chkNtfy\"></div>");
				sanitizingStringBuilder.Append(str);
				sanitizingStringBuilder.Append(" id=\"divNtfy\"><label for=\"chkNtfy\">");
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(816646619));
				sanitizingStringBuilder.Append(" ");
				sanitizingStringBuilder.Append<SanitizedHtmlString>(this.folderOwnerEncodedName);
				sanitizingStringBuilder.Append("</label></div>");
				str = (this.userContext.IsRtl ? "<div class=\"fltLeft\"" : "<div class=\"fltRight\"");
				sanitizingStringBuilder.Append(str);
			}
			if (!this.isSharedFolder || !this.renderNotifyForOtherUser)
			{
				sanitizingStringBuilder.Append(" id=\"divShrType\">");
				this.BuildFolderType(sanitizingStringBuilder);
			}
			else
			{
				sanitizingStringBuilder.Append(" id=\"divShrName\">");
				this.BuildFolderName(sanitizingStringBuilder);
			}
			sanitizingStringBuilder.Append(this.userContext.DirectionMark);
			sanitizingStringBuilder.Append("</div>");
			sanitizingStringBuilder.Append(str);
			sanitizingStringBuilder.Append(" id=\"divShrImg\">");
			this.BuildIcon(sanitizingStringBuilder);
			sanitizingStringBuilder.Append(this.userContext.DirectionMark);
			sanitizingStringBuilder.Append("</div>");
			sanitizingStringBuilder.Append(str);
			if (!this.isSharedFolder || !this.renderNotifyForOtherUser)
			{
				sanitizingStringBuilder.Append(" id=\"divShrName\">");
				this.BuildFolderName(sanitizingStringBuilder);
			}
			else
			{
				sanitizingStringBuilder.Append(" id=\"divShrType\">");
				this.BuildFolderType(sanitizingStringBuilder);
			}
			sanitizingStringBuilder.Append("</div></div>");
			infobar.AddMessage(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>(), InfobarMessageType.Informational, "divCalendarInfobarMessage");
		}

		private void BuildFolderType(SanitizingStringBuilder<OwaHtml> stringBuilder)
		{
			if (this.isSharedFolder || this.isSharedOutFolder)
			{
				stringBuilder.Append(LocalizedStrings.GetNonEncoded(1379417169));
				return;
			}
			if (this.isPublicFolder)
			{
				stringBuilder.Append(LocalizedStrings.GetNonEncoded(16167073));
				return;
			}
			stringBuilder.Append(LocalizedStrings.GetNonEncoded(185425884));
		}

		private void BuildIcon(SanitizingStringBuilder<OwaHtml> stringBuilder)
		{
			FolderSharingFlag sharingFlag = FolderSharingFlag.None;
			if (this.isSharedFolder || this.isPublicFolder)
			{
				sharingFlag = FolderSharingFlag.SharedIn;
			}
			else if (this.isSharedOutFolder)
			{
				sharingFlag = FolderSharingFlag.SharedOut;
			}
			using (SanitizingStringWriter<OwaHtml> sanitizingStringWriter = new SanitizingStringWriter<OwaHtml>())
			{
				SmallIconManager.RenderFolderIcon(sanitizingStringWriter, this.userContext, this.containerClass, sharingFlag, false, new string[0]);
				stringBuilder.Append<SanitizedHtmlString>(sanitizingStringWriter.ToSanitizedString<SanitizedHtmlString>());
			}
		}

		private void BuildFolderName(SanitizingStringBuilder<OwaHtml> stringBuilder)
		{
			if (!this.isSharedFolder)
			{
				stringBuilder.Append<SanitizedHtmlString>(this.folderEncodedDisplayName);
				return;
			}
			if (Utilities.IsDefaultFolder(this.folder, DefaultFolderType.Calendar))
			{
				stringBuilder.Append<SanitizedHtmlString>(this.folderOwnerEncodedName);
				return;
			}
			string htmlEncoded = LocalizedStrings.GetHtmlEncoded(-881877772);
			stringBuilder.Append<SanitizedHtmlString>(SanitizedHtmlString.Format(this.userContext.UserCulture, htmlEncoded, new object[]
			{
				this.folderEncodedDisplayName,
				this.folderOwnerEncodedName
			}));
		}

		private CalendarFolder folder;

		private UserContext userContext;

		private int colorIndex;

		private bool renderNotifyForOtherUser;

		private SanitizedHtmlString folderEncodedDisplayName;

		private bool isSharedOutFolder;

		private bool isSharedFolder;

		private bool isPublicFolder;

		private SanitizedHtmlString folderOwnerEncodedName;

		private string containerClass;
	}
}
