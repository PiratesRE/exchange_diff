using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class AddEventToMyCalendarCommandAction : CalendarActionBase<CalendarActionResponse>
	{
		public AddEventToMyCalendarCommandAction(MailboxSession groupSession, ExchangePrincipal userPrincipal, CultureInfo cultureInfo, Item forwardingItem) : base(groupSession)
		{
			if (groupSession == null)
			{
				throw new ArgumentNullException("groupSession");
			}
			if (userPrincipal == null)
			{
				throw new ArgumentNullException("userPrincipal");
			}
			if (cultureInfo == null)
			{
				throw new ArgumentNullException("cultureInfo");
			}
			if (forwardingItem == null)
			{
				throw new ArgumentNullException("forwardingItem");
			}
			this.userPrincipal = userPrincipal;
			this.clientCultureInfo = cultureInfo;
			this.forwardingItem = forwardingItem;
		}

		public override CalendarActionResponse Execute()
		{
			return this.SelfForwardMessageItem();
		}

		private CalendarActionResponse SelfForwardMessageItem()
		{
			CalendarActionResponse calendarActionResponse = new CalendarActionResponse();
			MailboxSession mailboxSession = base.MailboxSession;
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar);
			CalendarItemBase calendarItemBase = null;
			MessageItem messageItem = null;
			if (!XsoDataConverter.TryGetStoreObject<CalendarItemBase>(this.forwardingItem, out calendarItemBase) && !XsoDataConverter.TryGetStoreObject<MessageItem>(this.forwardingItem, out messageItem))
			{
				AddEventToMyCalendarCommand.TraceError(this.GetHashCode(), "AddEventToMyCalendarCommandAction.SelfForwardMessageItem(): The given item: {0} is not a MeessageItem type", new object[]
				{
					this.forwardingItem
				});
				calendarActionResponse.WasSuccessful = false;
				calendarActionResponse.ErrorCode = CalendarActionError.CalendarActionInvalidItemId;
				return calendarActionResponse;
			}
			ReplyForwardConfiguration replyForwardConfiguration = new ReplyForwardConfiguration(this.clientCultureInfo);
			replyForwardConfiguration.SubjectPrefix = string.Empty;
			AddEventToMyCalendarCommand.TraceDebug(this.GetHashCode(), "AddEventToMyCalendarCommandAction.SelfForwardMessageItem(): Create a forward item for: {0} and send it.", new object[]
			{
				calendarItemBase ?? messageItem
			});
			using (MessageItem messageItem2 = (calendarItemBase != null) ? calendarItemBase.CreateForward(mailboxSession, defaultFolderId, replyForwardConfiguration, null, null) : messageItem.CreateForward(mailboxSession, defaultFolderId, replyForwardConfiguration))
			{
				messageItem2.Recipients.Add(new Participant(this.userPrincipal));
				AppointmentAuxiliaryFlags? valueAsNullable = messageItem2.GetValueAsNullable<AppointmentAuxiliaryFlags>(MeetingMessageSchema.AppointmentAuxiliaryFlags);
				messageItem2[MeetingMessageSchema.AppointmentAuxiliaryFlags] = ((valueAsNullable != null) ? (valueAsNullable.Value | AppointmentAuxiliaryFlags.EventAddedFromGroupCalendar) : AppointmentAuxiliaryFlags.EventAddedFromGroupCalendar);
				messageItem2.Send();
				calendarActionResponse.WasSuccessful = true;
				calendarActionResponse.ErrorCode = CalendarActionError.None;
			}
			return calendarActionResponse;
		}

		private readonly ExchangePrincipal userPrincipal;

		private readonly CultureInfo clientCultureInfo;

		private readonly Item forwardingItem;
	}
}
