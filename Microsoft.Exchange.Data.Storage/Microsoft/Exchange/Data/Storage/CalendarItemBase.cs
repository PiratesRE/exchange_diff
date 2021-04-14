using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class CalendarItemBase : Item, ICalendarItemBase, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal static int ConvertTimeToOwnerId(ExDateTime time)
		{
			int num = 0;
			long num2 = time.UtcTicks;
			if (num2 > 300000000L)
			{
				num2 -= 300000000L;
			}
			else
			{
				num2 = 0L;
			}
			num |= (time.Year & 4095) << 20;
			num |= (time.Month & 15) << 16;
			num |= (time.Day & 31) << 11;
			return num | (int)(num2 & 2047L);
		}

		internal static void CoreObjectUpdateLocationAddress(CoreItem coreItem)
		{
			PersistablePropertyBag persistablePropertyBag = Microsoft.Exchange.Data.Storage.CoreObject.GetPersistablePropertyBag(coreItem);
			InternalSchema.LocationAddress.UpdateCompositePropertyValue(persistablePropertyBag);
		}

		internal CalendarItemBase(ICoreItem coreItem) : base(coreItem, false)
		{
			this.CreateCacheForChangeHighlight();
		}

		internal virtual void Initialize(bool newItem)
		{
			this.CheckDisposed("Initialize");
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(46741U, LastChangeAction.Create);
			this.AppointmentSequenceNumber = 0;
			this[InternalSchema.MeetingRequestWasSent] = false;
			this[InternalSchema.IsAllDayEvent] = false;
			this[InternalSchema.FreeBusyStatus] = BusyType.Busy;
			this[InternalSchema.IntendedFreeBusyStatus] = BusyType.Unknown;
			this[InternalSchema.AppointmentState] = 0;
			if (newItem)
			{
				MailboxSession mailboxSession = base.Session as MailboxSession;
				PublicFolderSession publicFolderSession = base.Session as PublicFolderSession;
				if (mailboxSession != null)
				{
					if (mailboxSession.MailboxOwner.MailboxInfo.IsAggregated)
					{
						this.Organizer = CalendarItemBase.GetAggregatedOwner(mailboxSession);
					}
					else
					{
						this.Organizer = new Participant(mailboxSession.MailboxOwner);
					}
					string value = null;
					if (CalendarOriginatorIdProperty.TryCreate(mailboxSession, out value))
					{
						this[InternalSchema.CalendarOriginatorId] = value;
					}
				}
				else if (publicFolderSession != null && publicFolderSession.LogonType == LogonType.Delegated)
				{
					this.Organizer = publicFolderSession.ConnectAsParticipant;
				}
				ExDateTime now = ExDateTime.GetNow(base.PropertyBag.ExTimeZone);
				this.OwnerAppointmentId = new int?(CalendarItemBase.ConvertTimeToOwnerId(now));
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(35477U);
				this[InternalSchema.Flags] = MessageFlags.IsRead;
				base.Reminder.MinutesBeforeStart = 15;
				base.Reminder.IsSet = true;
				this.ResponseType = ResponseType.Organizer;
				GlobalObjectId globalObjectId = new GlobalObjectId();
				this[InternalSchema.GlobalObjectId] = globalObjectId.Bytes;
				this.CleanGlobalObjectId = globalObjectId.Bytes;
				return;
			}
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(33429U);
			this[InternalSchema.IsDraft] = false;
		}

		internal static Participant GetAggregatedOwner(MailboxSession mailboxSession)
		{
			using (UserConfigurationDictionaryAdapter<AggregatedAccountConfiguration> userConfigurationDictionaryAdapter = new UserConfigurationDictionaryAdapter<AggregatedAccountConfiguration>(mailboxSession, "AggregatedAccount", new GetUserConfigurationDelegate(UserConfigurationHelper.GetMailboxConfiguration), new SimplePropertyDefinition[]
			{
				AggregatedAccountConfigurationSchema.EmailAddressRaw
			}))
			{
				AggregatedAccountConfiguration aggregatedAccountConfiguration = userConfigurationDictionaryAdapter.Read(mailboxSession.MailboxOwner);
				if (aggregatedAccountConfiguration.EmailAddress != null)
				{
					string text = aggregatedAccountConfiguration.EmailAddress.Value.ToString();
					return new Participant(text, text, "SMTP");
				}
			}
			return new Participant(mailboxSession.MailboxOwner);
		}

		public new static CalendarItemBase Bind(StoreSession session, StoreId storeId)
		{
			return CalendarItemBase.Bind(session, storeId, null);
		}

		public new static CalendarItemBase Bind(StoreSession session, StoreId storeId, params PropertyDefinition[] propsToReturn)
		{
			return CalendarItemBase.Bind(session, storeId, (ICollection<PropertyDefinition>)propsToReturn);
		}

		public new static CalendarItemBase Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<CalendarItemBase>(session, storeId, CalendarItemBaseSchema.Instance, propsToReturn);
		}

		public override object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				return base[propertyDefinition];
			}
			set
			{
				base[propertyDefinition] = value;
			}
		}

		public ExDateTime AppointmentReplyTime
		{
			get
			{
				this.CheckDisposed("AppointmentReplyTime::get");
				return base.GetValueOrDefault<ExDateTime>(CalendarItemBaseSchema.AppointmentReplyTime, ExDateTime.MinValue);
			}
			internal set
			{
				this.CheckDisposed("AppointmentReplyTime::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(54645U);
				this[CalendarItemBaseSchema.AppointmentReplyTime] = value;
			}
		}

		public string AppointmentReplyName
		{
			get
			{
				this.CheckDisposed("AppointmentReplyName::get");
				return base.GetValueOrDefault<string>(CalendarItemBaseSchema.AppointmentReplyName);
			}
			private set
			{
				this.CheckDisposed("AppointmentReplyName::set");
				this[CalendarItemBaseSchema.AppointmentReplyName] = value;
			}
		}

		public int? OwnerAppointmentId
		{
			get
			{
				this.CheckDisposed("OwnerAppointmentId::get");
				return base.GetValueAsNullable<int>(CalendarItemBaseSchema.OwnerAppointmentID);
			}
			private set
			{
				this.CheckDisposed("OwnerAppointmentId::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(60053U);
				this[CalendarItemBaseSchema.OwnerAppointmentID] = value;
			}
		}

		public byte[] CleanGlobalObjectId
		{
			get
			{
				this.CheckDisposed("CleanGlobalObjectId::get");
				return base.GetValueOrDefault<byte[]>(CalendarItemBaseSchema.CleanGlobalObjectId, null);
			}
			private set
			{
				this.CheckDisposed("CleanGlobalObjectId::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(58005U);
				this[CalendarItemBaseSchema.CleanGlobalObjectId] = value;
			}
		}

		public abstract int AppointmentLastSequenceNumber { get; set; }

		public int AppointmentSequenceNumber
		{
			get
			{
				this.CheckDisposed("AppointmentSequenceNumber::get");
				return base.GetValueOrDefault<int>(CalendarItemBaseSchema.AppointmentSequenceNumber);
			}
			protected set
			{
				this.CheckDisposed("AppointmentSequenceNumber::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(45429U);
				this[CalendarItemBaseSchema.AppointmentSequenceNumber] = value;
			}
		}

		public ExDateTime OwnerCriticalChangeTime
		{
			get
			{
				this.CheckDisposed("OwnerCriticalChangeTime::get");
				return base.GetValueOrDefault<ExDateTime>(CalendarItemBaseSchema.OwnerCriticalChangeTime, ExDateTime.MinValue);
			}
			protected set
			{
				this.CheckDisposed("OwnerCriticalChangeTime::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(35189U);
				this[CalendarItemBaseSchema.OwnerCriticalChangeTime] = value;
			}
		}

		public ExDateTime AttendeeCriticalChangeTime
		{
			get
			{
				this.CheckDisposed("AttendeeCriticalChangeTime::get");
				return base.GetValueOrDefault<ExDateTime>(CalendarItemBaseSchema.AttendeeCriticalChangeTime, ExDateTime.MinValue);
			}
		}

		public override string Subject
		{
			get
			{
				return base.Subject;
			}
			set
			{
				base.Subject = value;
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(49813U);
			}
		}

		public abstract ExDateTime StartTime { get; set; }

		public abstract ExDateTime StartWallClock { get; }

		public abstract ExDateTime EndWallClock { get; }

		public abstract ExDateTime EndTime { get; set; }

		public ExTimeZone StartTimeZone
		{
			get
			{
				this.CheckDisposed("StartTimeZone::get");
				base.Load(new PropertyDefinition[]
				{
					CalendarItemBaseSchema.StartTimeZone
				});
				return base.TryGetProperty(CalendarItemBaseSchema.StartTimeZone) as ExTimeZone;
			}
			set
			{
				this.CheckDisposed("StartTimeZone::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(40949U);
				this[CalendarItemBaseSchema.StartTimeZone] = value;
			}
		}

		public ExTimeZone EndTimeZone
		{
			get
			{
				this.CheckDisposed("EndTimeZone::get");
				base.Load(new PropertyDefinition[]
				{
					CalendarItemBaseSchema.EndTimeZone
				});
				return base.TryGetProperty(CalendarItemBaseSchema.EndTimeZone) as ExTimeZone;
			}
			set
			{
				this.CheckDisposed("EndTimeZone::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(45045U);
				this[CalendarItemBaseSchema.EndTimeZone] = value;
			}
		}

		public bool IsAllDayEvent
		{
			get
			{
				this.CheckDisposed("IsAllDayEvent::get");
				return base.GetValueOrDefault<bool>(InternalSchema.IsAllDayEvent);
			}
			set
			{
				this.CheckDisposed("IsAllDayEvent::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(40309U);
				this[InternalSchema.IsAllDayEvent] = value;
			}
		}

		public bool AllowNewTimeProposal
		{
			get
			{
				this.CheckDisposed("AllowNewTimeProposal::get");
				return !base.GetValueOrDefault<bool>(InternalSchema.DisallowNewTimeProposal);
			}
		}

		public bool IsEvent
		{
			get
			{
				this.CheckDisposed("IsEvent::get");
				return base.GetValueOrDefault<bool>(InternalSchema.IsEvent);
			}
		}

		public bool IsOrganizer()
		{
			this.CheckDisposed("IsOrganizer");
			return base.GetValueOrDefault<bool>(InternalSchema.IsOrganizer);
		}

		public CalendarItemType CalendarItemType
		{
			get
			{
				this.CheckDisposed("CalendarItemType::get");
				return Microsoft.Exchange.Data.Storage.PropertyBag.CheckPropertyValue<CalendarItemType>(InternalSchema.CalendarItemType, base.TryGetProperty(InternalSchema.CalendarItemType), CalendarItemType.Single);
			}
		}

		public bool IsCalendarItemTypeOccurrenceOrException
		{
			get
			{
				CalendarItemType calendarItemType = this.CalendarItemType;
				return calendarItemType == CalendarItemType.Occurrence || calendarItemType == CalendarItemType.Exception;
			}
		}

		public ResponseType ResponseType
		{
			get
			{
				this.CheckDisposed("ResponseType::get");
				return base.GetValueOrDefault<ResponseType>(CalendarItemBaseSchema.ResponseType, ResponseType.NotResponded);
			}
			set
			{
				this.CheckDisposed("ResponseType::set");
				EnumValidator.ThrowIfInvalid<ResponseType>(value, "value");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(56693U);
				this[CalendarItemBaseSchema.ResponseType] = value;
			}
		}

		public IAttendeeCollection AttendeeCollection
		{
			get
			{
				return this.FetchAttendeeCollection(true);
			}
		}

		public bool AttendeesChanged
		{
			get
			{
				this.CheckDisposed("AttendeesChanged::get");
				this.CalculateAttendeeDiff();
				return (this.addedAttendeeArray != null && this.addedAttendeeArray.Length > 0) || (this.removedAttendeeArray != null && this.removedAttendeeArray.Length > 0);
			}
		}

		public Reminders<EventTimeBasedInboxReminder> EventTimeBasedInboxReminders
		{
			get
			{
				this.CheckDisposed("EventTimeBasedInboxReminders::get");
				if (this.eventTimeBasedInboxReminders == null)
				{
					this.eventTimeBasedInboxReminders = this.FetchEventTimeBasedInboxReminders();
				}
				return this.eventTimeBasedInboxReminders;
			}
			set
			{
				this.CheckDisposed("EventTimeBasedInboxReminders::set");
				this.UpdateEventTimeBasedInboxRemindersForSave(value);
				Reminders<EventTimeBasedInboxReminder>.Set(this, CalendarItemBaseSchema.EventTimeBasedInboxReminders, value);
				this.eventTimeBasedInboxReminders = value;
			}
		}

		public BusyType FreeBusyStatus
		{
			get
			{
				this.CheckDisposed("FreeBusyStatus::get");
				return base.GetValueOrDefault<BusyType>(InternalSchema.FreeBusyStatus, BusyType.Busy);
			}
			set
			{
				this.CheckDisposed("FreeBusyStatus::set");
				EnumValidator.ThrowIfInvalid<BusyType>(value, "value");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(44405U);
				this[InternalSchema.FreeBusyStatus] = value;
			}
		}

		public string SeriesId
		{
			get
			{
				this.CheckDisposed("SeriesId::get");
				return base.GetValueOrDefault<string>(InternalSchema.SeriesId, string.Empty);
			}
			set
			{
				this.CheckDisposed("SeriesId::set");
				this[InternalSchema.SeriesId] = value;
			}
		}

		public string ClientId
		{
			get
			{
				this.CheckDisposed("ClientId::get");
				return base.GetValueOrDefault<string>(InternalSchema.EventClientId, string.Empty);
			}
			set
			{
				this.CheckDisposed("ClientId::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(63612U);
				this[InternalSchema.EventClientId] = value;
			}
		}

		public bool IsHiddenFromLegacyClients
		{
			get
			{
				this.CheckDisposed("IsHiddenFromLegacyClients::get");
				return base.GetValueOrDefault<bool>(CalendarItemBaseSchema.IsHiddenFromLegacyClients, false);
			}
			set
			{
				this.CheckDisposed("IsHiddenFromLegacyClients::set");
				this[CalendarItemBaseSchema.IsHiddenFromLegacyClients] = value;
			}
		}

		public string Location
		{
			get
			{
				this.CheckDisposed("Location::get");
				return base.GetValueOrDefault<string>(InternalSchema.Location, string.Empty);
			}
			set
			{
				this.CheckDisposed("Location::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(60789U);
				this[InternalSchema.Location] = value;
			}
		}

		public string LocationDisplayName
		{
			get
			{
				this.CheckDisposed("LocationDisplayName::get");
				return base.GetValueOrDefault<string>(InternalSchema.LocationDisplayName, null);
			}
			set
			{
				this.CheckDisposed("LocationDisplayName::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(57640U);
				base.SetOrDeleteProperty(InternalSchema.LocationDisplayName, value);
			}
		}

		public string LocationAnnotation
		{
			get
			{
				this.CheckDisposed("LocationAnnotation::get");
				return base.GetValueOrDefault<string>(InternalSchema.LocationAnnotation, null);
			}
			set
			{
				this.CheckDisposed("LocationAnnotation::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(40464U);
				base.SetOrDeleteProperty(InternalSchema.LocationAnnotation, value);
			}
		}

		public LocationSource LocationSource
		{
			get
			{
				this.CheckDisposed("LocationSource::get");
				return (LocationSource)base.GetValueOrDefault<int>(InternalSchema.LocationSource, 0);
			}
			set
			{
				this.CheckDisposed("LocationSource::set");
				EnumValidator.ThrowIfInvalid<LocationSource>(value, "LocationSource");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(33064U);
				this[InternalSchema.LocationSource] = value;
			}
		}

		public string LocationUri
		{
			get
			{
				this.CheckDisposed("LocationUri::get");
				return base.GetValueOrDefault<string>(InternalSchema.LocationUri, null);
			}
			set
			{
				this.CheckDisposed("LocationUri::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(49448U);
				base.SetOrDeleteProperty(InternalSchema.LocationUri, value);
			}
		}

		public double? Latitude
		{
			get
			{
				this.CheckDisposed("Latitude::get");
				return base.GetValueOrDefault<double?>(InternalSchema.Latitude, null);
			}
			set
			{
				this.CheckDisposed("Latitude::set");
				if (value.Equals(base.GetValueAsNullable<double>(InternalSchema.Latitude)))
				{
					return;
				}
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(48936U);
				base.SetOrDeleteProperty(InternalSchema.Latitude, value);
			}
		}

		public double? Longitude
		{
			get
			{
				this.CheckDisposed("Longitude::get");
				return base.GetValueOrDefault<double?>(InternalSchema.Longitude, null);
			}
			set
			{
				this.CheckDisposed("Longitude::set");
				if (value.Equals(base.GetValueAsNullable<double>(InternalSchema.Longitude)))
				{
					return;
				}
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(65320U);
				base.SetOrDeleteProperty(InternalSchema.Longitude, value);
			}
		}

		public double? Accuracy
		{
			get
			{
				this.CheckDisposed("Accuracy::get");
				return base.GetValueOrDefault<double?>(InternalSchema.Accuracy, null);
			}
			set
			{
				this.CheckDisposed("Accuracy::set");
				if (value.Equals(base.GetValueAsNullable<double>(InternalSchema.Accuracy)))
				{
					return;
				}
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(40744U);
				base.SetOrDeleteProperty(InternalSchema.Accuracy, value);
			}
		}

		public double? Altitude
		{
			get
			{
				this.CheckDisposed("Altitude::get");
				return base.GetValueOrDefault<double?>(InternalSchema.Altitude, null);
			}
			set
			{
				this.CheckDisposed("Altitude::set");
				if (value.Equals(base.GetValueAsNullable<double>(InternalSchema.Altitude)))
				{
					return;
				}
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(57128U);
				base.SetOrDeleteProperty(InternalSchema.Altitude, value);
			}
		}

		public double? AltitudeAccuracy
		{
			get
			{
				this.CheckDisposed("AltitudeAccuracy::get");
				return base.GetValueOrDefault<double?>(InternalSchema.AltitudeAccuracy, null);
			}
			set
			{
				this.CheckDisposed("AltitudeAccuracy::set");
				if (value.Equals(base.GetValueAsNullable<double>(InternalSchema.AltitudeAccuracy)))
				{
					return;
				}
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(44840U);
				base.SetOrDeleteProperty(InternalSchema.AltitudeAccuracy, value);
			}
		}

		public string LocationStreet
		{
			get
			{
				this.CheckDisposed("LocationStreet::get");
				return base.GetValueOrDefault<string>(CalendarItemBaseSchema.LocationStreet, null);
			}
			set
			{
				this.CheckDisposed("LocationStreet::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(61224U);
				base.SetOrDeleteProperty(CalendarItemBaseSchema.LocationStreet, value);
			}
		}

		public string LocationCity
		{
			get
			{
				this.CheckDisposed("LocationCity::get");
				return base.GetValueOrDefault<string>(CalendarItemBaseSchema.LocationCity, null);
			}
			set
			{
				this.CheckDisposed("LocationCity::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(36928U);
				base.SetOrDeleteProperty(CalendarItemBaseSchema.LocationCity, value);
			}
		}

		public string LocationState
		{
			get
			{
				this.CheckDisposed("LocationState::get");
				return base.GetValueOrDefault<string>(CalendarItemBaseSchema.LocationState, null);
			}
			set
			{
				this.CheckDisposed("LocationState::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(53312U);
				base.SetOrDeleteProperty(CalendarItemBaseSchema.LocationState, value);
			}
		}

		public string LocationCountry
		{
			get
			{
				this.CheckDisposed("LocationCountry::get");
				return base.GetValueOrDefault<string>(CalendarItemBaseSchema.LocationCountry, null);
			}
			set
			{
				this.CheckDisposed("LocationCountry::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(41024U);
				base.SetOrDeleteProperty(CalendarItemBaseSchema.LocationCountry, value);
			}
		}

		public string LocationPostalCode
		{
			get
			{
				this.CheckDisposed("LocationPostalCode::get");
				return base.GetValueOrDefault<string>(CalendarItemBaseSchema.LocationPostalCode, null);
			}
			set
			{
				this.CheckDisposed("LocationPostalCode::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(57408U);
				base.SetOrDeleteProperty(CalendarItemBaseSchema.LocationPostalCode, value);
			}
		}

		public string OnlineMeetingConfLink
		{
			get
			{
				this.CheckDisposed("OnlineMeetingConfLink::get");
				return base.GetValueOrDefault<string>(InternalSchema.OnlineMeetingConfLink, null);
			}
			set
			{
				this.CheckDisposed("OnlineMeetingConfLink::set");
				base.SetOrDeleteProperty(InternalSchema.OnlineMeetingConfLink, value);
			}
		}

		public string OnlineMeetingExternalLink
		{
			get
			{
				this.CheckDisposed("OnlineMeetingExternalLink::get");
				return base.GetValueOrDefault<string>(InternalSchema.OnlineMeetingExternalLink, null);
			}
			set
			{
				this.CheckDisposed("OnlineMeetingExternalLink::set");
				base.SetOrDeleteProperty(InternalSchema.OnlineMeetingExternalLink, value);
			}
		}

		public string OnlineMeetingInternalLink
		{
			get
			{
				this.CheckDisposed("OnlineMeetingInternalLink::get");
				return base.GetValueOrDefault<string>(InternalSchema.OnlineMeetingInternalLink, null);
			}
			set
			{
				this.CheckDisposed("OnlineMeetingInternalLink::set");
				base.SetOrDeleteProperty(InternalSchema.OnlineMeetingInternalLink, value);
			}
		}

		public string UCOpenedConferenceID
		{
			get
			{
				this.CheckDisposed("UCOpenedConferenceID::get");
				return base.GetValueOrDefault<string>(InternalSchema.UCOpenedConferenceID, null);
			}
			set
			{
				this.CheckDisposed("UCOpenedConferenceID::set");
				base.SetOrDeleteProperty(InternalSchema.UCOpenedConferenceID, value);
			}
		}

		public string UCCapabilities
		{
			get
			{
				this.CheckDisposed("UCCapabilities::get");
				return base.GetValueOrDefault<string>(InternalSchema.UCCapabilities, null);
			}
			set
			{
				this.CheckDisposed("UCCapabilities::set");
				base.SetOrDeleteProperty(InternalSchema.UCCapabilities, value);
			}
		}

		public string UCInband
		{
			get
			{
				this.CheckDisposed("UCInband::get");
				return base.GetValueOrDefault<string>(InternalSchema.UCInband, null);
			}
			set
			{
				this.CheckDisposed("UCInband::set");
				base.SetOrDeleteProperty(InternalSchema.UCInband, value);
			}
		}

		public string UCMeetingSetting
		{
			get
			{
				this.CheckDisposed("UCMeetingSetting::get");
				return base.GetValueOrDefault<string>(InternalSchema.UCMeetingSetting, null);
			}
			set
			{
				this.CheckDisposed("UCMeetingSetting::set");
				base.SetOrDeleteProperty(InternalSchema.UCMeetingSetting, value);
			}
		}

		public string UCMeetingSettingSent
		{
			get
			{
				this.CheckDisposed("UCMeetingSettingSent::get");
				return base.GetValueOrDefault<string>(InternalSchema.UCMeetingSettingSent, null);
			}
			set
			{
				this.CheckDisposed("UCMeetingSettingSent::set");
				base.SetOrDeleteProperty(InternalSchema.UCMeetingSettingSent, value);
			}
		}

		public string ConferenceTelURI
		{
			get
			{
				this.CheckDisposed("ConferenceTelURI::get");
				return base.GetValueOrDefault<string>(InternalSchema.ConferenceTelURI, null);
			}
			set
			{
				this.CheckDisposed("ConferenceTelURI::set");
				base.SetOrDeleteProperty(InternalSchema.ConferenceTelURI, value);
			}
		}

		public string ConferenceInfo
		{
			get
			{
				this.CheckDisposed("ConferenceInfo::get");
				return base.GetValueOrDefault<string>(InternalSchema.ConferenceInfo, null);
			}
			set
			{
				this.CheckDisposed("ConferenceInfo::set");
				base.SetOrDeleteProperty(InternalSchema.ConferenceInfo, value);
			}
		}

		public byte[] OutlookUserPropsPropDefStream
		{
			get
			{
				this.CheckDisposed("OutlookUserPropsPropDefStream::get");
				return base.GetValueOrDefault<byte[]>(InternalSchema.OutlookUserPropsPropDefStream, null);
			}
			set
			{
				this.CheckDisposed("OutlookUserPropsPropDefStream::set");
				base.SetOrDeleteProperty(InternalSchema.OutlookUserPropsPropDefStream, value);
			}
		}

		public string When
		{
			get
			{
				this.CheckDisposed("When::get");
				return base.GetValueOrDefault<string>(InternalSchema.When, string.Empty);
			}
			set
			{
				this.CheckDisposed("When::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(36213U);
				this[InternalSchema.When] = value;
			}
		}

		public bool IsMeeting
		{
			get
			{
				this.CheckDisposed("IsMeeting::get");
				return base.GetValueOrDefault<bool>(InternalSchema.IsMeeting);
			}
			set
			{
				this.CheckDisposed("IsMeeting::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(52597U);
				this[InternalSchema.IsMeeting] = value;
			}
		}

		public bool IsCancelled
		{
			get
			{
				this.CheckDisposed("IsMeetingCancelled::get");
				AppointmentStateFlags valueOrDefault = base.GetValueOrDefault<AppointmentStateFlags>(CalendarItemBaseSchema.AppointmentState);
				return CalendarItemBase.IsAppointmentStateCancelled(valueOrDefault);
			}
		}

		public bool MeetingRequestWasSent
		{
			get
			{
				this.CheckDisposed("MeetingRequestWasSent::get");
				return base.GetValueOrDefault<bool>(InternalSchema.MeetingRequestWasSent);
			}
		}

		public Participant Organizer
		{
			get
			{
				this.CheckDisposed("Organizer::get");
				return base.GetValueOrDefault<Participant>(InternalSchema.From);
			}
			private set
			{
				base.SetOrDeleteProperty(InternalSchema.From, value);
			}
		}

		public bool IsOrganizerExternal
		{
			get
			{
				this.CheckDisposed("IsOrganizerExternal::get");
				Participant organizer = this.Organizer;
				return organizer == null || MeetingMessage.IsFromExternalParticipant(organizer.RoutingType);
			}
		}

		public string CalendarOriginatorId
		{
			get
			{
				this.CheckDisposed("CalendarOriginatorId::get");
				return base.GetValueOrDefault<string>(CalendarItemBaseSchema.CalendarOriginatorId);
			}
		}

		public virtual bool IsForwardAllowed
		{
			get
			{
				this.CheckDisposed("IsForwardAllowed::get");
				return true;
			}
		}

		public ClientIntentFlags ClientIntent
		{
			get
			{
				this.CheckDisposed("ClientIntent::get");
				return base.GetValueOrDefault<ClientIntentFlags>(CalendarItemBaseSchema.ClientIntent, ClientIntentFlags.None);
			}
			set
			{
				this.CheckDisposed("ClientIntent::set");
				EnumValidator.ThrowIfInvalid<ClientIntentFlags>(value, "ClientIntent");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(59551U);
				this[CalendarItemBaseSchema.ClientIntent] = value;
			}
		}

		public bool ResponseRequested
		{
			get
			{
				this.CheckDisposed("ResponseRequested::get");
				return base.GetValueOrDefault<bool>(InternalSchema.IsResponseRequested);
			}
			set
			{
				this.CheckDisposed("ResponseRequested::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(46620U);
				this[InternalSchema.IsResponseRequested] = value;
			}
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return CalendarItemBaseSchema.Instance;
			}
		}

		public bool IsReminderSet
		{
			get
			{
				return base.Reminder.IsSet;
			}
			set
			{
				base.Reminder.IsSet = value;
			}
		}

		public int ReminderMinutesBeforeStart
		{
			get
			{
				return base.Reminder.MinutesBeforeStart;
			}
			set
			{
				base.Reminder.MinutesBeforeStart = value;
			}
		}

		public string ItemClass
		{
			get
			{
				return base.TryGetProperty(InternalSchema.ItemClass) as string;
			}
		}

		public RemindersState<EventTimeBasedInboxReminderState> EventTimeBasedInboxRemindersState
		{
			get
			{
				this.CheckDisposed("EventTimeBasedInboxRemindersState::get");
				if (this.eventTimeBasedInboxRemindersState == null)
				{
					this.eventTimeBasedInboxRemindersState = RemindersState<EventTimeBasedInboxReminderState>.Get(this, CalendarItemBaseSchema.EventTimeBasedInboxRemindersState);
				}
				return this.eventTimeBasedInboxRemindersState;
			}
			set
			{
				this.CheckDisposed("EventTimeBasedInboxRemindersState::set");
				RemindersState<EventTimeBasedInboxReminderState>.Set(this, CalendarItemBaseSchema.EventTimeBasedInboxRemindersState, value);
				this.eventTimeBasedInboxRemindersState = value;
			}
		}

		public static bool IsAppointmentStateCancelled(AppointmentStateFlags appointmentState)
		{
			appointmentState &= AppointmentStateFlags.Cancelled;
			return appointmentState == AppointmentStateFlags.Cancelled;
		}

		public abstract string GenerateWhen();

		public MeetingResponse RespondToMeetingRequest(ResponseType responseType)
		{
			return this.RespondToMeetingRequest(responseType, false, false, null, null);
		}

		public MeetingResponse RespondToMeetingRequest(ResponseType responseType, bool autoCaptureClientIntent, bool intendToSendResponse, ExDateTime? proposedStart = null, ExDateTime? proposedEnd = null)
		{
			this.CheckDisposed("RespondToMeetingRequest");
			EnumValidator.ThrowIfInvalid<ResponseType>(responseType, "responseType");
			if (autoCaptureClientIntent)
			{
				this.SetClientIntentBasedOnResponse(responseType, intendToSendResponse);
			}
			return this.RespondToMeetingRequest(responseType, null, proposedStart, proposedEnd);
		}

		public virtual MeetingResponse RespondToMeetingRequest(ResponseType responseType, string subjectPrefix, ExDateTime? proposedStart = null, ExDateTime? proposedEnd = null)
		{
			this.CheckDisposed("RespondToMeetingRequest");
			EnumValidator.ThrowIfInvalid<ResponseType>(responseType, "responseType");
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, ResponseType>((long)this.GetHashCode(), "Storage.CalendarItemBase.RespondToMeetingRequest: GOID={0}; responseType={1}", this.GlobalObjectId, responseType);
			if (this.IsCancelled)
			{
				throw new InvalidOperationException("RespondToMeetingRequest called on cancelled meeting");
			}
			if (this.IsOrganizer())
			{
				throw new InvalidOperationException(ServerStrings.ExCannotCreateMeetingResponse);
			}
			if (this.Organizer == null)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidOrganizer);
			}
			MailboxSession mailboxSession = base.Session as MailboxSession;
			bool flag = false;
			if (mailboxSession != null)
			{
				if (mailboxSession.MasterMailboxSession != null)
				{
					base.LocationIdentifierHelperInstance.SetLocationIdentifier(46453U);
					this.AppointmentReplyName = mailboxSession.MasterMailboxSession.MailboxOwner.MailboxInfo.DisplayName;
				}
				else if (mailboxSession.LogonType == LogonType.Delegated)
				{
					base.LocationIdentifierHelperInstance.SetLocationIdentifier(62837U);
					this.AppointmentReplyName = mailboxSession.DelegateUser.DisplayName;
				}
				else
				{
					base.LocationIdentifierHelperInstance.SetLocationIdentifier(38261U);
					this.AppointmentReplyName = mailboxSession.MailboxOwner.MailboxInfo.DisplayName;
				}
				mailboxSession = this.GetMessageMailboxSession(out flag);
			}
			bool flag2 = this.IsCorrelated;
			this.ResponseType = responseType;
			BusyType valueOrDefault = base.GetValueOrDefault<BusyType>(InternalSchema.IntendedFreeBusyStatus, BusyType.Busy);
			if (ResponseType.Tentative == responseType)
			{
				this.FreeBusyStatus = ((valueOrDefault == BusyType.Free) ? BusyType.Free : BusyType.Tentative);
			}
			else if (ResponseType.Accept == responseType)
			{
				this.FreeBusyStatus = ((valueOrDefault == BusyType.Unknown) ? BusyType.Busy : valueOrDefault);
			}
			else if (ResponseType.Decline == responseType)
			{
				this.FreeBusyStatus = BusyType.Free;
				if (flag)
				{
					base.LocationIdentifierHelperInstance.SetLocationIdentifier(42357U);
					this[InternalSchema.OriginalStoreEntryId] = base.Session.Mailbox.StoreObjectId.ProviderLevelItemId;
				}
			}
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(58741U, LastChangeAction.RespondToMeetingRequest);
			MeetingResponse meetingResponse = null;
			bool flag3 = false;
			byte[] bytes = CTSGlobals.AsciiEncoding.GetBytes(this[InternalSchema.AppointmentReplyName].ToString());
			MeetingResponse result;
			try
			{
				meetingResponse = this.CreateResponse(mailboxSession, responseType, flag, proposedStart, proposedEnd);
				this.AppointmentReplyTime = meetingResponse.AttendeeCriticalChangeTime;
				this.SaveWithConflictCheck(SaveMode.ResolveConflicts);
				if (proposedStart != null && proposedEnd != null && string.IsNullOrEmpty(subjectPrefix))
				{
					subjectPrefix = ClientStrings.MeetingProposedNewTime.ToString(base.Session.InternalCulture);
				}
				CalendarItemBase.SetMeetingResponseSubjectPrefix(responseType, meetingResponse, subjectPrefix);
				flag3 = true;
				if (responseType == ResponseType.Decline)
				{
					this.UpdateAppointmentTombstone(bytes);
				}
				result = meetingResponse;
			}
			finally
			{
				if (!flag3 && meetingResponse != null)
				{
					meetingResponse.Dispose();
					meetingResponse = null;
				}
			}
			return result;
		}

		public abstract void MoveToFolder(MailboxSession destinationSession, StoreObjectId destinationFolderId);

		public abstract void CopyToFolder(MailboxSession destinationSession, StoreObjectId destinationFolderId);

		private static void SetMeetingResponseSubjectPrefix(ResponseType responseType, MeetingMessage meetingMessage, string overrideSubjectPrefix)
		{
			if (overrideSubjectPrefix != null)
			{
				meetingMessage[InternalSchema.SubjectPrefix] = overrideSubjectPrefix;
				return;
			}
			LocalizedString localizedString = LocalizedString.Empty;
			switch (responseType)
			{
			case ResponseType.Tentative:
				localizedString = ClientStrings.MeetingTentative;
				break;
			case ResponseType.Accept:
				localizedString = ClientStrings.MeetingAccept;
				break;
			case ResponseType.Decline:
				localizedString = ClientStrings.MeetingDecline;
				break;
			default:
				throw new NotSupportedException(ServerStrings.ExResponseTypeNoSubjectPrefix(responseType.ToString()));
			}
			meetingMessage[InternalSchema.SubjectPrefix] = localizedString.ToString(meetingMessage.Session.InternalPreferedCulture);
		}

		private void UpdateAppointmentTombstone(byte[] username)
		{
			if (username == null)
			{
				ExTraceGlobals.MeetingMessageTracer.TraceError((long)this.GetHashCode(), "TombstoneRecord with UserName null cannot be created. Hence AppointmentTombstone is not updated!");
				return;
			}
			MailboxSession mailboxSession = base.Session as MailboxSession;
			if (mailboxSession != null && mailboxSession.MailboxOwner != null && mailboxSession.MailboxOwner.Delegates != null && mailboxSession.MailboxOwner.Delegates.Any<ADObjectId>())
			{
				try
				{
					TombstoneRecord tombstoneRecord = new TombstoneRecord
					{
						StartTime = this.StartTimeZone.ConvertDateTime(this.StartTime),
						EndTime = this.EndTimeZone.ConvertDateTime(this.EndTime),
						GlobalObjectId = this.GlobalObjectId.Bytes,
						UserName = username
					};
					StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.FreeBusyData);
					VersionedId localFreeBusyMessageId = mailboxSession.GetLocalFreeBusyMessageId(defaultFolderId);
					if (localFreeBusyMessageId != null)
					{
						using (Item item = Microsoft.Exchange.Data.Storage.Item.Bind(mailboxSession, localFreeBusyMessageId, CalendarItemBase.AppointmentTombstomeDefinition))
						{
							ExTraceGlobals.MeetingMessageTracer.Information<string>((long)this.GetHashCode(), "Reading AppointmentTombstone property for the mailbox: {0}.", mailboxSession.MailboxOwner.MailboxInfo.DisplayName);
							byte[] valueOrDefault = item.GetValueOrDefault<byte[]>(InternalSchema.AppointmentTombstones);
							int valueOrDefault2 = item.GetValueOrDefault<int>(InternalSchema.OutlookFreeBusyMonthCount, 2);
							AppointmentTombstone appointmentTombstone = new AppointmentTombstone();
							try
							{
								appointmentTombstone.LoadTombstoneRecords(valueOrDefault, valueOrDefault2);
							}
							catch (CorruptDataException)
							{
								ExTraceGlobals.MeetingMessageTracer.TraceError<string>((long)this.GetHashCode(), "Appointment tombstone is corrupted for: {0}", mailboxSession.MailboxOwner.MailboxInfo.DisplayName);
							}
							ExTraceGlobals.MeetingMessageTracer.Information<string>((long)this.GetHashCode(), "Updating AppointmentTombstone property for the mailbox: {0}.", mailboxSession.MailboxOwner.MailboxInfo.DisplayName);
							appointmentTombstone.AppendTombstoneRecord(tombstoneRecord);
							item.SafeSetProperty(InternalSchema.AppointmentTombstones, appointmentTombstone.ToByteArray());
							item.Save(SaveMode.ResolveConflicts);
							goto IL_1D4;
						}
					}
					ExTraceGlobals.MeetingMessageTracer.TraceError<string>((long)this.GetHashCode(), "Outlook freebusy message was not found for the mailbox: {0}.", mailboxSession.MailboxOwner.MailboxInfo.DisplayName);
					IL_1D4:;
				}
				catch (StoragePermanentException ex)
				{
					ExTraceGlobals.MeetingMessageTracer.TraceError<string, string>((long)this.GetHashCode(), "Unable to update AppointmentTombstone for mailbox '{0}' with exception:'{1}'", mailboxSession.MailboxOwner.MailboxInfo.DisplayName, ex.Message);
				}
				catch (StorageTransientException ex2)
				{
					ExTraceGlobals.MeetingMessageTracer.TraceError<string, string>((long)this.GetHashCode(), "Unable to update AppointmentTombstone for mailbox '{0}' with exception:'{1}'", mailboxSession.MailboxOwner.MailboxInfo.DisplayName, ex2.Message);
				}
			}
		}

		public static StoreObjectId GetDraftsFolderIdOrThrow(MailboxSession mailboxSession)
		{
			if (mailboxSession == null)
			{
				throw new NotSupportedException();
			}
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts);
			if (defaultFolderId == null)
			{
				ExTraceGlobals.MeetingMessageTracer.TraceError(-1L, "CalendarItemBase::GetDraftsFolderIdOrThrow. Drafts folder cannot be found.");
				throw new ObjectNotFoundException(ServerStrings.ExDefaultFolderNotFound(DefaultFolderType.Drafts));
			}
			return defaultFolderId;
		}

		protected virtual void InitializeMeetingResponse(MeetingResponse meetingResponse, ResponseType responseType, bool isCalendarDelegateAccess, ExDateTime? proposedStart, ExDateTime? proposedEnd)
		{
			meetingResponse[InternalSchema.IconIndex] = CalendarItemBase.GetIconIndexToSet(responseType);
			if (isCalendarDelegateAccess)
			{
				meetingResponse.From = new Participant(((MailboxSession)base.Session).MailboxOwner);
			}
			CalendarItemBase.CopyPropertiesTo(this, meetingResponse, CalendarItemProperties.MeetingResponseProperties);
			if (this.AssociatedItemId != null)
			{
				meetingResponse.AssociatedItemId = this.AssociatedItemId;
			}
			meetingResponse[CalendarItemBaseSchema.AttendeeCriticalChangeTime] = ExDateTime.GetNow(base.PropertyBag.ExTimeZone);
			meetingResponse.Recipients.Add(this.Organizer);
		}

		private MeetingResponse CreateResponse(MailboxSession mailboxSession, ResponseType responseType, bool isCalendarDelegateAccess, ExDateTime? proposedStart = null, ExDateTime? proposedEnd = null)
		{
			bool flag = false;
			MeetingResponse meetingResponse = this.CreateNewMeetingResponse(mailboxSession, responseType);
			try
			{
				this.InitializeMeetingResponse(meetingResponse, responseType, isCalendarDelegateAccess, proposedStart, proposedEnd);
				flag = true;
			}
			finally
			{
				if (!flag && meetingResponse != null)
				{
					meetingResponse.Dispose();
					meetingResponse = null;
				}
			}
			return meetingResponse;
		}

		private static IconIndex GetIconIndexToSet(ResponseType responseType)
		{
			IconIndex result;
			switch (responseType)
			{
			default:
				throw new ArgumentException(ServerStrings.ExInvalidResponseType(responseType));
			case ResponseType.Tentative:
				result = IconIndex.AppointmentMeetMaybe;
				break;
			case ResponseType.Accept:
				result = IconIndex.AppointmentMeetYes;
				break;
			case ResponseType.Decline:
				result = IconIndex.AppointmentMeetNo;
				break;
			}
			return result;
		}

		public virtual void SendMeetingMessages(bool isToAllAttendees, int? seriesSequenceNumber = null, bool autoCaptureClientIntent = false, bool copyToSentItems = true, string occurrencesViewPropertiesBlob = null, byte[] masterGoid = null)
		{
			this.CheckDisposed("SendMeetingMessages");
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, bool>((long)this.GetHashCode(), "Storage.CalendarItemBase.SendMeetingMessages: GOID={0}; isToAllAttendees={1}", this.GlobalObjectId, isToAllAttendees);
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(34975U, LastChangeAction.SendMeetingMessages);
			if (autoCaptureClientIntent)
			{
				this.SetClientIntentBasedOnModifiedProperties(null);
			}
			MailboxSession mailboxSession = this.SendMessagesProlog();
			this.AdjustIsToAllAttendees(ref isToAllAttendees);
			if (this.MeetingRequestWasSent && this.removedAttendeeArray != null)
			{
				IList<Attendee> list = new List<Attendee>(this.removedAttendeeArray.Length);
				foreach (Attendee attendee in this.removedAttendeeArray)
				{
					if (attendee.IsSendable())
					{
						list.Add(attendee);
					}
				}
				this.SendMeetingCancellations(mailboxSession, isToAllAttendees, list, copyToSentItems, false, null);
			}
			this.SendMeetingRequests(mailboxSession, copyToSentItems, isToAllAttendees, int.MinValue, occurrencesViewPropertiesBlob, seriesSequenceNumber, masterGoid);
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(34165U, LastChangeAction.SendMeetingMessages);
			this.SendMessagesEpilog();
		}

		public void SetClientIntentBasedOnModifiedProperties(ClientIntentFlags? mask)
		{
			this.CheckDisposed("SetClientIntentBasedOnModifiedProperties");
			this.ClientIntent = this.CalculateClientIntentBasedOnModifiedProperties();
			if (base.PropertyBag.IsPropertyDirty(CalendarItemBaseSchema.Location))
			{
				this.ClientIntent |= ClientIntentFlags.ModifiedLocation;
			}
			if (mask != null)
			{
				this.ClientIntent &= mask.Value;
			}
		}

		protected virtual ClientIntentFlags CalculateClientIntentBasedOnModifiedProperties()
		{
			return ClientIntentFlags.None;
		}

		private void SetClientIntentBasedOnResponse(ResponseType responseType, bool intendToSendResponse)
		{
			switch (responseType)
			{
			case ResponseType.Tentative:
				this.SetTentativeIntent(intendToSendResponse);
				return;
			case ResponseType.Accept:
				this.SetAcceptIntent(intendToSendResponse);
				return;
			case ResponseType.Decline:
				this.SetDeclineIntent(intendToSendResponse);
				return;
			default:
				return;
			}
		}

		protected virtual void SetTentativeIntent(bool intendToSendResponse)
		{
			this.ClientIntent = (intendToSendResponse ? ClientIntentFlags.RespondedTentative : ClientIntentFlags.None);
		}

		protected virtual void SetAcceptIntent(bool intendToSendResponse)
		{
			this.ClientIntent = (intendToSendResponse ? ClientIntentFlags.RespondedAccept : ClientIntentFlags.None);
		}

		protected abstract void SetDeclineIntent(bool intendToSendResponse);

		public void UnsafeSetMeetingRequestWasSent(bool value)
		{
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, bool>((long)this.GetHashCode(), "Storage.CalendarItemBase.UnsafeSetMeetingRequestWasSent: GOID={0}; value={1}.", this.GlobalObjectId, value);
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(50549U);
			this[InternalSchema.MeetingRequestWasSent] = value;
		}

		public virtual bool DeleteMeeting(DeleteItemFlags deleteFlag)
		{
			return base.Session.Delete(deleteFlag, new StoreId[]
			{
				base.StoreObjectId
			}).OperationResult == OperationResult.Succeeded;
		}

		public virtual MeetingCancellation CancelMeeting(int? seriesSequenceNumber = null, byte[] masterGoid = null)
		{
			bool flag = false;
			this.CheckDisposed("CancelMeeting");
			if (!this.IsOrganizer())
			{
				throw new InvalidOperationException(ServerStrings.ExCannotCreateMeetingCancellation);
			}
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.CalendarItemBase.CancelMeeting: GOID={0}", this.GlobalObjectId);
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(47477U);
			this[InternalSchema.FreeBusyStatus] = BusyType.Free;
			int num = 0;
			object obj = base.TryGetProperty(InternalSchema.AppointmentState);
			if (!PropertyError.IsPropertyError(obj))
			{
				num = (int)obj;
			}
			if ((num & 4) == 0)
			{
				num |= 4;
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(63861U);
				this[InternalSchema.AppointmentState] = num;
			}
			bool flag2;
			MailboxSession messageMailboxSession = this.GetMessageMailboxSession(out flag2);
			MeetingCancellation meetingCancellation = this.CreateMeetingCancellation(messageMailboxSession, true, seriesSequenceNumber, masterGoid);
			MeetingCancellation result;
			try
			{
				meetingCancellation.CopySendableParticipantsToMessage(this.AttendeeCollection);
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(39285U, LastChangeAction.CancelMeeting);
				this.SaveWithConflictCheck(SaveMode.ResolveConflicts);
				base.Load(null);
				flag = true;
				result = meetingCancellation;
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(meetingCancellation);
				}
			}
			return result;
		}

		public MessageItem CreateReply(StoreObjectId parentFolderId, ReplyForwardConfiguration replyForwardParameters)
		{
			this.CheckDisposed("CreateReply");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(replyForwardParameters, "replyForwardParameters");
			MailboxSession mailboxSession = base.Session as MailboxSession;
			if (mailboxSession == null)
			{
				string message = "CalendarItemBase::CreateReply: Unable to create reply/forward on non-Mailbox session";
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), message);
				throw new NotSupportedException(message);
			}
			return this.CreateReply(mailboxSession, parentFolderId, replyForwardParameters);
		}

		public MessageItem CreateReply(MailboxSession session, StoreObjectId parentFolderId, ReplyForwardConfiguration replyForwardParameters)
		{
			this.CheckDisposed("CreateReply");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(replyForwardParameters, "replyForwardParameters");
			if (!this.IsMeeting)
			{
				throw new NotSupportedException(ServerStrings.AppointmentActionNotSupported("CreateReply"));
			}
			ExTraceGlobals.MeetingMessageTracer.Information((long)this.GetHashCode(), "CalendarItemBase::CreateReply.");
			MessageItem messageItem = null;
			bool flag = false;
			try
			{
				messageItem = base.InternalCreateReply(session, parentFolderId, replyForwardParameters);
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(55669U, LastChangeAction.CreateReply);
				flag = true;
			}
			finally
			{
				if (!flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return messageItem;
		}

		public MessageItem CreateReplyAll(StoreObjectId parentFolderId, ReplyForwardConfiguration replyForwardParameters)
		{
			this.CheckDisposed("CreateReplyAll");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(replyForwardParameters, "replyForwardParameters");
			MailboxSession mailboxSession = base.Session as MailboxSession;
			if (mailboxSession == null)
			{
				string message = "CalendarItemBase::CreateReplyAll: Unable to create reply/forward on non-Mailbox session";
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), message);
				throw new NotSupportedException(message);
			}
			return this.CreateReplyAll(mailboxSession, parentFolderId, replyForwardParameters);
		}

		public MessageItem CreateReplyAll(MailboxSession session, StoreObjectId parentFolderId, ReplyForwardConfiguration replyForwardParameters)
		{
			this.CheckDisposed("CreateReplyAll");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(replyForwardParameters, "replyForwardParameters");
			if (!this.IsMeeting)
			{
				throw new NotSupportedException(ServerStrings.AppointmentActionNotSupported("CreateReplyAll"));
			}
			ExTraceGlobals.MeetingMessageTracer.Information((long)this.GetHashCode(), "CalendarItemBase::CreateReplyAll.");
			bool flag = false;
			MessageItem messageItem = null;
			try
			{
				messageItem = base.InternalCreateReplyAll(session, parentFolderId, replyForwardParameters);
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(43381U, LastChangeAction.CreateReply);
				flag = true;
			}
			finally
			{
				if (!flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return messageItem;
		}

		public MessageItem CreateForward(StoreObjectId parentFolderId, ReplyForwardConfiguration replyForwardParameters)
		{
			this.CheckDisposed("CreateForward");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(replyForwardParameters, "replyForwardParameters");
			bool flag;
			MailboxSession messageMailboxSession = this.GetMessageMailboxSession(out flag);
			return this.CreateForward(messageMailboxSession, parentFolderId, replyForwardParameters, null, null);
		}

		public MessageItem CreateForward(MailboxSession session, StoreObjectId parentFolderId, ReplyForwardConfiguration replyForwardParameters, int? seriesSequenceNumber = null, string occurrencesViewPropertiesBlob = null)
		{
			this.CheckDisposed("CreateForward");
			this.ValidateForwardArguments(session, parentFolderId, replyForwardParameters);
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.CalendarItemBase.CreateForward: GOID={0}", this.GlobalObjectId);
			bool flag = false;
			MessageItem messageItem = null;
			try
			{
				if (this.IsMeeting)
				{
					messageItem = this.ForwardMeeting(session, parentFolderId, replyForwardParameters, seriesSequenceNumber, occurrencesViewPropertiesBlob);
				}
				else
				{
					messageItem = this.ForwardAppointment(session, parentFolderId, replyForwardParameters);
				}
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(59765U, LastChangeAction.CreateForward);
				flag = true;
			}
			finally
			{
				if (!flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return messageItem;
		}

		public void ExportAsICAL(Stream outputStream, string charset, OutboundConversionOptions options)
		{
			this.CheckDisposed("ExportAsICAL");
			Util.ThrowOnNullArgument(options, "options");
			Util.ThrowOnNullOrEmptyArgument(options.ImceaEncapsulationDomain, "options.ImceaEncapsulationDomain");
			OutboundAddressCache addressCache = new OutboundAddressCache(options, new ConversionLimitsTracker(options.Limits));
			addressCache.CopyDataFromItem(this);
			addressCache.Resolve();
			ConvertUtils.CallCts(ExTraceGlobals.ICalTracer, "CalendarItemBase::ExportAsICAL", ServerStrings.ConversionCorruptContent, delegate
			{
				CalendarDocument.ItemToICal(this, null, addressCache, outputStream, charset, options);
			});
		}

		private MailboxSession GetMessageMailboxSession(out bool isCalendarDelegateAccess)
		{
			MailboxSession mailboxSession = base.Session as MailboxSession;
			if (mailboxSession == null)
			{
				throw new NotSupportedException("Calendar work flow is only enabled on MailboxSession.");
			}
			if (mailboxSession.LogonType == LogonType.Delegated && mailboxSession.IsInternallyOpenedDelegateAccess)
			{
				isCalendarDelegateAccess = true;
				return mailboxSession.MasterMailboxSession;
			}
			isCalendarDelegateAccess = false;
			return mailboxSession;
		}

		protected virtual void ValidateForwardArguments(MailboxSession session, StoreObjectId parentFolderId, ReplyForwardConfiguration replyForwardParameters)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(replyForwardParameters, "replyForwardParameters");
		}

		protected override void OnBeforeSave()
		{
			this.CheckDisposed("OnBeforeSave");
			if (!base.IsInMemoryObject)
			{
				bool flag = this.IsCorrelated;
				this.UpdateAttendeesOnException();
				if (this.IsAttendeeListCreated)
				{
					((IAttendeeCollectionImpl)this.AttendeeCollection).Cleanup();
				}
				this.needToCalculateAttendeeDiff = this.IsAttendeeListDirty;
				if (this.CalendarItemType != CalendarItemType.Occurrence)
				{
					AppointmentAuxiliaryFlags valueOrDefault = base.GetValueOrDefault<AppointmentAuxiliaryFlags>(CalendarItemBaseSchema.AppointmentAuxiliaryFlags);
					if ((valueOrDefault & AppointmentAuxiliaryFlags.ForwardedAppointment) != (AppointmentAuxiliaryFlags)0)
					{
						this[CalendarItemBaseSchema.AppointmentAuxiliaryFlags] = valueOrDefault - AppointmentAuxiliaryFlags.ForwardedAppointment;
					}
				}
			}
			base.OnBeforeSave();
			if (!base.IsInMemoryObject)
			{
				this.OnBeforeSaveUpdateChangeHighlights();
				this.OnBeforeSaveUpdateLastChangeAction();
				this.OnBeforeSaveUpdateDisplayAttendees();
				this.OnBeforeSaveUpdateSender();
			}
		}

		private void OnBeforeSaveUpdateChangeHighlights()
		{
			if (base.CoreItem.AreOptionalAutoloadPropertiesLoaded && this.MeetingRequestWasSent)
			{
				ChangeHighlightHelper changeHighlightHelper = MeetingRequest.ComparePropertyBags(this.propertyBagForChangeHighlight, base.PropertyBag);
				if (base.Body.Size != this.originalBodySize)
				{
					base.LocationIdentifierHelperInstance.SetLocationIdentifier(41461U);
					changeHighlightHelper[InternalSchema.HtmlBody] = true;
				}
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(57845U);
				this[InternalSchema.ChangeHighlight] = changeHighlightHelper.HighlightFlags;
			}
		}

		private void OnBeforeSaveUpdateLastChangeAction()
		{
			if (base.PropertyBag.IsDirty)
			{
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(33269U);
				this[InternalSchema.OutlookVersion] = StorageGlobals.ExchangeVersion;
				this[InternalSchema.OutlookInternalVersion] = (int)StorageGlobals.BuildVersion;
			}
		}

		private void OnBeforeSaveUpdateDisplayAttendees()
		{
			if (this.IsOrganizer() && this.AttendeesChanged)
			{
				StorePropertyDefinition[] sourceProperties = new StorePropertyDefinition[]
				{
					InternalSchema.DisplayAll,
					InternalSchema.DisplayTo,
					InternalSchema.DisplayCc
				};
				StorePropertyDefinition[] targetProperties = new StorePropertyDefinition[]
				{
					InternalSchema.DisplayAttendeesAll,
					InternalSchema.DisplayAttendeesTo,
					InternalSchema.DisplayAttendeesCc
				};
				CalendarItemBase.CopyPropertiesTo(this, this, sourceProperties, targetProperties);
			}
		}

		private void OnBeforeSaveUpdateSender()
		{
			object propertyValue = base.TryGetProperty(InternalSchema.Sender);
			object obj = base.TryGetProperty(InternalSchema.From);
			if (PropertyError.IsPropertyError(propertyValue) && !PropertyError.IsPropertyError(obj))
			{
				this[InternalSchema.Sender] = obj;
			}
		}

		internal override VersionedId AssociatedItemId
		{
			get
			{
				this.CheckDisposed("AssociatedItemId");
				return this.associatedId;
			}
			set
			{
				this.CheckDisposed("AssociatedItemId");
				this.associatedId = value;
			}
		}

		internal abstract IAttendeeCollection FetchAttendeeCollection(bool forceOpen);

		internal abstract bool IsAttendeeListDirty { get; }

		internal abstract bool IsAttendeeListCreated { get; }

		protected virtual void InternalUpdateSequencingProperties(bool isToAllAttendees, MeetingMessage message, int minSequenceNumber, int? seriesSequenceNumber = null)
		{
		}

		protected virtual Reminders<EventTimeBasedInboxReminder> FetchEventTimeBasedInboxReminders()
		{
			return Reminders<EventTimeBasedInboxReminder>.Get(this, CalendarItemBaseSchema.EventTimeBasedInboxReminders);
		}

		protected virtual void UpdateEventTimeBasedInboxRemindersForSave(Reminders<EventTimeBasedInboxReminder> reminders)
		{
		}

		private void UpdateSequencingProperties(bool isToAllAttendees, MeetingMessage message, int minSequenceNumber, int? seriesSequenceNumber = null)
		{
			if (seriesSequenceNumber != null)
			{
				message.SeriesSequenceNumber = seriesSequenceNumber.Value;
			}
			this.InternalUpdateSequencingProperties(isToAllAttendees, message, minSequenceNumber, seriesSequenceNumber);
		}

		public static IconIndex CalculateMeetingRequestIcon(MeetingRequest meetingRequest)
		{
			IconIndex result = IconIndex.Default;
			MeetingMessageType meetingRequestType = meetingRequest.MeetingRequestType;
			if (meetingRequestType != MeetingMessageType.NewMeetingRequest && meetingRequestType != MeetingMessageType.FullUpdate)
			{
				if (meetingRequestType == MeetingMessageType.InformationalUpdate)
				{
					result = IconIndex.AppointmentMeetInfo;
				}
			}
			else
			{
				bool valueOrDefault = meetingRequest.GetValueOrDefault<bool>(InternalSchema.AppointmentRecurring);
				if (meetingRequest.Recipients.Count == 0)
				{
					if (valueOrDefault)
					{
						result = IconIndex.AppointmentRecur;
					}
					else
					{
						result = IconIndex.BaseAppointment;
					}
				}
				else if (valueOrDefault)
				{
					result = IconIndex.AppointmentMeetRecur;
				}
				else
				{
					result = IconIndex.AppointmentMeet;
				}
			}
			return result;
		}

		internal void SetChangeHighlight(int highlight)
		{
			if (highlight != 0)
			{
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(45941U);
				this[InternalSchema.ChangeHighlight] = (highlight | base.GetValueOrDefault<int>(InternalSchema.ChangeHighlight));
			}
		}

		protected abstract void SendMeetingCancellations(MailboxSession mailboxSession, bool isToAllAttendees, IList<Attendee> removedAttendeeList, bool copyToSentItems, bool ignoreSendAsRight, CancellationRumInfo rumInfo);

		protected void SendMessage(MailboxSession mailboxSession, MessageItem message, bool copyToSentItems, bool ignoreSendAsRight)
		{
			MeetingMessage.SendLocalOrRemote(message, copyToSentItems, ignoreSendAsRight);
		}

		protected MeetingCancellation CreateMeetingCancellation(MailboxSession mailboxSession, bool isToAllAttendees, int? seriesSequenceNumber = null, byte[] masterGoid = null)
		{
			return this.CreateMeetingCancellation(mailboxSession, isToAllAttendees, int.MinValue, new Action<MeetingCancellation>(this.CopyBodyWithPrefixToMeetingMessage), true, seriesSequenceNumber, masterGoid);
		}

		protected MeetingCancellation CreateMeetingCancellation(MailboxSession mailboxSession, bool isToAllAttendees, int minSequenceNumber, Action<MeetingCancellation> setBodyAndAdjustFlags, bool includeAttachments, int? seriesSequenceNumber = null, byte[] masterGoid = null)
		{
			if (!this.IsOrganizer())
			{
				throw new InvalidOperationException(ServerStrings.ExCannotCreateMeetingCancellation);
			}
			ExTraceGlobals.MeetingMessageTracer.Information<int>((long)this.GetHashCode(), "CalendarItemBase::CreateMeetingCancellation. HashCode = {0}.", this.GetHashCode());
			MeetingCancellation meetingCancellation = null;
			bool flag = false;
			MeetingCancellation result;
			try
			{
				meetingCancellation = this.CreateNewMeetingCancelation(mailboxSession);
				CalendarItemBase.CopyPropertiesTo(this, meetingCancellation, MeetingMessage.MeetingMessageProperties);
				Microsoft.Exchange.Data.Storage.Item.CopyCustomPublicStrings(this, meetingCancellation);
				this.UpdateSequencingProperties(isToAllAttendees, meetingCancellation, minSequenceNumber, seriesSequenceNumber);
				setBodyAndAdjustFlags(meetingCancellation);
				if (this.CalendarItemType == CalendarItemType.RecurringMaster)
				{
					CalendarItemBase.CopyPropertiesTo(this, meetingCancellation, new PropertyDefinition[]
					{
						InternalSchema.AppointmentRecurrenceBlob
					});
				}
				meetingCancellation[InternalSchema.FreeBusyStatus] = BusyType.Free;
				meetingCancellation[InternalSchema.IntendedFreeBusyStatus] = BusyType.Free;
				meetingCancellation[InternalSchema.MeetingRequestType] = MeetingMessageType.FullUpdate;
				meetingCancellation.AdjustAppointmentState();
				meetingCancellation[InternalSchema.SubjectPrefix] = ClientStrings.MeetingCancel.ToString(meetingCancellation.Session.InternalPreferedCulture);
				if (includeAttachments)
				{
					base.ReplaceAttachments(meetingCancellation);
				}
				meetingCancellation.SetOrDeleteProperty(InternalSchema.MasterGlobalObjectId, masterGoid);
				flag = true;
				result = meetingCancellation;
			}
			finally
			{
				if (!flag && meetingCancellation != null)
				{
					meetingCancellation.Dispose();
					meetingCancellation = null;
				}
			}
			return result;
		}

		protected void ResetAttendeeCache()
		{
			if (this.originalAttendeeArray == null)
			{
				this.originalAttendeeArray = this.GetAttendeeArray();
			}
			this.needToCalculateAttendeeDiff = true;
			this.addedAttendeeArray = null;
			this.removedAttendeeArray = null;
		}

		protected void SwapCoreObject(Item newItem)
		{
			ICoreObject coreObject = base.CoreObject;
			base.CoreObject = newItem.CoreObject;
			((IDirectPropertyBag)base.PropertyBag).Context.StoreObject = this;
			newItem.CoreObject = coreObject;
			((IDirectPropertyBag)newItem.PropertyBag).Context.StoreObject = newItem;
		}

		private Attendee[] GetAttendeeArray()
		{
			Attendee[] array = new Attendee[this.AttendeeCollection.Count];
			for (int i = 0; i < this.AttendeeCollection.Count; i++)
			{
				array[i] = this.AttendeeCollection[i];
			}
			return array;
		}

		private void CalculateAttendeeDiff()
		{
			if (!this.NeedToCalculateAttendeeDiff)
			{
				return;
			}
			CalendarItemBase.AttendeeComparer attendeeComparer = new CalendarItemBase.AttendeeComparer(base.Session as MailboxSession);
			Array.Sort(this.originalAttendeeArray, attendeeComparer);
			Attendee[] attendeeArray = this.GetAttendeeArray();
			Array.Sort(attendeeArray, attendeeComparer);
			List<Attendee> list = new List<Attendee>();
			List<Attendee> list2 = new List<Attendee>();
			int i = 0;
			int j = 0;
			while (j < this.originalAttendeeArray.Length)
			{
				if (i >= attendeeArray.Length)
				{
					break;
				}
				int num = attendeeComparer.Compare(this.originalAttendeeArray[j], attendeeArray[i]);
				if (num < 0)
				{
					list2.Add(this.originalAttendeeArray[j]);
					j++;
				}
				else if (num > 0)
				{
					list.Add(attendeeArray[i]);
					i++;
				}
				else
				{
					j++;
					i++;
				}
			}
			while (j < this.originalAttendeeArray.Length)
			{
				list2.Add(this.originalAttendeeArray[j]);
				j++;
			}
			while (i < attendeeArray.Length)
			{
				list.Add(attendeeArray[i]);
				i++;
			}
			this.removedAttendeeArray = list2.ToArray();
			this.addedAttendeeArray = list.ToArray();
			this.needToCalculateAttendeeDiff = false;
		}

		private bool NeedToCalculateAttendeeDiff
		{
			get
			{
				if (this.IsAttendeeListDirty)
				{
					this.needToCalculateAttendeeDiff = true;
				}
				return this.needToCalculateAttendeeDiff;
			}
		}

		internal bool? IsAllDayEventCache
		{
			get
			{
				return this.isAllDayEventCache;
			}
			set
			{
				this.isAllDayEventCache = value;
			}
		}

		protected void CreateCacheForChangeHighlight()
		{
			if (!base.IsNew)
			{
				if (this.propertyBagForChangeHighlight == null)
				{
					this.propertyBagForChangeHighlight = new MemoryPropertyBag();
					this.propertyBagForChangeHighlight.ExTimeZone = base.PropertyBag.ExTimeZone;
				}
				IDirectPropertyBag directPropertyBag = this.propertyBagForChangeHighlight;
				for (int i = 0; i < ChangeHighlightHelper.HighlightProperties.Length; i++)
				{
					object propertyValue = base.TryGetProperty(ChangeHighlightHelper.HighlightProperties[i]);
					directPropertyBag.SetValue(ChangeHighlightHelper.HighlightProperties[i], propertyValue);
				}
				if (((IDirectPropertyBag)base.PropertyBag).IsLoaded(InternalSchema.TextBody))
				{
					this.originalBodySize = base.Body.Size;
				}
			}
		}

		public static void CopyPropertiesTo(StoreObject sourceItem, StoreObject targetItem, params PropertyDefinition[] properties)
		{
			CalendarItemBase.CopyPropertiesTo(sourceItem, targetItem, properties, properties);
		}

		public static void CopyPropertiesTo(StoreObject sourceItem, StoreObject targetItem, PropertyDefinition[] sourceProperties, PropertyDefinition[] targetProperties)
		{
			if (sourceProperties.Length != targetProperties.Length)
			{
				throw new ArgumentException("Property arrays with different sizes");
			}
			for (int i = 0; i < sourceProperties.Length; i++)
			{
				PropertyDefinition propertyDefinition = sourceProperties[i];
				PropertyDefinition propertyDefinition2 = targetProperties[i];
				object obj = sourceItem.TryGetProperty(propertyDefinition);
				if (!PropertyError.IsPropertyError(obj))
				{
					if (obj != null)
					{
						object obj2;
						try
						{
							obj2 = targetItem.TryGetProperty(propertyDefinition2);
						}
						catch (NotInBagPropertyErrorException)
						{
							obj2 = null;
						}
						if (propertyDefinition2.Equals(InternalSchema.SentRepresentingEmailAddress) || propertyDefinition2.Equals(InternalSchema.SenderEmailAddress))
						{
							if (!CalendarItemBase.AreEqualIgnoreCase(obj2, obj))
							{
								targetItem.LocationIdentifierHelperInstance.SetLocationIdentifier(37996U);
								targetItem[propertyDefinition2] = obj;
							}
						}
						else if (!Util.ValueEquals(obj2, obj))
						{
							targetItem.LocationIdentifierHelperInstance.SetLocationIdentifier(49525U);
							targetItem[propertyDefinition2] = obj;
						}
					}
				}
				else if (PropertyError.IsPropertyValueTooBig(obj))
				{
					using (Stream stream = sourceItem.OpenPropertyStream(propertyDefinition, PropertyOpenMode.ReadOnly))
					{
						using (Stream stream2 = targetItem.OpenPropertyStream(propertyDefinition2, PropertyOpenMode.Create))
						{
							Util.StreamHandler.CopyStreamData(stream, stream2);
						}
					}
				}
			}
		}

		public static string CreateWhenStringForBodyPrefix(Item item, ExTimeZone preferredTimeZone = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0} {1}", ClientStrings.WhenPart.ToString(item.Session.InternalPreferedCulture), CalendarItem.InternalWhen(item, null, true, preferredTimeZone).ToString(item.Session.InternalPreferedCulture));
			stringBuilder.AppendLine();
			string valueOrDefault = item.GetValueOrDefault<string>(InternalSchema.Location);
			if (!string.IsNullOrEmpty(valueOrDefault))
			{
				stringBuilder.AppendFormat("{0} {1}", ClientStrings.WherePart.ToString(item.Session.InternalPreferedCulture), valueOrDefault);
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine();
			stringBuilder.Append("*~*~*~*~*~*~*~*~*~*");
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}

		internal bool IsCorrelated
		{
			get
			{
				return this.isCorrelated;
			}
			set
			{
				this.isCorrelated = value;
			}
		}

		private static bool AreEqualIgnoreCase(object propValue, object originalValue)
		{
			return propValue is string && originalValue is string && ((string)propValue).Equals((string)originalValue, StringComparison.OrdinalIgnoreCase);
		}

		private void AdjustIsToAllAttendees(ref bool isToAllAttendees)
		{
			if (!isToAllAttendees && this.removedAttendeeArray != null && this.removedAttendeeArray.Length != 0)
			{
				foreach (Attendee attendee in Util.CompositeEnumerator<Attendee>(new IEnumerable<Attendee>[]
				{
					this.AttendeeCollection,
					this.removedAttendeeArray
				}))
				{
					bool? flag = attendee.Participant.IsRoutable(null);
					if (flag != null && flag.Value)
					{
						bool flag2 = attendee.IsDistributionList() ?? true;
						if (flag2)
						{
							isToAllAttendees = true;
							break;
						}
					}
				}
			}
		}

		public GlobalObjectId GlobalObjectId
		{
			get
			{
				if (this.cachedGlobalObjectId != null)
				{
					return this.cachedGlobalObjectId;
				}
				byte[] valueOrDefault = base.GetValueOrDefault<byte[]>(CalendarItemBaseSchema.GlobalObjectId);
				if (valueOrDefault != null)
				{
					this.cachedGlobalObjectId = new GlobalObjectId(valueOrDefault);
				}
				return this.cachedGlobalObjectId;
			}
		}

		public static bool IsTenantToBeFixed(MailboxSession mailboxSession)
		{
			if (mailboxSession == null || mailboxSession.MailboxOwner == null)
			{
				return false;
			}
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(mailboxSession.MailboxOwner.GetContext(null), null, null);
			return snapshot.CalendarLogging.FixMissingMeetingBody.Enabled;
		}

		internal virtual void SendUpdateRums(UpdateRumInfo rumInfo, bool copyToSentItems)
		{
			this.CheckDisposed("SendUpdateRums");
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.CalendarItemBase.SendUpdateRums: GOID={0};", this.GlobalObjectId);
			if (!this.IsOrganizer())
			{
				ExTraceGlobals.MeetingMessageTracer.Information((long)this.GetHashCode(), "Storage.CalendarItemBase.SendUpdateRums: Invalid organizer, skip sending RUMs");
				return;
			}
			if (this.MeetingRequestWasSent)
			{
				MailboxSession mailboxSession = this.SendMessagesProlog();
				this.SendRumRequest(mailboxSession, rumInfo, copyToSentItems);
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(49141U, LastChangeAction.UpdateRumSent);
				this.SendMessagesEpilog();
			}
		}

		internal void SendForwardRum(UpdateRumInfo rumInfo, bool copyToSentItems)
		{
			this.CheckDisposed("SendForwardRum");
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.CalendarItemBase.SendForwardRum: GOID={0};", this.GlobalObjectId);
			if (this.MeetingRequestWasSent)
			{
				ForwardCreation forwardCreation = null;
				bool flag;
				MailboxSession messageMailboxSession = this.GetMessageMailboxSession(out flag);
				ReplyForwardConfiguration replyForwardParameters = new ReplyForwardConfiguration(BodyFormat.TextHtml, ForwardCreationFlags.None, messageMailboxSession.InternalCulture);
				using (MeetingRequest meetingRequest = this.ForwardMeeting(messageMailboxSession, CalendarItemBase.GetDraftsFolderIdOrThrow(messageMailboxSession), replyForwardParameters, out forwardCreation, null, null))
				{
					meetingRequest.CopySendableParticipantsToMessage(rumInfo.AttendeeList);
					this.AdjustRumMessage(messageMailboxSession, meetingRequest, rumInfo, false);
					if (rumInfo is MissingAttendeeItemRumInfo)
					{
						meetingRequest[InternalSchema.ChangeHighlight] = ChangeHighlightProperties.BodyProps;
					}
					this.SendMessage(messageMailboxSession, meetingRequest, copyToSentItems, true);
					base.LocationIdentifierHelperInstance.SetLocationIdentifier(61941U, LastChangeAction.ForwardRumSent);
				}
			}
		}

		internal virtual void SendCancellationRums(CancellationRumInfo rumInfo, bool copyToSentItems)
		{
			this.CheckDisposed("SendCancellationRums");
			if (this.MeetingRequestWasSent)
			{
				bool flag;
				MailboxSession messageMailboxSession = this.GetMessageMailboxSession(out flag);
				this.SendMeetingCancellations(messageMailboxSession, false, rumInfo.AttendeeList, copyToSentItems, true, rumInfo);
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(37365U, LastChangeAction.CancellationRumSent);
				this.SaveWithConflictCheck(SaveMode.ResolveConflicts);
			}
		}

		internal virtual void SendResponseRum(ResponseRumInfo rumInfo, bool copyToSentItems)
		{
			this.CheckDisposed("SendResponseRum");
			bool isCalendarDelegateAccess;
			MailboxSession messageMailboxSession = this.GetMessageMailboxSession(out isCalendarDelegateAccess);
			using (MeetingResponse meetingResponse = this.CreateResponse(messageMailboxSession, this.ResponseType, isCalendarDelegateAccess, null, null))
			{
				this.AdjustRumMessage(messageMailboxSession, meetingResponse, rumInfo, false);
				this.SendMessage(messageMailboxSession, meetingResponse, copyToSentItems, true);
			}
		}

		internal void SendAttendeeInquiryRum(AttendeeInquiryRumInfo rumInfo, bool copyToSentItems)
		{
			this.SendAttendeeInquiryRum(rumInfo, copyToSentItems, null);
		}

		internal virtual void SendAttendeeInquiryRum(AttendeeInquiryRumInfo rumInfo, bool copyToSentItems, string subjectOverride)
		{
			this.CheckDisposed("SendAttendeeInquiryRum");
			bool flag;
			MailboxSession messageMailboxSession = this.GetMessageMailboxSession(out flag);
			using (MeetingInquiryMessage meetingInquiryMessage = MeetingInquiryMessage.Create(messageMailboxSession, CalendarItemBase.GetDraftsFolderIdOrThrow(messageMailboxSession), rumInfo))
			{
				meetingInquiryMessage.GlobalObjectId = this.GlobalObjectId;
				meetingInquiryMessage.Recipients.Add(this.Organizer);
				meetingInquiryMessage.Subject = (subjectOverride ?? base.GetValueOrDefault<string>(ItemSchema.NormalizedSubject));
				this.SendMessage(messageMailboxSession, meetingInquiryMessage, copyToSentItems, true);
			}
		}

		internal MeetingRequest InternalCreateMeetingRequest(MailboxSession mailboxSession, bool isToAllAttendees, IList<Attendee> attendeeRecipients, Action<MeetingRequest> setBodyAndAdjustFlags, int changeHighlights, int minSequenceNumber, MeetingMessageType requestType, bool sendAttachments, string occurrencesViewPropertiesBlob, int? seriesSequenceNumber = null, byte[] masterGoid = null)
		{
			MeetingRequest meetingRequest = this.CreateNewMeetingRequest(mailboxSession);
			this.InitializeMeetingRequest(setBodyAndAdjustFlags, meetingRequest);
			if (!this.MeetingRequestWasSent)
			{
				isToAllAttendees = true;
			}
			this.UpdateSequencingProperties(isToAllAttendees, meetingRequest, minSequenceNumber, seriesSequenceNumber);
			meetingRequest.AdjustAppointmentState();
			meetingRequest[InternalSchema.ChangeHighlight] = changeHighlights;
			meetingRequest.MeetingRequestType = requestType;
			BusyType valueOrDefault = base.GetValueOrDefault<BusyType>(InternalSchema.FreeBusyStatus, BusyType.Busy);
			meetingRequest[InternalSchema.IntendedFreeBusyStatus] = valueOrDefault;
			meetingRequest[InternalSchema.FreeBusyStatus] = ((valueOrDefault != BusyType.Free) ? BusyType.Tentative : BusyType.Free);
			meetingRequest[InternalSchema.AppointmentClass] = this.ItemClass;
			meetingRequest.CopySendableParticipantsToMessage(attendeeRecipients);
			if (!isToAllAttendees)
			{
				List<BlobRecipient> list = new List<BlobRecipient>();
				foreach (Attendee attendee in this.AttendeeCollection)
				{
					if (!this.ContainsAttendeeParticipant(attendeeRecipients, attendee))
					{
						list.Add(new BlobRecipient(attendee));
					}
				}
				meetingRequest.SetUnsendableRecipients(list);
			}
			if (sendAttachments)
			{
				base.ReplaceAttachments(meetingRequest);
			}
			meetingRequest[InternalSchema.IconIndex] = CalendarItemBase.CalculateMeetingRequestIcon(meetingRequest);
			meetingRequest[InternalSchema.CalendarProcessingSteps] = 0;
			if (!string.IsNullOrEmpty(occurrencesViewPropertiesBlob))
			{
				meetingRequest.OccurrencesExceptionalViewProperties = occurrencesViewPropertiesBlob;
			}
			meetingRequest.SetOrDeleteProperty(InternalSchema.MasterGlobalObjectId, masterGoid);
			return meetingRequest;
		}

		protected virtual void CopyMeetingRequestProperties(MeetingRequest meetingRequest)
		{
			CalendarItemBase.CopyPropertiesTo(this, meetingRequest, MeetingMessage.MeetingMessageProperties);
			CalendarItemBase.CopyPropertiesTo(this, meetingRequest, MeetingMessage.WriteOnCreateProperties);
		}

		protected abstract MeetingRequest CreateNewMeetingRequest(MailboxSession mailboxSession);

		protected abstract MeetingCancellation CreateNewMeetingCancelation(MailboxSession mailboxSession);

		protected abstract MeetingResponse CreateNewMeetingResponse(MailboxSession mailboxSession, ResponseType responseType);

		protected abstract void SetSequencingPropertiesForForward(MeetingRequest meetingRequest);

		protected virtual void InitializeMeetingRequest(Action<MeetingRequest> setBodyAndAdjustFlags, MeetingRequest meetingRequest)
		{
			this.CopyMeetingRequestProperties(meetingRequest);
			if (setBodyAndAdjustFlags != null)
			{
				setBodyAndAdjustFlags(meetingRequest);
			}
		}

		protected virtual void UpdateAttendeesOnException()
		{
		}

		public void SaveWithConflictCheck(SaveMode saveMode)
		{
			ConflictResolutionResult conflictResolutionResult = base.Save(saveMode);
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(base.InternalObjectId), conflictResolutionResult);
			}
		}

		protected abstract bool IsInThePast { get; }

		private void SendMeetingRequests(MailboxSession mailboxSession, bool copyToSentItems, bool isToAllAttendees, int minSequenceNumber, string occurrencesViewPropertiesBlob = null, int? seriesSequenceNumber = null, byte[] masterGoid = null)
		{
			int num = 0;
			MeetingMessageType meetingMessageType = MeetingMessageType.NewMeetingRequest;
			if (this.MeetingRequestWasSent)
			{
				ChangeHighlightHelper changeHighlightHelper = MeetingRequest.CompareForChangeHighlightOnUpdatedItems(this.propertyBagForChangeHighlight, base.Body.Size, base.PropertyBag, this.originalBodySize);
				num = changeHighlightHelper.HighlightFlags;
				meetingMessageType = changeHighlightHelper.SuggestedMeetingType;
				int valueOrDefault = base.GetValueOrDefault<int>(InternalSchema.ChangeHighlight, 0);
				num |= valueOrDefault;
			}
			IList<Attendee> list = isToAllAttendees ? this.AttendeeCollection : ((IList<Attendee>)this.addedAttendeeArray);
			if (meetingMessageType == MeetingMessageType.FullUpdate)
			{
				foreach (Attendee attendee in list)
				{
					attendee.ResponseType = ResponseType.None;
					if (this.IsCalendarItemTypeOccurrenceOrException)
					{
						attendee.RecipientFlags |= RecipientFlags.ExceptionalResponse;
					}
				}
			}
			this.SendMeetingRequests(mailboxSession, copyToSentItems, isToAllAttendees, list, new Action<MeetingRequest>(this.CopyBodyWithPrefixToMeetingMessage), num, minSequenceNumber, meetingMessageType, true, false, occurrencesViewPropertiesBlob, seriesSequenceNumber, masterGoid);
		}

		private void CopyBodyWithPrefixToMeetingMessage(MeetingMessage message)
		{
			ReplyForwardConfiguration configuration = new ReplyForwardConfiguration(base.Body.Format);
			ReplyForwardCommon.CopyBodyWithPrefix(base.Body, message.Body, configuration, default(BodyConversionCallbacks));
		}

		private void SendRumRequest(MailboxSession mailboxSession, UpdateRumInfo rumInfo, bool copyToSentItems)
		{
			Action<MeetingRequest> setBodyAndAdjustFlags = (rumInfo is MissingAttendeeItemRumInfo && CalendarItemBase.IsTenantToBeFixed(mailboxSession)) ? delegate(MeetingRequest meetingRequest)
			{
				this.AdjustRumMessage2(mailboxSession, meetingRequest, rumInfo);
			} : delegate(MeetingRequest meetingRequest)
			{
				this.AdjustRumMessage(mailboxSession, meetingRequest, rumInfo, false);
			};
			this.SendMeetingRequests(mailboxSession, copyToSentItems, false, rumInfo.AttendeeList, setBodyAndAdjustFlags, (rumInfo is MissingAttendeeItemRumInfo) ? 128 : 0, rumInfo.AttendeeRequiredSequenceNumber, MeetingMessageType.InformationalUpdate, false, true, null, null, null);
		}

		protected void AdjustRumMessage2(MailboxSession mailboxSession, MeetingMessage message, RumInfo rumInfo)
		{
			this.CopyBodyWithPrefixToMeetingMessage(message);
			this.AdjustRumMessage(mailboxSession, message, rumInfo, true);
		}

		protected void AdjustRumMessage(MailboxSession mailboxSession, MessageItem message, RumInfo rumInfo, bool skipBodyUpdate = false)
		{
			RumDecorator rumDecorator = RumDecorator.CreateInstance(rumInfo);
			rumDecorator.AdjustRumMessage(mailboxSession, message, rumInfo, null, skipBodyUpdate);
		}

		private void SendMeetingRequests(MailboxSession mailboxSession, bool copyToSentItems, bool isToAllAttendees, IList<Attendee> attendeeRecipients, Action<MeetingRequest> setBodyAndAdjustFlags, int changeHighlights, int minSequenceNumber, MeetingMessageType requestType, bool sendAttachments, bool ignoreSendAsRight, string occurrencesViewPropertiesBlob = null, int? seriesSequenceNumber = null, byte[] masterGoid = null)
		{
			if (attendeeRecipients == null || attendeeRecipients.Count == 0)
			{
				ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.CalendarItemBase.SendMeetingRequests: GOID={0}; There are no attendees to send to.", this.GlobalObjectId);
				return;
			}
			using (MeetingRequest meetingRequest = this.InternalCreateMeetingRequest(mailboxSession, isToAllAttendees, attendeeRecipients, setBodyAndAdjustFlags, changeHighlights, minSequenceNumber, requestType, sendAttachments, occurrencesViewPropertiesBlob, seriesSequenceNumber, masterGoid))
			{
				this.SendMessage(mailboxSession, meetingRequest, copyToSentItems, ignoreSendAsRight);
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(53621U);
				this[InternalSchema.MeetingRequestWasSent] = true;
				this.CreateCacheForChangeHighlight();
			}
		}

		private bool ContainsAttendeeParticipant(IList<Attendee> list, Attendee attendee)
		{
			if (attendee == null)
			{
				throw new ArgumentNullException("attendee");
			}
			if (attendee.Participant == null)
			{
				throw new ArgumentNullException("attendee.Participant");
			}
			foreach (Attendee attendee2 in list)
			{
				if (attendee2 == attendee)
				{
					return true;
				}
				if (attendee2 != null && attendee2.Participant != null && (attendee2.Participant == attendee.Participant || Participant.HasSameEmail(attendee2.Participant, attendee.Participant, base.Session as MailboxSession, true)))
				{
					return true;
				}
			}
			return false;
		}

		private MessageItem ForwardAppointment(MailboxSession session, StoreObjectId parentFolderId, ReplyForwardConfiguration replyForwardParameters)
		{
			MessageItem messageItem = null;
			bool flag = false;
			MessageItem result;
			try
			{
				messageItem = MessageItem.Create(session, parentFolderId);
				messageItem[InternalSchema.SubjectPrefix] = ClientStrings.ItemForward.ToString(session.InternalPreferedCulture);
				messageItem[InternalSchema.NormalizedSubject] = base.GetValueOrDefault<string>(InternalSchema.NormalizedSubjectInternal, string.Empty);
				using (Attachment attachment = messageItem.AttachmentCollection.AddExistingItem(this))
				{
					attachment.Save();
				}
				flag = true;
				result = messageItem;
			}
			finally
			{
				if (!flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return result;
		}

		private MeetingRequest ForwardMeeting(MailboxSession session, StoreObjectId parentFolderId, ReplyForwardConfiguration replyForwardParameters, int? seriesSequenceNumber, string occurrencesViewPropertiesBlob)
		{
			bool flag = false;
			ForwardCreation forwardCreation = null;
			MeetingRequest meetingRequest = null;
			try
			{
				meetingRequest = this.ForwardMeeting(session, parentFolderId, replyForwardParameters, out forwardCreation, seriesSequenceNumber, occurrencesViewPropertiesBlob);
				forwardCreation.PopulateContents();
				meetingRequest.AdjustAppointmentStateFlagsForForward();
				flag = true;
			}
			finally
			{
				if (!flag && meetingRequest != null)
				{
					meetingRequest.Dispose();
					meetingRequest = null;
				}
			}
			return meetingRequest;
		}

		private MeetingRequest ForwardMeeting(MailboxSession session, StoreObjectId parentFolderId, ReplyForwardConfiguration replyForwardParameters, out ForwardCreation forward, int? seriesSequenceNumber = null, string occurrencesViewPropertiesBlob = null)
		{
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.CalendarItemBase.ForwardMeeting: GOID={0}", this.GlobalObjectId);
			MeetingRequest meetingRequest = null;
			bool flag = false;
			MeetingRequest result;
			try
			{
				meetingRequest = this.CreateNewMeetingRequest(session);
				forward = new ForwardCreation(this, meetingRequest, replyForwardParameters);
				forward.PopulateProperties(false);
				List<BlobRecipient> list = new List<BlobRecipient>();
				foreach (Attendee attendee in this.AttendeeCollection)
				{
					if ((attendee.RecipientFlags & RecipientFlags.Organizer) != RecipientFlags.Organizer)
					{
						list.Add(new BlobRecipient(attendee));
					}
				}
				meetingRequest.SetUnsendableRecipients(list);
				this.SetSequencingPropertiesForForward(meetingRequest);
				if (seriesSequenceNumber != null)
				{
					meetingRequest.SeriesSequenceNumber = seriesSequenceNumber.Value;
				}
				if (!string.IsNullOrEmpty(occurrencesViewPropertiesBlob))
				{
					meetingRequest.OccurrencesExceptionalViewProperties = occurrencesViewPropertiesBlob;
				}
				flag = true;
				result = meetingRequest;
			}
			finally
			{
				if (!flag && meetingRequest != null)
				{
					meetingRequest.Dispose();
					meetingRequest = null;
				}
			}
			return result;
		}

		private MailboxSession SendMessagesProlog()
		{
			if (!this.IsOrganizer())
			{
				throw new InvalidOperationException(ServerStrings.ExCannotSendMeetingMessages);
			}
			this.IsMeeting = true;
			((IAttendeeCollectionImpl)this.AttendeeCollection).Cleanup();
			((IAttendeeCollectionImpl)this.AttendeeCollection).LoadIsDistributionList();
			this.SaveWithConflictCheck(SaveMode.ResolveConflicts);
			base.Load(null);
			this.CalculateAttendeeDiff();
			bool flag;
			return this.GetMessageMailboxSession(out flag);
		}

		private void SendMessagesEpilog()
		{
			this.originalAttendeeArray = null;
			this.ResetAttendeeCache();
			this.SaveWithConflictCheck(SaveMode.ResolveConflicts);
		}

		protected const bool DefaultAutoCaptureClientIntent = false;

		protected const bool DefaultCopyToSentItems = true;

		protected const string DefaultOccurrencesViewPropertiesBlob = null;

		private const int DefaultPublishingMonths = 2;

		private const int MapiBusyTypeFree = 0;

		private const int MapiBusyTypeTentative = 1;

		private const int MapiBusyTypeBusy = 2;

		private const int MapiBusyTypeOOF = 3;

		private const BusyType DefaultBusyType = BusyType.Busy;

		private static readonly PropertyDefinition[] AppointmentTombstomeDefinition = new PropertyDefinition[]
		{
			InternalSchema.AppointmentTombstones,
			InternalSchema.OutlookFreeBusyMonthCount
		};

		private Attendee[] originalAttendeeArray;

		private Attendee[] addedAttendeeArray;

		private Attendee[] removedAttendeeArray;

		private bool needToCalculateAttendeeDiff;

		private bool? isAllDayEventCache;

		private MemoryPropertyBag propertyBagForChangeHighlight;

		private long originalBodySize;

		private VersionedId associatedId;

		private Reminders<EventTimeBasedInboxReminder> eventTimeBasedInboxReminders;

		private RemindersState<EventTimeBasedInboxReminderState> eventTimeBasedInboxRemindersState;

		private GlobalObjectId cachedGlobalObjectId;

		private bool isCorrelated;

		internal static ExDateTime OutlookRtmNone = new ExDateTime(ExTimeZone.UtcTimeZone, 4501, 1, 1, 0, 0, 0);

		private class AttendeeComparer : IComparer
		{
			public AttendeeComparer(MailboxSession session)
			{
				this.session = session;
			}

			public int Compare(object x, object y)
			{
				Attendee attendee = x as Attendee;
				Attendee attendee2 = y as Attendee;
				if (attendee == null || attendee2 == null)
				{
					throw new ArgumentException();
				}
				if (Participant.HasSameEmail(attendee.Participant, attendee2.Participant, this.session, false))
				{
					return 0;
				}
				if (attendee.Participant == null)
				{
					return -1;
				}
				if (attendee2.Participant == null)
				{
					return 1;
				}
				if (attendee.Participant.EmailAddress == null != (attendee2.Participant.EmailAddress == null))
				{
					if (attendee.Participant.EmailAddress == null)
					{
						return -1;
					}
					return 1;
				}
				else
				{
					if (attendee.Participant.EmailAddress == null)
					{
						return string.Compare(attendee.Participant.ToString(), attendee2.Participant.ToString(), StringComparison.CurrentCulture);
					}
					return string.Compare(attendee.Participant.EmailAddress, attendee2.Participant.EmailAddress, StringComparison.CurrentCulture);
				}
			}

			private MailboxSession session;
		}
	}
}
