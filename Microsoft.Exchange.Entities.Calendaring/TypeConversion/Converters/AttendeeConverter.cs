using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal class AttendeeConverter : ParticipantConverter<Attendee, AttendeeParticipantWrapper, Attendee>
	{
		public AttendeeConverter(IResponseTypeConverter responseTypeConverter, IAttendeeTypeConverter attendeeTypeConverter, ICalendarItemBase calendarItem) : base(new ParticipantRoutingTypeConverter(calendarItem.AssertNotNull("calendarItem").Session))
		{
			this.includeStatus = calendarItem.IsOrganizer();
			this.responseTypeConverter = responseTypeConverter;
			this.attendeeTypeConverter = attendeeTypeConverter;
		}

		protected virtual bool IncludeStatus
		{
			get
			{
				return this.includeStatus;
			}
		}

		public void ToXso(IList<Attendee> attendees, ICalendarItemBase calendarItem)
		{
			if (calendarItem == null)
			{
				throw new ExArgumentNullException("calendarItem");
			}
			if (calendarItem.IsOrganizer())
			{
				IAttendeeCollection attendeeCollection = calendarItem.AttendeeCollection;
				attendeeCollection.Clear();
				AttendeeConverter.StorageAttendeeData[] array;
				Participant[] array2;
				if (this.TryGetAttendeesData(calendarItem.Session, attendees, out array, out array2))
				{
					for (int i = 0; i < array2.Length; i++)
					{
						AttendeeConverter.StorageAttendeeData storageAttendeeData = array[i];
						calendarItem.AttendeeCollection.Add(array2[i], storageAttendeeData.AttendeeType, storageAttendeeData.ResponseType, storageAttendeeData.ReplyTime, true);
					}
				}
				if (calendarItem.AttendeeCollection.Count > 0)
				{
					calendarItem.IsMeeting = true;
				}
			}
		}

		protected override AttendeeParticipantWrapper WrapStorageType(Attendee value)
		{
			return new AttendeeParticipantWrapper(value);
		}

		protected override Attendee CopyFromParticipant(Participant value, Attendee original)
		{
			Attendee attendee = base.CopyFromParticipant(value, original);
			if (attendee != null)
			{
				attendee.Type = this.attendeeTypeConverter.Convert(original.AttendeeType);
				if (this.IncludeStatus)
				{
					this.CopyStatusProperties(original, attendee);
				}
			}
			return attendee;
		}

		protected virtual void CopyStatusProperties(Attendee source, Attendee destination)
		{
			destination.Status = new ResponseStatus
			{
				Response = this.responseTypeConverter.Convert(source.ResponseType)
			};
			ExDateTime time;
			if (AttendeeAccessors.ReplyTime.TryGetValue(source, out time))
			{
				destination.Status.Time = time;
			}
		}

		protected virtual AttendeeConverter.StorageAttendeeData GetAttendeeData(Attendee attendee, out Participant participant)
		{
			if (attendee == null)
			{
				throw new ExArgumentNullException("attendee");
			}
			attendee.ThrowIfPropertyNotSet(attendee.Schema.TypeProperty);
			string routingType = base.GetRoutingType(attendee);
			participant = new Participant(attendee.Name, attendee.EmailAddress, routingType);
			AttendeeConverter.StorageAttendeeData result = new AttendeeConverter.StorageAttendeeData
			{
				AttendeeType = this.attendeeTypeConverter.Convert(attendee.Type)
			};
			if (attendee.IsPropertySet(attendee.Schema.StatusProperty) && attendee.Status != null)
			{
				ResponseStatus status = attendee.Status;
				if (status.IsPropertySet(status.Schema.ResponseProperty))
				{
					result.ResponseType = new ResponseType?(this.responseTypeConverter.Convert(status.Response));
				}
				if (status.IsPropertySet(status.Schema.TimeProperty))
				{
					result.ReplyTime = new ExDateTime?(status.Time);
				}
			}
			return result;
		}

		protected virtual bool TryGetAttendeesData(IStoreSession session, IList<Attendee> attendees, out AttendeeConverter.StorageAttendeeData[] data, out Participant[] convertedParticipants)
		{
			if (attendees == null)
			{
				data = null;
				convertedParticipants = null;
				return false;
			}
			int count = attendees.Count;
			data = new AttendeeConverter.StorageAttendeeData[count];
			if (count == 0)
			{
				convertedParticipants = new Participant[0];
				return false;
			}
			Participant[] array = new Participant[count];
			for (int i = 0; i < count; i++)
			{
				try
				{
					data[i] = this.GetAttendeeData(attendees[i], out array[i]);
				}
				catch (ExArgumentNullException innerException)
				{
					throw new InvalidRequestException(CalendaringStrings.ErrorInvalidAttendee, innerException);
				}
			}
			convertedParticipants = this.RoutingTypeConverter.ConvertToStorage(array);
			return true;
		}

		private readonly bool includeStatus;

		private readonly IAttendeeTypeConverter attendeeTypeConverter;

		private readonly IResponseTypeConverter responseTypeConverter;

		public struct StorageAttendeeData
		{
			public Microsoft.Exchange.Data.Storage.AttendeeType AttendeeType { get; set; }

			public ExDateTime? ReplyTime { get; set; }

			public ResponseType? ResponseType { get; set; }
		}
	}
}
