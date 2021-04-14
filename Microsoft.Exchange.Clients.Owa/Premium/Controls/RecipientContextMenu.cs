using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class RecipientContextMenu : ContextMenu
	{
		public RecipientContextMenu(UserContext userContext, RecipientJunkEmailContextMenuType recipientJunkEmailContextMenuType) : base("divRm", userContext)
		{
			this.recipientJunkEmailContextMenuType = recipientJunkEmailContextMenuType;
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			bool flag = this.userContext.IsInstantMessageEnabled();
			base.RenderHeader(output);
			base.RenderNoOpMenuItem(output, "divAdOf", -14224391, "spnAdOf");
			base.RenderNoOpMenuItem(output, "divAdPh", 1064004649, "spnAdPh");
			base.RenderNoOpMenuItem(output, "divCWPh", 913324763, "spnCWPh");
			base.RenderNoOpMenuItem(output, "divCHPh", 1918389986, "spnCHPh");
			base.RenderNoOpMenuItem(output, "divCMPh", -207426239, "spnCMPh");
			ContextMenu.RenderMenuDivider(output, "divEmailPropDivider");
			if (flag)
			{
				base.RenderMenuItem(output, -124986716, ThemeFileId.Chat, "divCht", "chat");
			}
			base.RenderMenuItem(output, 2130777130, ThemeFileId.EMailContact, "divNwEml", "nweml");
			if (base.UserContext.IsSmsEnabled)
			{
				base.RenderMenuItem(output, -843330244, ThemeFileId.Sms, "divSndSms", "sendsms");
			}
			if (this.userContext.IsFeatureEnabled(Feature.Calendar))
			{
				base.RenderMenuItem(output, -57938456, ThemeFileId.Appointment, "divNwMtg", "nwmtg");
			}
			ContextMenu.RenderMenuDivider(output, "divNewActionDivider");
			if (flag)
			{
				base.RenderMenuItem(output, -127081077, ThemeFileId.AddBuddy, "divAdBdyLst", "ablst");
				base.RenderMenuItem(output, -564556103, ThemeFileId.RemoveBuddy, "divRmBdyLst", "rmblst");
				ContextMenu.RenderMenuDivider(output, "divInstantMessagingDivider");
			}
			if (this.userContext.IsFeatureEnabled(Feature.Contacts))
			{
				base.RenderMenuItem(output, 5908146, ThemeFileId.Contact, "divAC", "ac");
			}
			base.RenderMenuItem(output, -186712112, ThemeFileId.None, "divPr", "prps");
			base.RenderMenuItem(output, 1388922078, ThemeFileId.None, "divRmv", "rmv");
			if (this.userContext.IsJunkEmailEnabled && this.recipientJunkEmailContextMenuType != RecipientJunkEmailContextMenuType.None)
			{
				if ((this.recipientJunkEmailContextMenuType & RecipientJunkEmailContextMenuType.Sender) != RecipientJunkEmailContextMenuType.None)
				{
					base.RenderMenuItem(output, -2053927452, ThemeFileId.JunkEMail, "divJnkSender", null, false, null, null, JunkEmailContextMenu.Create(this.userContext, JunkEmailContextMenuType.Sender));
				}
				if ((this.recipientJunkEmailContextMenuType & RecipientJunkEmailContextMenuType.Recipient) != RecipientJunkEmailContextMenuType.None)
				{
					base.RenderMenuItem(output, -2053927452, ThemeFileId.JunkEMail, "divJnkRecipient", null, false, null, null, JunkEmailContextMenu.Create(this.userContext, JunkEmailContextMenuType.Recipient));
				}
			}
		}

		private void RenderMenuItemInnerSpan(TextWriter output, ContextMenu.RenderMenuItemHtml renderMenuItemHtml)
		{
			base.RenderMenuItemInnerSpan(output, null, null, renderMenuItemHtml, false);
		}

		private RecipientJunkEmailContextMenuType recipientJunkEmailContextMenuType;
	}
}
