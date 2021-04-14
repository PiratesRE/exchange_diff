using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Common;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CalendarItemConverter
	{
		internal void ConvertItem(CalendarItemBase localItem, CalendarItemType remoteItem)
		{
			if (localItem == null)
			{
				throw new ArgumentNullException("localItem");
			}
			if (remoteItem == null)
			{
				throw new ArgumentNullException("remoteItem");
			}
			Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "{0}: Copying Subject: {1}, Location: {2}, StartTime: {3}, EndTime: {4}", new object[]
			{
				this,
				remoteItem.Subject,
				remoteItem.Location,
				remoteItem.Start,
				remoteItem.End
			});
			ExTimeZone exTimeZone = this.ConvertTimeZone(remoteItem.StartTimeZone);
			ExTimeZone exTimeZone2 = this.ConvertTimeZone(remoteItem.EndTimeZone);
			localItem.StartTime = new ExDateTime(exTimeZone, this.NormalizeDateToUtc(remoteItem.Start));
			localItem.StartTimeZone = exTimeZone;
			localItem.EndTime = new ExDateTime(exTimeZone2, this.NormalizeDateToUtc(remoteItem.End));
			localItem.EndTimeZone = exTimeZone2;
			localItem.ResponseType = this.ConvertToResponseType(remoteItem.MyResponseType);
			localItem[ItemSchema.IsResponseRequested] = remoteItem.IsResponseRequested;
			localItem[CalendarItemBaseSchema.AppointmentSequenceNumber] = remoteItem.AppointmentSequenceNumber;
			localItem[CalendarItemBaseSchema.AppointmentState] = remoteItem.AppointmentState;
			if (remoteItem.TimeZone != null)
			{
				localItem[CalendarItemBaseSchema.TimeZone] = remoteItem.TimeZone;
			}
			if (remoteItem.Subject != null)
			{
				localItem.Subject = remoteItem.Subject;
			}
			if (remoteItem.Location != null)
			{
				localItem.Location = remoteItem.Location;
			}
			DateTime appointmentReplyTime = remoteItem.AppointmentReplyTime;
			localItem[CalendarItemBaseSchema.AppointmentReplyTime] = new ExDateTime(ExTimeZone.UtcTimeZone, this.NormalizeDateToUtc(remoteItem.AppointmentReplyTime));
			DateTime lastModifiedTime = remoteItem.LastModifiedTime;
			localItem[StoreObjectSchema.LastModifiedTime] = new ExDateTime(ExTimeZone.UtcTimeZone, this.NormalizeDateToUtc(remoteItem.LastModifiedTime));
			if (remoteItem.RequiredAttendees != null)
			{
				foreach (Microsoft.Exchange.SoapWebClient.EWS.AttendeeType attendeeType in remoteItem.RequiredAttendees)
				{
					if (attendeeType.Mailbox != null)
					{
						Participant participant = new Participant(attendeeType.Mailbox.Name, attendeeType.Mailbox.EmailAddress, attendeeType.Mailbox.RoutingType);
						ResponseType value = this.ConvertToResponseType(attendeeType.ResponseType);
						localItem.AttendeeCollection.Add(participant, Microsoft.Exchange.Data.Storage.AttendeeType.Required, new ResponseType?(value), null, false);
					}
				}
			}
			if (remoteItem.OptionalAttendees != null)
			{
				foreach (Microsoft.Exchange.SoapWebClient.EWS.AttendeeType attendeeType2 in remoteItem.OptionalAttendees)
				{
					if (attendeeType2.Mailbox != null)
					{
						Participant participant2 = new Participant(attendeeType2.Mailbox.Name, attendeeType2.Mailbox.EmailAddress, attendeeType2.Mailbox.RoutingType);
						ResponseType value2 = this.ConvertToResponseType(attendeeType2.ResponseType);
						localItem.AttendeeCollection.Add(participant2, Microsoft.Exchange.Data.Storage.AttendeeType.Optional, new ResponseType?(value2), null, false);
					}
				}
			}
			if (remoteItem.ExtendedProperty != null)
			{
				foreach (ExtendedPropertyType extendedPropertyType in remoteItem.ExtendedProperty)
				{
					if (extendedPropertyType.ExtendedFieldURI != null && extendedPropertyType.Item != null)
					{
						Globals.ConsistencyChecksTracer.TraceDebug<CalendarItemConverter, int, object>((long)this.GetHashCode(), "{0}: Copying ExtendedProperty: PropertyId {1}, Value: {2}", this, extendedPropertyType.ExtendedFieldURI.PropertyId, extendedPropertyType.Item);
						if (string.Compare(extendedPropertyType.ExtendedFieldURI.PropertySetId, CalendarItemFields.PSETIDMeeting.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
						{
							bool flag;
							if (extendedPropertyType.ExtendedFieldURI.PropertyId == CalendarItemFields.CleanGlobalObjectIdProp.PropertyId && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.CleanGlobalObjectIdProp.PropertyType)
							{
								localItem[CalendarItemBaseSchema.CleanGlobalObjectId] = Convert.FromBase64String((string)extendedPropertyType.Item);
							}
							else if (extendedPropertyType.ExtendedFieldURI.PropertyId == CalendarItemFields.GlobalObjectIdProp.PropertyId && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.GlobalObjectIdProp.PropertyType)
							{
								localItem[CalendarItemBaseSchema.GlobalObjectId] = new GlobalObjectId(Convert.FromBase64String((string)extendedPropertyType.Item)).Bytes;
							}
							else if (extendedPropertyType.ExtendedFieldURI.PropertyId == CalendarItemFields.OwnerCriticalChangeTimeProp.PropertyId && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.OwnerCriticalChangeTimeProp.PropertyType)
							{
								DateTime dateTime;
								if (DateTime.TryParse((string)extendedPropertyType.Item, out dateTime))
								{
									localItem[CalendarItemBaseSchema.OwnerCriticalChangeTime] = new ExDateTime(ExTimeZone.UtcTimeZone, this.NormalizeDateToUtc(dateTime));
								}
							}
							else if (extendedPropertyType.ExtendedFieldURI.PropertyId == CalendarItemFields.AttendeeCriticalChangeTimeProp.PropertyId && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.AttendeeCriticalChangeTimeProp.PropertyType)
							{
								DateTime dateTime2;
								if (DateTime.TryParse((string)extendedPropertyType.Item, out dateTime2))
								{
									localItem[CalendarItemBaseSchema.AttendeeCriticalChangeTime] = new ExDateTime(ExTimeZone.UtcTimeZone, this.NormalizeDateToUtc(dateTime2));
								}
							}
							else if (extendedPropertyType.ExtendedFieldURI.PropertyId == CalendarItemFields.IsExceptionProp.PropertyId && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.IsExceptionProp.PropertyType && bool.TryParse((string)extendedPropertyType.Item, out flag))
							{
								localItem[CalendarItemBaseSchema.IsException] = flag;
							}
						}
						else if (string.Compare(extendedPropertyType.ExtendedFieldURI.PropertySetId, CalendarItemFields.PSETIDAppointment.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
						{
							bool flag2;
							if (extendedPropertyType.ExtendedFieldURI.PropertyId == CalendarItemFields.AppointmentExtractTimeProp.PropertyId && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.AppointmentExtractTimeProp.PropertyType)
							{
								DateTime dateTime3;
								if (DateTime.TryParse((string)extendedPropertyType.Item, out dateTime3))
								{
									localItem[CalendarItemBaseSchema.AppointmentExtractTime] = new ExDateTime(ExTimeZone.UtcTimeZone, this.NormalizeDateToUtc(dateTime3));
								}
							}
							else if (extendedPropertyType.ExtendedFieldURI.PropertyId == CalendarItemFields.AppointmentExtractVersionProp.PropertyId && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.AppointmentExtractVersionProp.PropertyType)
							{
								long num;
								if (long.TryParse((string)extendedPropertyType.Item, out num))
								{
									localItem[CalendarItemBaseSchema.AppointmentExtractVersion] = num;
								}
							}
							else if (extendedPropertyType.ExtendedFieldURI.PropertyId == CalendarItemFields.AppointmentRecurrenceBlobProp.PropertyId && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.AppointmentRecurrenceBlobProp.PropertyType)
							{
								localItem[CalendarItemBaseSchema.AppointmentRecurrenceBlob] = Convert.FromBase64String((string)extendedPropertyType.Item);
							}
							else if (extendedPropertyType.ExtendedFieldURI.PropertyId == CalendarItemFields.TimeZoneBlobProp.PropertyId && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.TimeZoneBlobProp.PropertyType)
							{
								localItem[CalendarItemBaseSchema.TimeZoneBlob] = Convert.FromBase64String((string)extendedPropertyType.Item);
							}
							else if (extendedPropertyType.ExtendedFieldURI.PropertyId == CalendarItemFields.TimeZoneDefinitionStartProp.PropertyId && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.TimeZoneDefinitionStartProp.PropertyType)
							{
								localItem[ItemSchema.TimeZoneDefinitionStart] = Convert.FromBase64String((string)extendedPropertyType.Item);
							}
							else if (extendedPropertyType.ExtendedFieldURI.PropertyId == CalendarItemFields.TimeZoneDefinitionEndProp.PropertyId && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.TimeZoneDefinitionEndProp.PropertyType)
							{
								localItem[CalendarItemBaseSchema.TimeZoneDefinitionEnd] = Convert.FromBase64String((string)extendedPropertyType.Item);
							}
							else if (extendedPropertyType.ExtendedFieldURI.PropertyId == CalendarItemFields.TimeZoneDefinitionRecurringProp.PropertyId && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.TimeZoneDefinitionRecurringProp.PropertyType)
							{
								localItem[CalendarItemBaseSchema.TimeZoneDefinitionRecurring] = Convert.FromBase64String((string)extendedPropertyType.Item);
							}
							else if (extendedPropertyType.ExtendedFieldURI.PropertyId == CalendarItemFields.AppointmentRecurringProp.PropertyId && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.AppointmentRecurringProp.PropertyType && bool.TryParse((string)extendedPropertyType.Item, out flag2))
							{
								localItem[CalendarItemBaseSchema.AppointmentRecurring] = flag2;
							}
						}
						else if (string.Compare(extendedPropertyType.ExtendedFieldURI.PropertySetId, CalendarItemFields.PSETIDCalendarAssistant.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
						{
							int num2;
							if (extendedPropertyType.ExtendedFieldURI.PropertyId == CalendarItemFields.ItemVersionProp.PropertyId && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.ItemVersionProp.PropertyType && int.TryParse((string)extendedPropertyType.Item, out num2))
							{
								localItem[CalendarItemBaseSchema.ItemVersion] = num2;
							}
						}
						else
						{
							int num3 = Convert.ToInt32(extendedPropertyType.ExtendedFieldURI.PropertyTag, 16);
							if (num3 == Convert.ToInt32(CalendarItemFields.OwnerAppointmentIDProp.PropertyTag, 16) && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.OwnerAppointmentIDProp.PropertyType)
							{
								int num4;
								if (int.TryParse((string)extendedPropertyType.Item, out num4))
								{
									localItem[CalendarItemBaseSchema.OwnerAppointmentID] = num4;
								}
							}
							else if (num3 == Convert.ToInt32(CalendarItemFields.CreationTimeProp.PropertyTag, 16) && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.CreationTimeProp.PropertyType)
							{
								DateTime dateTime4;
								if (DateTime.TryParse((string)extendedPropertyType.Item, out dateTime4))
								{
									localItem[StoreObjectSchema.CreationTime] = dateTime4;
								}
							}
							else if (num3 == Convert.ToInt32(CalendarItemFields.ItemClassProp.PropertyTag, 16) && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.ItemClassProp.PropertyType)
							{
								localItem[StoreObjectSchema.ItemClass] = (string)extendedPropertyType.Item;
							}
						}
					}
				}
			}
		}

		internal ResponseType ConvertToResponseType(ResponseTypeType responseType)
		{
			switch (responseType)
			{
			case ResponseTypeType.Organizer:
				return ResponseType.Organizer;
			case ResponseTypeType.Tentative:
				return ResponseType.Tentative;
			case ResponseTypeType.Accept:
				return ResponseType.Accept;
			case ResponseTypeType.Decline:
				return ResponseType.Decline;
			case ResponseTypeType.NoResponseReceived:
				return ResponseType.NotResponded;
			}
			return ResponseType.None;
		}

		internal MeetingInquiryAction ConvertToMeetingInquiryAction(ClientIntentMeetingInquiryActionType action)
		{
			switch (action)
			{
			case ClientIntentMeetingInquiryActionType.SendCancellation:
				return MeetingInquiryAction.SendCancellation;
			case ClientIntentMeetingInquiryActionType.ReviveMeeting:
				return MeetingInquiryAction.ReviveMeeting;
			case ClientIntentMeetingInquiryActionType.SendUpdateForMaster:
				return MeetingInquiryAction.SendUpdateForMaster;
			case ClientIntentMeetingInquiryActionType.MeetingAlreadyExists:
				return MeetingInquiryAction.MeetingAlreadyExists;
			case ClientIntentMeetingInquiryActionType.ExistingOccurrence:
				return MeetingInquiryAction.ExistingOccurrence;
			case ClientIntentMeetingInquiryActionType.HasDelegates:
				return MeetingInquiryAction.HasDelegates;
			case ClientIntentMeetingInquiryActionType.PairedCancellationFound:
				return MeetingInquiryAction.PairedCancellationFound;
			case ClientIntentMeetingInquiryActionType.FailedToRevive:
				return MeetingInquiryAction.FailedToRevive;
			}
			return MeetingInquiryAction.DeletedVersionNotFound;
		}

		private DateTime NormalizeDateToUtc(DateTime dateTime)
		{
			DateTime dateTime2 = (dateTime.Kind == DateTimeKind.Local) ? dateTime.ToUniversalTime() : dateTime;
			Globals.ConsistencyChecksTracer.TraceDebug<CalendarItemConverter, DateTime, DateTime>((long)this.GetHashCode(), "{0}: Normalizing DateTime value. Before: {1}, After: {2}", this, dateTime, dateTime2);
			return dateTime2;
		}

		private ExTimeZone ConvertTimeZone(TimeZoneDefinitionType remoteTimeZone)
		{
			ExTimeZone result = ExTimeZone.UtcTimeZone;
			if (remoteTimeZone != null)
			{
				try
				{
					Globals.ConsistencyChecksTracer.TraceDebug<CalendarItemConverter, string, string>((long)this.GetHashCode(), "{0}: Converting TimeZone - Id: {1}, Name: {2}", this, remoteTimeZone.Id, remoteTimeZone.Name);
					result = new TimeZoneDefinitionAdaptor(remoteTimeZone).ExTimeZone;
				}
				catch (TimeZoneException)
				{
				}
			}
			return result;
		}
	}
}
