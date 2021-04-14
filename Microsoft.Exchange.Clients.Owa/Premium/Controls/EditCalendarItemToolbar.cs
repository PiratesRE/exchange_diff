using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class EditCalendarItemToolbar : Toolbar
	{
		internal EditCalendarItemToolbar(CalendarItemBase calendarItemBase, bool isMeeting, Markup currentMarkup, bool isPublicItem)
		{
			this.isNew = (calendarItemBase == null);
			this.isMeeting = isMeeting;
			this.meetingRequestWasSent = (!this.isNew && calendarItemBase.MeetingRequestWasSent);
			this.initialMarkup = currentMarkup;
			this.importance = (this.isNew ? Importance.Normal : calendarItemBase.Importance);
			this.calendarItemType = (this.isNew ? CalendarItemType.Single : calendarItemBase.CalendarItemType);
			this.isPublicItem = isPublicItem;
			this.canEdit = (this.isNew || ItemUtility.UserCanEditItem(calendarItemBase));
			if (this.isNew || CalendarUtilities.UserCanDeleteCalendarItem(calendarItemBase) || (calendarItemBase.CalendarItemType == CalendarItemType.RecurringMaster && this.canEdit))
			{
				this.canDelete = true;
				return;
			}
			this.canDelete = false;
		}

		protected override void RenderButtons()
		{
			base.RenderHelpButton(HelpIdsLight.CalendarLight.ToString(), string.Empty);
			if (this.isMeeting)
			{
				if (this.isNew || !this.meetingRequestWasSent)
				{
					if (this.isPublicItem)
					{
						base.RenderButton(ToolbarButtons.SaveAndClose);
					}
					else
					{
						base.RenderButton(ToolbarButtons.Send);
						base.RenderButton(ToolbarButtons.SaveImageOnly);
						base.RenderButton(ToolbarButtons.SaveAndClose, ToolbarButtonFlags.Hidden);
					}
				}
				else
				{
					if (this.isPublicItem)
					{
						base.RenderButton(ToolbarButtons.SaveAndClose);
					}
					else
					{
						base.RenderButton(ToolbarButtons.SendUpdate);
						base.RenderButton(ToolbarButtons.SendCancelation, ToolbarButtonFlags.Hidden);
						base.RenderButton(ToolbarButtons.SaveAndCloseImageOnly);
					}
					if (this.isPublicItem)
					{
						base.RenderButton(ToolbarButtons.ReplyImageOnly, ToolbarButtonFlags.Disabled);
						base.RenderButton(ToolbarButtons.ReplyAllImageOnly, ToolbarButtonFlags.Disabled);
					}
					else
					{
						base.RenderButton(ToolbarButtons.ReplyAllImageOnly);
					}
				}
			}
			else
			{
				base.RenderButton(ToolbarButtons.SaveAndClose);
				base.RenderButton(ToolbarButtons.Send, ToolbarButtonFlags.Hidden);
				base.RenderButton(ToolbarButtons.SaveImageOnly, ToolbarButtonFlags.Hidden);
			}
			base.RenderButton(ToolbarButtons.AttachFile);
			base.RenderButton(ToolbarButtons.InsertImage);
			ToolbarButtonFlags flags = ToolbarButtonFlags.None;
			if (CalendarItemType.Occurrence == this.calendarItemType || CalendarItemType.Exception == this.calendarItemType)
			{
				flags = ToolbarButtonFlags.Disabled;
			}
			base.RenderButton(ToolbarButtons.RecurrenceImageOnly, flags);
			if (!this.isPublicItem)
			{
				base.RenderButton(ToolbarButtons.CheckNames, this.isMeeting ? ToolbarButtonFlags.None : ToolbarButtonFlags.Hidden);
			}
			if (base.UserContext.BrowserType == BrowserType.IE)
			{
				base.RenderButton(ToolbarButtons.SpellCheck, base.UserContext.IsFeatureEnabled(Feature.SpellChecker) ? ToolbarButtonFlags.None : ToolbarButtonFlags.Disabled, new Toolbar.RenderMenuItems(base.RenderSpellCheckLanguageDialog));
			}
			if (!this.isPublicItem && (!this.isMeeting || (this.isMeeting && !this.meetingRequestWasSent)))
			{
				base.RenderButton(ToolbarButtons.CancelInvitation, this.isMeeting ? ToolbarButtonFlags.None : ToolbarButtonFlags.Hidden);
				base.RenderButton(ToolbarButtons.InviteAttendees, this.isMeeting ? ToolbarButtonFlags.Hidden : ToolbarButtonFlags.None);
			}
			base.RenderButton(ToolbarButtons.ImportanceHigh, (this.importance == Importance.High) ? ToolbarButtonFlags.Pressed : ToolbarButtonFlags.None);
			base.RenderButton(ToolbarButtons.ImportanceLow, (this.importance == Importance.Low) ? ToolbarButtonFlags.Pressed : ToolbarButtonFlags.None);
			if (CalendarItemType.Occurrence != this.calendarItemType && CalendarItemType.Exception != this.calendarItemType)
			{
				base.RenderButton(ToolbarButtons.Categories);
			}
			this.RenderDeleteButton();
			base.RenderButton(ToolbarButtons.Print);
			if (this.initialMarkup == Markup.Html)
			{
				base.RenderHtmlTextToggle("0");
			}
			else
			{
				base.RenderHtmlTextToggle("1");
			}
			base.RenderButton(ToolbarButtons.MailTips);
		}

		private void RenderDeleteButton()
		{
			ToolbarButtonFlags flags = this.canDelete ? ToolbarButtonFlags.None : ToolbarButtonFlags.Disabled;
			if (this.isMeeting && this.meetingRequestWasSent)
			{
				base.RenderButton(ToolbarButtons.CancelMeeting, flags);
				return;
			}
			if (!this.isNew)
			{
				base.RenderButton(ToolbarButtons.Delete, flags);
			}
		}

		private bool isNew = true;

		private bool isMeeting;

		private bool meetingRequestWasSent;

		private Markup initialMarkup;

		private Importance importance = Importance.Normal;

		private CalendarItemType calendarItemType;

		private bool isPublicItem;

		private bool canEdit;

		private bool canDelete;
	}
}
