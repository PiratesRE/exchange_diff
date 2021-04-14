using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class ReadMessageToolbar : Toolbar
	{
		internal ReadMessageToolbar(bool isInDeleteItems, bool isEmbeddedItem, Item message, bool isInJunkEmailFolder, bool isSuspectedPhishingItem, bool isLinkEnabled) : this(isInDeleteItems, isEmbeddedItem, message, isInJunkEmailFolder, isSuspectedPhishingItem, isLinkEnabled, false, false, false, false, false)
		{
		}

		internal ReadMessageToolbar(bool isInDeleteItems, bool isEmbeddedItem, Item message, bool isInJunkEmailFolder, bool isSuspectedPhishingItem, bool isLinkEnabled, bool isMessageReadForm, bool isReplyRestricted, bool isReplyAllRestricted, bool isForwardRestricted, bool isPrintRestricted) : base(ToolbarType.Form)
		{
			this.isInDeleteItems = isInDeleteItems;
			this.isEmbeddedItem = isEmbeddedItem;
			if (message == null)
			{
				throw new ArgumentException("message must not be null");
			}
			this.message = message;
			this.isInJunkEmailFolder = isInJunkEmailFolder;
			this.isSuspectedPhishingItem = isSuspectedPhishingItem;
			this.isLinkEnabled = isLinkEnabled;
			this.isMessageReadForm = isMessageReadForm;
			this.isReplyRestricted = isReplyRestricted;
			this.isReplyAllRestricted = isReplyAllRestricted;
			this.isForwardRestricted = isForwardRestricted;
			this.isPrintRestricted = isPrintRestricted;
		}

		protected override bool IsNarrow
		{
			get
			{
				return this.isMessageReadForm;
			}
		}

		protected override void RenderButtons()
		{
			ToolbarButtonFlags flags = this.isEmbeddedItem ? ToolbarButtonFlags.Disabled : ToolbarButtonFlags.None;
			bool flag = Utilities.IsPublic(this.message);
			bool flag2 = Utilities.IsOtherMailbox(this.message);
			base.RenderHelpButton(ObjectClass.IsSmsMessage(this.message.ClassName) ? HelpIdsLight.DefaultLight.ToString() : HelpIdsLight.MailLight.ToString(), string.Empty);
			if (ItemUtility.ShouldRenderSendAgain(this.message, this.isEmbeddedItem) && !flag2)
			{
				base.RenderButton(ToolbarButtons.SendAgain, flag ? ToolbarButtonFlags.Disabled : ToolbarButtonFlags.None);
			}
			if (this.isInJunkEmailFolder && base.UserContext.IsJunkEmailEnabled && !this.isSuspectedPhishingItem)
			{
				base.RenderButton(ToolbarButtons.NotJunk);
			}
			ToolbarButtonFlags flags2 = ToolbarButtonFlags.None;
			ToolbarButtonFlags flags3 = ToolbarButtonFlags.None;
			if (this.isInJunkEmailFolder || (this.isSuspectedPhishingItem && !this.isLinkEnabled))
			{
				flags2 = ToolbarButtonFlags.Disabled;
				flags3 = ToolbarButtonFlags.Disabled;
			}
			if (!base.UserContext.IsFeatureEnabled(Feature.Tasks) && ObjectClass.IsOfClass(this.message.ClassName, "IPM.Task"))
			{
				flags2 = ToolbarButtonFlags.Disabled;
				flags3 = ToolbarButtonFlags.Disabled;
			}
			bool flag3 = ReadMessageToolbar.IsReplySupported(this.message);
			bool flag4 = base.UserContext.IsSmsEnabled && ObjectClass.IsSmsMessage(this.message.ClassName);
			if (this.isReplyRestricted)
			{
				flags2 = ToolbarButtonFlags.Disabled;
			}
			if (this.isReplyAllRestricted)
			{
				flags3 = ToolbarButtonFlags.Disabled;
			}
			if (flag3)
			{
				base.RenderButton(flag4 ? ToolbarButtons.ReplySms : ToolbarButtons.Reply, flags2);
				base.RenderButton(flag4 ? ToolbarButtons.ReplyAllSms : ToolbarButtons.ReplyAll, flags3);
			}
			ToolbarButtonFlags flags4 = ToolbarButtonFlags.None;
			if (ObjectClass.IsOfClass(this.message.ClassName, "IPM.Note.Microsoft.Approval.Request") || this.isForwardRestricted || this.isInJunkEmailFolder || (this.isSuspectedPhishingItem && !this.isLinkEnabled))
			{
				flags4 = ToolbarButtonFlags.Disabled;
			}
			if (!ObjectClass.IsOfClass(this.message.ClassName, "IPM.Conflict.Message"))
			{
				base.RenderButton(flag4 ? ToolbarButtons.ForwardSms : ToolbarButtons.Forward, flags4);
			}
			bool flag5 = this.message is CalendarItemBase;
			bool flag6 = ItemUtility.UserCanEditItem(this.message);
			if (base.UserContext.IsInstantMessageEnabled() && (!flag5 || (flag5 && flag3)))
			{
				base.RenderButton(ToolbarButtons.Chat, ToolbarButtonFlags.Disabled);
			}
			MessageItem messageItem = this.message as MessageItem;
			bool flag7 = messageItem != null && messageItem.IsDraft;
			ToolbarButtonFlags flags5 = (flag6 && !flag7) ? ToolbarButtonFlags.None : ToolbarButtonFlags.Disabled;
			if (!this.isEmbeddedItem && base.UserContext.ExchangePrincipal.RecipientTypeDetails == RecipientTypeDetails.DiscoveryMailbox)
			{
				base.RenderButton(ToolbarButtons.MessageNoteInToolbar);
			}
			if (!flag5 && !this.isInDeleteItems && !this.isEmbeddedItem)
			{
				base.RenderButton(ToolbarButtons.Flag, flags5);
			}
			ToolbarButtonFlags flags6 = flag6 ? ToolbarButtonFlags.None : ToolbarButtonFlags.Disabled;
			if (!this.isEmbeddedItem)
			{
				bool flag8 = true;
				if (flag5)
				{
					CalendarItemBase calendarItemBase = (CalendarItemBase)this.message;
					flag8 = (CalendarItemType.Occurrence != calendarItemBase.CalendarItemType && CalendarItemType.Exception != calendarItemBase.CalendarItemType);
				}
				if (flag8)
				{
					base.RenderButton(ToolbarButtons.Categories, flags6);
				}
			}
			if (!flag5)
			{
				base.RenderButton(ToolbarButtons.MessageDetails);
			}
			base.RenderButton(ToolbarButtons.Print, this.isPrintRestricted ? ToolbarButtonFlags.Disabled : ToolbarButtonFlags.None);
			if (base.UserContext.IsFeatureEnabled(Feature.Rules) && this.isMessageReadForm && !flag2 && !this.isEmbeddedItem)
			{
				base.RenderButton(ToolbarButtons.CreateRule, (base.UserContext.IsWebPartRequest || flag) ? ToolbarButtonFlags.Disabled : ToolbarButtonFlags.None);
			}
			bool flag9;
			if (flag5)
			{
				flag9 = CalendarUtilities.UserCanDeleteCalendarItem((CalendarItemBase)this.message);
			}
			else
			{
				flag9 = ItemUtility.UserCanDeleteItem(this.message);
			}
			ToolbarButtonFlags flags7 = (!flag9 || this.isEmbeddedItem) ? ToolbarButtonFlags.Disabled : ToolbarButtonFlags.None;
			base.RenderButton(ToolbarButtons.Delete, flags7);
			if (!this.isEmbeddedItem && !flag5)
			{
				base.RenderButton(ToolbarButtons.Move);
			}
			if (!flag5)
			{
				base.RenderButton(ToolbarButtons.Previous, flags);
				base.RenderButton(ToolbarButtons.Next, flags);
			}
		}

		private static bool IsReplySupported(Item item)
		{
			MessageItem messageItem;
			if ((messageItem = (item as MessageItem)) != null)
			{
				return messageItem.IsReplyAllowed;
			}
			CalendarItemBase calendarItemBase;
			return (calendarItemBase = (item as CalendarItemBase)) != null && calendarItemBase.IsMeeting;
		}

		private Item message;

		private bool isEmbeddedItem;

		private bool isInDeleteItems;

		private bool isMessageReadForm;

		private bool isInJunkEmailFolder;

		private bool isSuspectedPhishingItem;

		private bool isLinkEnabled = true;

		private bool isReplyRestricted;

		private bool isReplyAllRestricted;

		private bool isForwardRestricted;

		private bool isPrintRestricted;
	}
}
