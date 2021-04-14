using System;
using System.Collections.Generic;
using Microsoft.Exchange.Connections.Eas.Model.Response.Calendar;
using Microsoft.Exchange.Connections.Eas.Model.Response.ItemOperations;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class EasFxCalendarMessage : IMessage, IDisposable
	{
		public EasFxCalendarMessage(Properties calendarItemProperties)
		{
			this.propertyBag = EasFxCalendarMessage.CreatePropertyBag(calendarItemProperties);
			this.recipients = EasFxCalendarMessage.CreateRecipientsCollection(calendarItemProperties.Attendees);
		}

		IPropertyBag IMessage.PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		bool IMessage.IsAssociated
		{
			get
			{
				return false;
			}
		}

		IEnumerable<IRecipient> IMessage.GetRecipients()
		{
			return this.recipients;
		}

		IRecipient IMessage.CreateRecipient()
		{
			throw new NotSupportedException();
		}

		void IMessage.RemoveRecipient(int rowId)
		{
			throw new NotSupportedException();
		}

		IEnumerable<IAttachmentHandle> IMessage.GetAttachments()
		{
			yield break;
		}

		IAttachment IMessage.CreateAttachment()
		{
			throw new NotSupportedException();
		}

		void IMessage.Save()
		{
			throw new NotSupportedException();
		}

		void IMessage.SetLongTermId(StoreLongTermId longTermId)
		{
			throw new NotSupportedException();
		}

		void IDisposable.Dispose()
		{
		}

		private static IEnumerable<IRecipient> CreateRecipientsCollection(List<Attendee> attendees)
		{
			List<IRecipient> list = new List<IRecipient>();
			for (int i = 0; i < attendees.Count; i++)
			{
				Attendee attendee = attendees[i];
				FxPropertyBag fxPropertyBag = new FxPropertyBag(new FxSession(SyncCalendarUtils.AttendeePropertyTagsToNamedProperties));
				fxPropertyBag[SyncCalendarUtils.RowId] = i;
				int attendeeStatus = attendee.AttendeeStatus;
				EasFxCalendarMessage.SetOrThrowIfInvalid<ResponseType>(fxPropertyBag, SyncCalendarUtils.RecipientTrackStatus, (ResponseType)attendeeStatus, attendeeStatus);
				int attendeeType = attendee.AttendeeType;
				EasFxCalendarMessage.SetOrThrowIfInvalid<AttendeeType>(fxPropertyBag, SyncCalendarUtils.RecipientType, (AttendeeType)attendeeType, attendeeType);
				fxPropertyBag[SyncCalendarUtils.EmailAddress] = attendee.Email;
				fxPropertyBag[SyncCalendarUtils.DisplayName] = attendee.Name;
				EasFxCalendarRecipient item = new EasFxCalendarRecipient(fxPropertyBag);
				list.Add(item);
			}
			return list;
		}

		private static FxPropertyBag CreatePropertyBag(Properties calendarItemProperties)
		{
			FxPropertyBag fxPropertyBag = new FxPropertyBag(new FxSession(SyncCalendarUtils.CalendarItemPropertyTagsToNamedProperties));
			ExDateTime exDateTime = SyncCalendarUtils.ToUtcExDateTime(calendarItemProperties.StartTime);
			ExDateTime exDateTime2 = SyncCalendarUtils.ToUtcExDateTime(calendarItemProperties.EndTime);
			if (exDateTime > exDateTime2)
			{
				throw new EasFetchFailedPermanentException(new LocalizedString(string.Format("Start {0} is greater than end {1}.", exDateTime, exDateTime2)));
			}
			fxPropertyBag[SyncCalendarUtils.Start] = exDateTime;
			fxPropertyBag[SyncCalendarUtils.End] = exDateTime2;
			fxPropertyBag[SyncCalendarUtils.GlobalObjectId] = new GlobalObjectId(calendarItemProperties.Uid).Bytes;
			ExTimeZone exTimeZone = SyncCalendarUtils.ToExTimeZone(calendarItemProperties.TimeZone);
			fxPropertyBag[SyncCalendarUtils.TimeZoneBlob] = O11TimeZoneFormatter.GetTimeZoneBlob(exTimeZone);
			int busyStatus = calendarItemProperties.BusyStatus;
			EasFxCalendarMessage.SetOrThrowIfInvalid<BusyType>(fxPropertyBag, SyncCalendarUtils.BusyStatus, (BusyType)busyStatus, busyStatus);
			int sensitivity = calendarItemProperties.Sensitivity;
			EasFxCalendarMessage.SetOrThrowIfInvalid<Sensitivity>(fxPropertyBag, SyncCalendarUtils.Sensitivity, (Sensitivity)sensitivity, sensitivity);
			int meetingStatus = calendarItemProperties.MeetingStatus;
			EasFxCalendarMessage.SetOrThrowIfInvalid<AppointmentStateFlags>(fxPropertyBag, SyncCalendarUtils.MeetingStatus, (AppointmentStateFlags)meetingStatus, meetingStatus);
			fxPropertyBag[PropertyTag.MessageClass] = "IPM.Appointment";
			fxPropertyBag[PropertyTag.Subject] = calendarItemProperties.CalendarSubject;
			fxPropertyBag[SyncCalendarUtils.AllDayEvent] = calendarItemProperties.AllDayEvent;
			fxPropertyBag[SyncCalendarUtils.Location] = calendarItemProperties.Location;
			fxPropertyBag[SyncCalendarUtils.Reminder] = calendarItemProperties.Reminder;
			fxPropertyBag[PropertyTag.Body] = calendarItemProperties.Body.Data;
			fxPropertyBag[SyncCalendarUtils.SentRepresentingName] = calendarItemProperties.OrganizerName;
			fxPropertyBag[SyncCalendarUtils.SentRepresentingEmailAddress] = calendarItemProperties.OrganizerEmail;
			fxPropertyBag[SyncCalendarUtils.ResponseType] = calendarItemProperties.ResponseType;
			Recurrence recurrence = calendarItemProperties.Recurrence;
			if (recurrence != null)
			{
				fxPropertyBag[SyncCalendarUtils.AppointmentRecurrenceBlob] = SyncCalendarUtils.ToRecurrenceBlob(calendarItemProperties, exDateTime, exDateTime2, exTimeZone);
			}
			return fxPropertyBag;
		}

		private static void SetOrThrowIfInvalid<T>(FxPropertyBag propertyBag, PropertyTag propertyTag, T typedValue, int valueToSet) where T : struct
		{
			if (!EnumValidator<T>.IsValidValue(typedValue))
			{
				throw new EasFetchFailedPermanentException(new LocalizedString(propertyTag + ": " + typedValue));
			}
			propertyBag[propertyTag] = valueToSet;
		}

		private readonly IPropertyBag propertyBag;

		private readonly IEnumerable<IRecipient> recipients;
	}
}
