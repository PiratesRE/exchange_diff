using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class TaskViewContextMenu : ContextMenu
	{
		public TaskViewContextMenu(UserContext userContext) : base("divVwm", userContext)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderMenuItem(output, 197744374, ThemeFileId.None, "divO", "open");
			ContextMenu.RenderMenuDivider(output, "divS1");
			base.RenderMenuItem(output, -747517193, ThemeFileId.EMailContact, "divNM", "nmsgct");
			if (base.UserContext.IsFeatureEnabled(Feature.Calendar))
			{
				base.RenderMenuItem(output, -1596894910, ThemeFileId.Appointment, "divNMR", "nmrgct");
			}
			base.RenderMenuItem(output, -327372070, ThemeFileId.Reply, "divR", "reply");
			base.RenderMenuItem(output, 826363927, ThemeFileId.ReplyAll, "divRA", "replyall");
			base.RenderMenuItem(output, -1428116961, ThemeFileId.Forward, "divF", "forward");
			ContextMenu.RenderMenuDivider(output, "divS2");
			base.RenderMenuItem(output, -475579318, ThemeFileId.MeetingAccept, "divMIA", null, false, null, null, MeetingInviteResponseMenu.Create(this.userContext, ResponseType.Accept));
			base.RenderMenuItem(output, 1797669216, ThemeFileId.MeetingTentative, "divMIT", null, false, null, null, MeetingInviteResponseMenu.Create(this.userContext, ResponseType.Tentative));
			base.RenderMenuItem(output, -2119870632, ThemeFileId.MeetingDecline, "divMID", null, false, null, null, MeetingInviteResponseMenu.Create(this.userContext, ResponseType.Decline));
			ContextMenu.RenderMenuDivider(output, "divS3");
			base.RenderMenuItem(output, 438661106, ThemeFileId.ForwardAsAttachment, "divFIA", "fwia");
			ContextMenu.RenderMenuDivider(output, "divS4");
			base.RenderMenuItem(output, -32068740, ThemeFileId.MarkComplete, "divMkCmp", "markcomplete");
			ContextMenu.RenderMenuDivider(output, "divS5");
			base.RenderMenuItem(output, 1381996313, ThemeFileId.Delete, "divD", "delete");
		}
	}
}
