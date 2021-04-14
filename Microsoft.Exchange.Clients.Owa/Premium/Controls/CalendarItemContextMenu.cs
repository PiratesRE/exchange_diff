using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class CalendarItemContextMenu : ContextMenu
	{
		public CalendarItemContextMenu(UserContext userContext) : base("divCalCm", userContext)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderMenuItem(output, 197744374, ThemeFileId.None, "divOpen", "open");
			base.RenderMenuItem(output, -612034802, ThemeFileId.Print, "divPrint", "print");
			ContextMenu.RenderMenuDivider(output, "divS1");
			base.RenderMenuItem(output, -327372070, ThemeFileId.Reply, "divReply", "reply");
			base.RenderMenuItem(output, 826363927, ThemeFileId.ReplyAll, "divReplyAll", "replyall");
			base.RenderMenuItem(output, -1428116961, ThemeFileId.Forward, "divForward", "forward");
			base.RenderMenuItem(output, 1643198969, ThemeFileId.None, "divCallSender", "callsender");
			if (this.userContext.IsInstantMessageEnabled())
			{
				base.RenderMenuItem(output, -725533133, ThemeFileId.None, "divChatWithSender", "chatwithsender");
			}
			if (base.UserContext.IsSmsEnabled)
			{
				base.RenderMenuItem(output, 1509309420, ThemeFileId.Sms, "divSndSms", "sendsms");
			}
			ContextMenu.RenderMenuDivider(output, "divS2");
			base.RenderMenuItem(output, -475579318, ThemeFileId.MeetingAccept, "divAccept", "accept", false, null, null, MeetingInviteResponseMenu.Create(this.userContext, ResponseType.Accept));
			base.RenderMenuItem(output, 1797669216, ThemeFileId.MeetingTentative, "divTentative", "tentative", false, null, null, MeetingInviteResponseMenu.Create(this.userContext, ResponseType.Tentative));
			base.RenderMenuItem(output, -2119870632, ThemeFileId.MeetingDecline, "divDecline", "decline", false, null, null, MeetingInviteResponseMenu.Create(this.userContext, ResponseType.Decline));
			ContextMenu.RenderMenuDivider(output, "divS3");
			base.RenderMenuItem(output, -1268489823, ThemeFileId.MenuPrivate, "divPrivate", "private");
			base.RenderMenuItem(output, 1833378074, ThemeFileId.None, "divShowTimeAs", "showtimeas", false, null, null, new CalendarItemShowTimeAsContextMenu(base.UserContext));
			ContextMenu.RenderMenuDivider(output, "divS4");
			base.RenderMenuItem(output, -1414360302, ThemeFileId.Move, "divMove", "move");
			base.RenderMenuItem(output, 1381996313, ThemeFileId.Delete, "divDelete", "delete");
		}
	}
}
