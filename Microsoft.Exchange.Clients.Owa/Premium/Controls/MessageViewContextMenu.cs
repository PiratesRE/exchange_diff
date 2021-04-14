using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class MessageViewContextMenu : ContextMenu
	{
		public MessageViewContextMenu(UserContext userContext, string menuId, bool isPublicFolder, bool allowConversationView) : base(menuId, userContext)
		{
			this.isPublicFolder = isPublicFolder;
			this.allowConversationView = allowConversationView;
		}

		protected override void RenderExpandoData(TextWriter output)
		{
			if (this.allowConversationView)
			{
				output.Write(" _rgCnvOnlyItems=\"divOpnMsg,divSOpnMsg,divNewOnTop,divOldOnTop,divExpAll,divColAll,divS0\"");
			}
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (this.allowConversationView)
			{
				base.RenderMenuItem(output, 754949053, ThemeFileId.None, "divOpnMsg", "openMessage");
				ContextMenu.RenderMenuDivider(output, "divSOpnMsg");
				base.RenderMenuItem(output, 1746211700, ThemeFileId.None, "divNewOnTop", "oldestOnTop");
				base.RenderMenuItem(output, 2070168051, ThemeFileId.None, "divOldOnTop", "newestOnTop");
				base.RenderMenuItem(output, 18372887, ThemeFileId.None, "divExpAll", "expandAll");
				base.RenderMenuItem(output, -1678460464, ThemeFileId.None, "divColAll", "collapseAll");
				ContextMenu.RenderMenuDivider(output, "divS0");
			}
			base.RenderMenuItem(output, -1780771632, ThemeFileId.Post, "divPR", "postreply");
			base.RenderMenuItem(output, -327372070, ThemeFileId.Reply, "divR", "reply");
			base.RenderMenuItem(output, 826363927, ThemeFileId.ReplyAll, "divRA", "replyall");
			base.RenderMenuItem(output, -1428116961, ThemeFileId.Forward, "divF", "forward");
			if (this.userContext.IsInstantMessageEnabled())
			{
				base.RenderMenuItem(output, -124986716, ThemeFileId.Chat, "divCht", "chat");
			}
			if (base.UserContext.IsSmsEnabled)
			{
				base.RenderMenuItem(output, 1509309420, ThemeFileId.Sms, "divSndSms", "sendsms");
			}
			ContextMenu.RenderMenuDivider(output, "divS1");
			base.RenderMenuItem(output, -475579318, ThemeFileId.MeetingAccept, "divMIA", null, false, null, null, MeetingInviteResponseMenu.Create(this.userContext, ResponseType.Accept));
			base.RenderMenuItem(output, 1797669216, ThemeFileId.MeetingTentative, "divMIT", null, false, null, null, MeetingInviteResponseMenu.Create(this.userContext, ResponseType.Tentative));
			base.RenderMenuItem(output, -2119870632, ThemeFileId.MeetingDecline, "divMID", null, false, null, null, MeetingInviteResponseMenu.Create(this.userContext, ResponseType.Decline));
			ContextMenu.RenderMenuDivider(output, "divS2");
			base.RenderMenuItem(output, 438661106, ThemeFileId.ForwardAsAttachment, "divFIA", "fwia");
			if (this.userContext.ExchangePrincipal.RecipientTypeDetails == RecipientTypeDetails.DiscoveryMailbox)
			{
				ContextMenu.RenderMenuDivider(output, "divMsgNote");
				base.RenderMenuItem(output, 1146710980, ThemeFileId.MessageAnnotation, "divOpMsgNote", "opmsgnote");
			}
			ContextMenu.RenderMenuDivider(output, "divS3");
			base.RenderMenuItem(output, -228249127, ThemeFileId.MessageRead, "divMR", "markread");
			base.RenderMenuItem(output, 556449500, ThemeFileId.MessageUnread, "divMU", "markunread");
			try
			{
				DeletePolicyContextMenu.RenderAsSubmenu(output, this.userContext, new RenderMenuItemDelegate(base.RenderMenuItem));
				MovePolicyContextMenu.RenderAsSubmenu(output, this.userContext, new RenderMenuItemDelegate(base.RenderMenuItem));
			}
			catch (AccessDeniedException)
			{
			}
			ContextMenu.RenderMenuDivider(output, "divS4");
			if (this.userContext.IsFeatureEnabled(Feature.Rules))
			{
				base.RenderMenuItem(output, 1219103799, ThemeFileId.RulesSmall, "divCR", "crrul");
			}
			if (this.userContext.IsJunkEmailEnabled)
			{
				base.RenderMenuItem(output, -2053927452, ThemeFileId.JunkEMail, "divJnk", null, false, null, null, JunkEmailContextMenu.Create(this.userContext, JunkEmailContextMenuType.Item));
				ContextMenu.RenderMenuDivider(output, "divS5");
			}
			base.RenderMenuItem(output, 1381996313, ThemeFileId.Delete, "divD", "delete");
			if (this.allowConversationView)
			{
				base.RenderMenuItem(output, 1486263145, ThemeFileId.IgnoreConversation, "divIgnCnv", "ignoreconversation");
				base.RenderMenuItem(output, -476691185, ThemeFileId.IgnoreConversation, "divCanIgnCnv", "cancelignoreconversation");
			}
			ContextMenu.RenderMenuDivider(output, "divS6");
			base.RenderMenuItem(output, -1664268159, ThemeFileId.Move, "divMvToF", "MvToF");
			base.RenderMenuItem(output, -1581636675, ThemeFileId.CopyToFolder, "divCpToF", "CpToF");
			if (this.isPublicFolder)
			{
				return;
			}
			ContextMenu.RenderMenuDivider(output, "divSODL");
			base.RenderMenuItem(output, 472458684, ThemeFileId.OpenDeliveryReport, "divOpDlvRp", "opdlvrp");
			bool flag = true;
			int num = 0;
			using (List<UIExtensionManager.RightClickMenuExtensionItem>.Enumerator messageContextMenuItemEnumerator = UIExtensionManager.GetMessageContextMenuItemEnumerator())
			{
				while (messageContextMenuItemEnumerator.MoveNext())
				{
					UIExtensionManager.RightClickMenuExtensionItem rightClickMenuExtensionItem = messageContextMenuItemEnumerator.Current;
					if (flag)
					{
						ContextMenu.RenderMenuDivider(output, "divCustomSep");
						flag = false;
					}
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("_sUrl=\"");
					Utilities.HtmlEncode(rightClickMenuExtensionItem.TargetUrl, stringBuilder);
					Utilities.HtmlEncode(rightClickMenuExtensionItem.HasQueryString ? "&" : "?", stringBuilder);
					stringBuilder.Append("ea=");
					Utilities.HtmlEncode(Utilities.UrlEncode(this.userContext.MailboxIdentity.GetOWAMiniRecipient().PrimarySmtpAddress.ToString()), stringBuilder);
					stringBuilder.Append("\"");
					if (!string.IsNullOrEmpty(rightClickMenuExtensionItem.CustomType))
					{
						stringBuilder.Append(" _sT=\"");
						Utilities.HtmlEncode(rightClickMenuExtensionItem.CustomType, stringBuilder);
						stringBuilder.Append("\"");
					}
					base.RenderMenuItem(output, rightClickMenuExtensionItem.GetTextByLanguage(this.userContext.UserCulture.Name), string.IsNullOrEmpty(rightClickMenuExtensionItem.Icon) ? null : rightClickMenuExtensionItem.Icon, "divCstmCM" + num.ToString(CultureInfo.InvariantCulture), "cstmCM", false, stringBuilder.ToString(), null, null, null, null);
					num++;
				}
			}
		}

		private bool isPublicFolder;

		private bool allowConversationView;
	}
}
