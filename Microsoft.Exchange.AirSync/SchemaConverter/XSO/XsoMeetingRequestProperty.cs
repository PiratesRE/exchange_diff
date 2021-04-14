using System;
using System.Globalization;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoMeetingRequestProperty : XsoNestedProperty
	{
		public XsoMeetingRequestProperty(int protocolVersion) : base(new MeetingRequestData(protocolVersion), PropertyType.ReadOnly, XsoMeetingRequestProperty.prefetchProps)
		{
			this.protocolVersion = protocolVersion;
		}

		public override INestedData NestedData
		{
			get
			{
				if (base.NestedData.SubProperties.Count > 1)
				{
					AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.XsoTracer, this, "using already cached data. SubProperties Count {0}", base.NestedData.SubProperties.Count);
					return base.NestedData;
				}
				MeetingMessage meetingMessage = base.XsoItem as MeetingMessage;
				if (meetingMessage != null && !meetingMessage.IsDelegated())
				{
					MeetingRequestData meetingRequestData = base.NestedData as MeetingRequestData;
					if (meetingRequestData == null)
					{
						throw new UnexpectedTypeException("MeetingRequestData", base.NestedData);
					}
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.XsoTracer, this, "loading meeting request data.");
					bool flag = false;
					bool flag2 = false;
					object obj = meetingMessage.TryGetProperty(CalendarItemInstanceSchema.StartTime);
					if (obj != null && obj is ExDateTime)
					{
						flag = true;
						meetingRequestData.StartTime = ((this.protocolVersion >= 160) ? ((ExDateTime)obj) : ExTimeZone.UtcTimeZone.ConvertDateTime((ExDateTime)obj));
					}
					obj = meetingMessage.TryGetProperty(CalendarItemBaseSchema.OwnerCriticalChangeTime);
					if (obj != null && obj is ExDateTime)
					{
						meetingRequestData.DtStamp = ((this.protocolVersion >= 160) ? ((ExDateTime)obj) : ExTimeZone.UtcTimeZone.ConvertDateTime((ExDateTime)obj));
					}
					else
					{
						ExDateTime exDateTime = meetingMessage.Session.ExTimeZone.ConvertDateTime(meetingMessage.GetValueOrDefault<ExDateTime>(StoreObjectSchema.LastModifiedTime));
						meetingRequestData.DtStamp = ((this.protocolVersion >= 160) ? exDateTime : ExTimeZone.UtcTimeZone.ConvertDateTime(exDateTime));
					}
					obj = meetingMessage.TryGetProperty(CalendarItemInstanceSchema.EndTime);
					if (obj != null && obj is ExDateTime)
					{
						flag2 = true;
						meetingRequestData.EndTime = ((this.protocolVersion >= 160) ? ((ExDateTime)obj) : ExTimeZone.UtcTimeZone.ConvertDateTime((ExDateTime)obj));
					}
					obj = meetingMessage.TryGetProperty(CalendarItemBaseSchema.MapiIsAllDayEvent);
					if (obj != null && obj is bool)
					{
						if (flag && flag2 && (meetingRequestData.EndTime.Hour != meetingRequestData.StartTime.Hour || meetingRequestData.EndTime.Minute != meetingRequestData.StartTime.Minute))
						{
							meetingRequestData.AllDayEvent = false;
						}
						else
						{
							meetingRequestData.AllDayEvent = (bool)obj;
						}
					}
					if (this.protocolVersion < 160)
					{
						obj = meetingMessage.TryGetProperty(CalendarItemBaseSchema.Location);
						if (obj != null && obj is string)
						{
							meetingRequestData.Location = (string)obj;
						}
					}
					else
					{
						EnhancedLocationData enhancedLocationData = new EnhancedLocationData(this.protocolVersion);
						obj = meetingMessage.TryGetProperty(MeetingMessageSchema.LocationDisplayName);
						if (obj != null && obj is string && !string.IsNullOrEmpty((string)obj))
						{
							enhancedLocationData.DisplayName = (string)obj;
						}
						else
						{
							obj = meetingMessage.TryGetProperty(CalendarItemBaseSchema.Location);
							if (obj != null && obj is string && !string.IsNullOrEmpty((string)obj))
							{
								enhancedLocationData.DisplayName = (string)obj;
							}
						}
						obj = meetingMessage.TryGetProperty(MeetingMessageSchema.LocationAnnotation);
						if (obj != null && obj is string && !string.IsNullOrEmpty((string)obj))
						{
							enhancedLocationData.Annotation = (string)obj;
						}
						obj = meetingMessage.TryGetProperty(MeetingMessageSchema.LocationStreet);
						if (obj != null && obj is string && !string.IsNullOrEmpty((string)obj))
						{
							enhancedLocationData.Street = (string)obj;
						}
						obj = meetingMessage.TryGetProperty(MeetingMessageSchema.LocationCity);
						if (obj != null && obj is string && !string.IsNullOrEmpty((string)obj))
						{
							enhancedLocationData.City = (string)obj;
						}
						obj = meetingMessage.TryGetProperty(MeetingMessageSchema.LocationState);
						if (obj != null && obj is string && !string.IsNullOrEmpty((string)obj))
						{
							enhancedLocationData.State = (string)obj;
						}
						obj = meetingMessage.TryGetProperty(MeetingMessageSchema.LocationCountry);
						if (obj != null && obj is string && !string.IsNullOrEmpty((string)obj))
						{
							enhancedLocationData.Country = (string)obj;
						}
						obj = meetingMessage.TryGetProperty(MeetingMessageSchema.LocationPostalCode);
						if (obj != null && obj is string && !string.IsNullOrEmpty((string)obj))
						{
							enhancedLocationData.PostalCode = (string)obj;
						}
						obj = meetingMessage.TryGetProperty(MeetingMessageSchema.Latitude);
						if (obj != null && obj is double && !double.IsNaN((double)obj))
						{
							enhancedLocationData.Latitude = ((double)obj).ToString();
						}
						obj = meetingMessage.TryGetProperty(MeetingMessageSchema.Longitude);
						if (obj != null && obj is double && !double.IsNaN((double)obj))
						{
							enhancedLocationData.Longitude = ((double)obj).ToString();
						}
						obj = meetingMessage.TryGetProperty(MeetingMessageSchema.Accuracy);
						if (obj != null && obj is double && !double.IsNaN((double)obj))
						{
							enhancedLocationData.Accuracy = ((double)obj).ToString();
						}
						obj = meetingMessage.TryGetProperty(MeetingMessageSchema.Altitude);
						if (obj != null && obj is double && !double.IsNaN((double)obj))
						{
							enhancedLocationData.Altitude = ((double)obj).ToString();
						}
						obj = meetingMessage.TryGetProperty(MeetingMessageSchema.AltitudeAccuracy);
						if (obj != null && obj is double && !double.IsNaN((double)obj))
						{
							enhancedLocationData.AltitudeAccuracy = ((double)obj).ToString();
						}
						obj = meetingMessage.TryGetProperty(MeetingMessageSchema.LocationUri);
						if (obj != null && obj is string && !string.IsNullOrEmpty((string)obj))
						{
							enhancedLocationData.LocationUri = (string)obj;
						}
						meetingRequestData.EnhancedLocation = enhancedLocationData;
					}
					obj = meetingMessage.TryGetProperty(ItemSchema.ReminderIsSet);
					if (obj is bool && (bool)obj)
					{
						int num = 60 * meetingMessage.GetValueOrDefault<int>(ItemSchema.ReminderMinutesBeforeStart, 15);
						if (num > 1209600)
						{
							num = 900;
						}
						meetingRequestData.Reminder = num;
					}
					obj = meetingMessage.TryGetProperty(ItemSchema.IsResponseRequested);
					if (!(obj is PropertyError))
					{
						meetingRequestData.ResponseRequested = (bool)obj;
					}
					obj = meetingMessage.TryGetProperty(CalendarItemBaseSchema.DisallowNewTimeProposal);
					if (!(obj is PropertyError))
					{
						meetingRequestData.DisallowNewTimeProposal = (bool)obj;
					}
					meetingRequestData.Sensitivity = (int)meetingMessage.GetValueOrDefault<Sensitivity>(ItemSchema.Sensitivity);
					obj = meetingMessage.TryGetProperty(MeetingRequestSchema.IntendedFreeBusyStatus);
					if (!(obj is PropertyError))
					{
						meetingRequestData.BusyStatus = (int)obj;
					}
					if (this.protocolVersion < 160)
					{
						ExTimeZone exTimeZone = TimeZoneHelper.GetPromotedTimeZoneFromItem(meetingMessage);
						if (exTimeZone == null)
						{
							exTimeZone = ExTimeZone.CurrentTimeZone;
						}
						meetingRequestData.TimeZoneSubProperty = Convert.ToBase64String(TimeZoneConverter.GetBytes(exTimeZone, meetingRequestData.StartTime));
					}
					if (this.protocolVersion >= 160)
					{
						meetingRequestData.GlobalObjId = EntitySyncItemId.GetEntityID(meetingMessage.Id);
					}
					else
					{
						obj = meetingMessage.TryGetProperty(CalendarItemBaseSchema.GlobalObjectId);
						if (!(obj is PropertyError))
						{
							meetingRequestData.GlobalObjId = Convert.ToBase64String((byte[])obj);
						}
					}
					obj = meetingMessage.TryGetProperty(ItemSchema.Categories);
					if (!(obj is PropertyError))
					{
						meetingRequestData.Categories = (string[])obj;
					}
					MeetingMessageType meetingMessageType = MeetingMessageType.None;
					obj = meetingMessage.TryGetProperty(CalendarItemBaseSchema.MeetingRequestType);
					if (obj != null && obj is int)
					{
						meetingMessageType = (MeetingMessageType)obj;
					}
					if (!EnumValidator.IsValidValue<MeetingMessageType>(meetingMessageType))
					{
						throw new ConversionException("MeetingMessageType value is invalid.");
					}
					MeetingMessageType meetingMessageType2 = meetingMessageType;
					if (meetingMessageType2 <= MeetingMessageType.InformationalUpdate)
					{
						if (meetingMessageType2 == MeetingMessageType.NewMeetingRequest)
						{
							meetingRequestData.MeetingMessageType = AirSyncMeetingMessageType.NewMeetingRequest;
							goto IL_743;
						}
						if (meetingMessageType2 == MeetingMessageType.FullUpdate)
						{
							meetingRequestData.MeetingMessageType = AirSyncMeetingMessageType.FullUpdate;
							goto IL_743;
						}
						if (meetingMessageType2 == MeetingMessageType.InformationalUpdate)
						{
							meetingRequestData.MeetingMessageType = AirSyncMeetingMessageType.InformationalUpdate;
							goto IL_743;
						}
					}
					else
					{
						if (meetingMessageType2 == MeetingMessageType.SilentUpdate)
						{
							meetingRequestData.MeetingMessageType = AirSyncMeetingMessageType.Unspecified;
							goto IL_743;
						}
						if (meetingMessageType2 == MeetingMessageType.Outdated)
						{
							meetingRequestData.MeetingMessageType = AirSyncMeetingMessageType.Outdated;
							goto IL_743;
						}
						if (meetingMessageType2 == MeetingMessageType.PrincipalWantsCopy)
						{
							meetingRequestData.MeetingMessageType = AirSyncMeetingMessageType.PrincipalWantsCopy;
							goto IL_743;
						}
					}
					meetingRequestData.MeetingMessageType = AirSyncMeetingMessageType.Unspecified;
					try
					{
						IL_743:
						using (CalendarItemBase correlatedItem = meetingMessage.GetCorrelatedItem())
						{
							if (correlatedItem != null)
							{
								meetingRequestData.Organizer = this.RetrieveOrganizerInfo(correlatedItem);
								meetingRequestData.InstanceType = (int)correlatedItem.CalendarItemType;
								if (correlatedItem.CalendarItemType == CalendarItemType.Exception || correlatedItem.CalendarItemType == CalendarItemType.Occurrence)
								{
									meetingRequestData.RecurrenceId = ((CalendarItemOccurrence)correlatedItem).OriginalStartTime;
								}
							}
							else
							{
								AirSyncDiagnostics.TraceDebug(ExTraceGlobals.XsoTracer, this, "Correlated item not found, get the instance type and recurrence id from the meeting request");
								meetingRequestData.Organizer = this.RetrieveOrganizerInfo(meetingMessage);
								bool valueOrDefault = meetingMessage.GetValueOrDefault<bool>(CalendarItemBaseSchema.IsRecurring);
								bool valueOrDefault2 = meetingMessage.GetValueOrDefault<bool>(CalendarItemBaseSchema.IsException);
								bool flag3 = InternalRecurrence.HasRecurrenceBlob(meetingMessage.PropertyBag);
								CalendarItemType calendarItemType;
								if (valueOrDefault)
								{
									if (valueOrDefault2)
									{
										calendarItemType = CalendarItemType.Exception;
									}
									else if (flag3)
									{
										calendarItemType = CalendarItemType.RecurringMaster;
									}
									else
									{
										calendarItemType = CalendarItemType.Occurrence;
									}
								}
								else
								{
									calendarItemType = CalendarItemType.Single;
								}
								meetingRequestData.InstanceType = (int)calendarItemType;
								if (calendarItemType == CalendarItemType.Exception || calendarItemType == CalendarItemType.Occurrence)
								{
									meetingRequestData.RecurrenceId = meetingMessage.GetValueOrDefault<ExDateTime>(MeetingMessageInstanceSchema.MapiStartTime);
								}
							}
						}
					}
					catch (CorrelationFailedException ex)
					{
						AirSyncDiagnostics.TraceError<string, string>(ExTraceGlobals.CommonTracer, this, "CorrelationFailedException has been thrown.\n{0}\n{1}", ex.Message, ex.StackTrace);
					}
					catch (RecurrenceFormatException ex2)
					{
						if (!string.Equals(meetingMessage.ClassName, "IPM.Schedule.Meeting.Request", StringComparison.OrdinalIgnoreCase))
						{
							throw;
						}
						AirSyncDiagnostics.TraceError<string, string, string>(ExTraceGlobals.CommonTracer, this, "Recurrenceblob is corrupt, class={0}\n{1}\n{2}", meetingMessage.ClassName, ex2.Message, ex2.StackTrace);
					}
					CalendarItemType valueOrDefault3 = meetingMessage.GetValueOrDefault<CalendarItemType>(CalendarItemBaseSchema.CalendarItemType);
					switch (valueOrDefault3)
					{
					case CalendarItemType.Single:
						goto IL_97D;
					case CalendarItemType.RecurringMaster:
						try
						{
							using (CalendarItemBase embeddedItem = meetingMessage.GetEmbeddedItem())
							{
								RecurrenceData recurrenceData = new RecurrenceData(TypeOfRecurrence.Calendar, this.protocolVersion);
								try
								{
									XsoRecurrenceProperty.Populate(recurrenceData, embeddedItem);
								}
								catch (RecurrenceFormatException arg)
								{
									if (!string.Equals(meetingMessage.ClassName, "IPM.Schedule.Meeting.Request", StringComparison.OrdinalIgnoreCase))
									{
										throw;
									}
									AirSyncDiagnostics.TraceError<StoreObject, string, RecurrenceFormatException>(ExTraceGlobals.ConversionTracer, this, "The recurrence blob is corrupt for the item {0} of type {1} Exception = {2}", base.XsoItem, embeddedItem.ClassName, arg);
								}
								meetingRequestData.Recurrences = recurrenceData;
							}
							goto IL_97D;
						}
						catch (ObjectNotFoundException arg2)
						{
							AirSyncDiagnostics.TraceError<StoreObject, string, ObjectNotFoundException>(ExTraceGlobals.ConversionTracer, this, "The recurrence blob is not found for the item {0} of type {1} Exception = {2}", base.XsoItem, meetingMessage.ClassName, arg2);
							goto IL_97D;
						}
						break;
					}
					throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "Unsupported CalendarItemType {0}", new object[]
					{
						valueOrDefault3
					}));
				}
				else
				{
					base.State = PropertyState.SetToDefault;
				}
				IL_97D:
				return base.NestedData;
			}
		}

		private string RetrieveOrganizerInfo(Item item)
		{
			if (item == null)
			{
				AirSyncDiagnostics.TraceError(ExTraceGlobals.XsoTracer, this, "Organizer could not be set. No item found.");
				return null;
			}
			string text = null;
			string text2 = null;
			string text3 = null;
			try
			{
				text = item.GetValueOrDefault<string>(CalendarItemBaseSchema.OrganizerEmailAddress);
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.XsoTracer, this, "OrganizerEmailAddress: {0}", text);
				text2 = item.GetValueOrDefault<string>(CalendarItemBaseSchema.OrganizerType);
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.XsoTracer, this, "OrganizerType: {0}", text2);
				text3 = item.GetValueOrDefault<string>(CalendarItemBaseSchema.OrganizerDisplayName);
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.XsoTracer, this, "OrganizerDisplayName: {0}", text3);
			}
			catch (PropertyErrorException ex)
			{
				AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.XsoTracer, this, "PropertyErrorException retrieving Organizer info: {0}", ex.Message);
			}
			if (string.IsNullOrEmpty(text))
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.XsoTracer, this, "Organizer could not be set. OrganizerEmailAddress could not be retrieved");
				return null;
			}
			if (SmtpAddress.IsValidSmtpAddress(text) && !string.Equals(text2, "SMTP".ToString(), StringComparison.InvariantCultureIgnoreCase))
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.XsoTracer, this, "Meeting Request Organizer Type is invalid: OrganizerType:{0}. Defaulting to Smtp  RoutingType", text2);
				text2 = "SMTP".ToString();
			}
			Participant participant = new Participant(text3, text, text2);
			string participantString = EmailAddressConverter.GetParticipantString(participant, item.Session.MailboxOwner);
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.XsoTracer, this, "Meeting Request Organizer: {0}", participantString);
			return participantString;
		}

		private static PropertyDefinition[] prefetchProps = new PropertyDefinition[]
		{
			ItemSchema.ReminderIsSet,
			ItemSchema.ReminderMinutesBeforeStart,
			ItemSchema.IsResponseRequested,
			MeetingRequestSchema.IntendedFreeBusyStatus,
			CalendarItemBaseSchema.GlobalObjectId,
			ItemSchema.Categories,
			CalendarItemBaseSchema.DisallowNewTimeProposal
		};

		private int protocolVersion;
	}
}
