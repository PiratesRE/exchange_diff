using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarItemBaseSchema : ItemSchema
	{
		public new static CalendarItemBaseSchema Instance
		{
			get
			{
				if (CalendarItemBaseSchema.instance == null)
				{
					CalendarItemBaseSchema.instance = new CalendarItemBaseSchema();
				}
				return CalendarItemBaseSchema.instance;
			}
		}

		protected override void AddConstraints(List<StoreObjectConstraint> constraints)
		{
			base.AddConstraints(constraints);
			constraints.Add(new OrganizerPropertiesConstraint());
			constraints.Add(new CalendarOriginatorIdConstraint());
		}

		internal override void CoreObjectUpdate(CoreItem coreItem, CoreItemOperation operation)
		{
			CalendarItemBase.CoreObjectUpdateLocationAddress(coreItem);
			base.CoreObjectUpdate(coreItem, operation);
		}

		protected override ICollection<PropertyRule> PropertyRules
		{
			get
			{
				if (this.propertyRulesCache == null)
				{
					this.propertyRulesCache = base.PropertyRules.Concat(CalendarItemBaseSchema.CalendarItemBasePropertyRules);
				}
				return this.propertyRulesCache;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static CalendarItemBaseSchema()
		{
			PropertyRule[] array = new PropertyRule[4];
			array[0] = PropertyRuleLibrary.DefaultClientIntent;
			array[1] = PropertyRuleLibrary.ResponseAndReplyRequested;
			array[2] = new SequenceCompositePropertyRule(string.Empty, delegate(ILocationIdentifierSetter lidSetter)
			{
				lidSetter.SetLocationIdentifier(60447U, LastChangeAction.SequenceCompositePropertyRuleApplied);
			}, new PropertyRule[]
			{
				PropertyRuleLibrary.EventLocationRule
			});
			array[3] = PropertyRuleLibrary.HasAttendees;
			CalendarItemBaseSchema.CalendarItemBasePropertyRules = array;
			CalendarItemBaseSchema.instance = null;
		}

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition AttendeeCriticalChangeTime = InternalSchema.AttendeeCriticalChangeTime;

		[Autoload]
		public static readonly StorePropertyDefinition OldLocation = InternalSchema.OldLocation;

		[Autoload]
		public static readonly StorePropertyDefinition MeetingRequestType = InternalSchema.MeetingRequestType;

		[Autoload]
		public static readonly StorePropertyDefinition OwnerCriticalChangeTime = InternalSchema.OwnerCriticalChangeTime;

		[Autoload]
		internal static readonly StorePropertyDefinition OnlineMeetingChanged = InternalSchema.OnlineMeetingChanged;

		[Autoload]
		public static readonly StorePropertyDefinition Duration = InternalSchema.Duration;

		[Autoload]
		public static readonly StorePropertyDefinition IsDraft = InternalSchema.IsDraft;

		[Autoload]
		internal static readonly StorePropertyDefinition IsSilent = InternalSchema.IsSilent;

		[Autoload]
		internal static readonly StorePropertyDefinition NetMeetingServer = InternalSchema.NetMeetingServer;

		[Autoload]
		internal static readonly StorePropertyDefinition NetMeetingOrganizerAlias = InternalSchema.NetMeetingOrganizerAlias;

		[Autoload]
		internal static readonly StorePropertyDefinition NetMeetingDocPathName = InternalSchema.NetMeetingDocPathName;

		[Autoload]
		internal static readonly StorePropertyDefinition NetMeetingConferenceServerAllowExternal = InternalSchema.NetMeetingConferenceServerAllowExternal;

		[Autoload]
		internal static readonly StorePropertyDefinition NetMeetingConferenceSerPassword = InternalSchema.NetMeetingConferenceSerPassword;

		[LegalTracking]
		[Autoload]
		internal static readonly StorePropertyDefinition ReceivedBy = InternalSchema.ReceivedBy;

		[Autoload]
		internal static readonly StorePropertyDefinition ReceivedRepresenting = InternalSchema.ReceivedRepresenting;

		[Autoload]
		internal static readonly StorePropertyDefinition MapiSubject = InternalSchema.MapiSubject;

		[Autoload]
		public static readonly StorePropertyDefinition ConferenceType = InternalSchema.ConferenceType;

		[Autoload]
		public static readonly StorePropertyDefinition NetShowURL = InternalSchema.NetShowURL;

		[Autoload]
		public static readonly StorePropertyDefinition DisallowNewTimeProposal = InternalSchema.DisallowNewTimeProposal;

		[Autoload]
		public static readonly StorePropertyDefinition MeetingWorkspaceUrl = InternalSchema.MeetingWorkspaceUrl;

		[Autoload]
		internal static readonly StorePropertyDefinition MarkedForDownload = InternalSchema.MarkedForDownload;

		[Autoload]
		internal static readonly StorePropertyDefinition Mileage = InternalSchema.Mileage;

		[Autoload]
		internal static readonly StorePropertyDefinition Companies = InternalSchema.Companies;

		[Autoload]
		internal static readonly StorePropertyDefinition BillingInformation = InternalSchema.BillingInformation;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentLastSequenceNumber = InternalSchema.AppointmentLastSequenceNumber;

		[Autoload]
		internal static readonly StorePropertyDefinition CdoSequenceNumber = InternalSchema.CdoSequenceNumber;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentRecurrenceBlob = InternalSchema.AppointmentRecurrenceBlob;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentRecurring = InternalSchema.AppointmentRecurring;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentSequenceNumber = InternalSchema.AppointmentSequenceNumber;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentSequenceTime = InternalSchema.AppointmentSequenceTime;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentState = InternalSchema.AppointmentState;

		[Autoload]
		public static readonly StorePropertyDefinition ChangeHighlight = InternalSchema.ChangeHighlight;

		[Autoload]
		public static readonly StorePropertyDefinition GlobalObjectId = InternalSchema.GlobalObjectId;

		[Autoload]
		public static readonly StorePropertyDefinition CleanGlobalObjectId = InternalSchema.CleanGlobalObjectId;

		[Autoload]
		public static readonly StorePropertyDefinition EventClientId = InternalSchema.EventClientId;

		[Autoload]
		public static readonly StorePropertyDefinition SeriesId = InternalSchema.SeriesId;

		[Autoload]
		public static readonly StorePropertyDefinition IsHiddenFromLegacyClients = InternalSchema.IsHiddenFromLegacyClients;

		[Autoload]
		public static readonly StorePropertyDefinition MeetingUniqueId = InternalSchema.MeetingUniqueId;

		[Autoload]
		public static readonly StorePropertyDefinition FreeBusyStatus = InternalSchema.FreeBusyStatus;

		[Autoload]
		public static readonly StorePropertyDefinition AllAttachmentsHidden = InternalSchema.AllAttachmentsHidden;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition IsAllDayEvent = InternalSchema.IsAllDayEvent;

		[Autoload]
		public static readonly StorePropertyDefinition IsEvent = InternalSchema.IsEvent;

		[Autoload]
		public static readonly StorePropertyDefinition IsException = InternalSchema.IsException;

		[Autoload]
		public static readonly StorePropertyDefinition IsMeeting = InternalSchema.IsMeeting;

		[Autoload]
		public static readonly StorePropertyDefinition IsOnlineMeeting = InternalSchema.IsOnlineMeeting;

		[Autoload]
		internal static readonly StorePropertyDefinition OutlookUserPropsFormStorage = InternalSchema.OutlookUserPropsFormStorage;

		[Autoload]
		internal static readonly StorePropertyDefinition OutlookUserPropsScriptStream = InternalSchema.OutlookUserPropsScriptStream;

		[Autoload]
		internal static readonly StorePropertyDefinition OutlookUserPropsFormPropStream = InternalSchema.OutlookUserPropsFormPropStream;

		[Autoload]
		internal static readonly StorePropertyDefinition OutlookUserPropsPageDirStream = InternalSchema.OutlookUserPropsPageDirStream;

		[Autoload]
		internal static readonly StorePropertyDefinition OutlookUserPropsVerbStream = InternalSchema.OutlookUserPropsVerbStream;

		[Autoload]
		internal static readonly StorePropertyDefinition OutlookUserPropsPropDefStream = InternalSchema.OutlookUserPropsPropDefStream;

		[Autoload]
		internal static readonly StorePropertyDefinition OutlookUserPropsCustomFlag = InternalSchema.OutlookUserPropsCustomFlag;

		[Autoload]
		public static readonly StorePropertyDefinition IsRecurring = InternalSchema.IsRecurring;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition Location = InternalSchema.Location;

		[Autoload]
		public static readonly StorePropertyDefinition LocationDisplayName = InternalSchema.LocationDisplayName;

		[Autoload]
		public static readonly StorePropertyDefinition LocationAnnotation = InternalSchema.LocationAnnotation;

		[Autoload]
		public static readonly StorePropertyDefinition LocationSource = InternalSchema.LocationSource;

		[Autoload]
		public static readonly StorePropertyDefinition LocationUri = InternalSchema.LocationUri;

		[Autoload]
		public static readonly StorePropertyDefinition Latitude = InternalSchema.Latitude;

		[Autoload]
		public static readonly StorePropertyDefinition Longitude = InternalSchema.Longitude;

		[Autoload]
		public static readonly StorePropertyDefinition Accuracy = InternalSchema.Accuracy;

		[Autoload]
		public static readonly StorePropertyDefinition Altitude = InternalSchema.Altitude;

		[Autoload]
		public static readonly StorePropertyDefinition AltitudeAccuracy = InternalSchema.AltitudeAccuracy;

		[Autoload]
		public static readonly StorePropertyDefinition LocationStreet = InternalSchema.LocationStreet;

		[Autoload]
		public static readonly StorePropertyDefinition LocationCity = InternalSchema.LocationCity;

		[Autoload]
		public static readonly StorePropertyDefinition LocationState = InternalSchema.LocationState;

		[Autoload]
		public static readonly StorePropertyDefinition LocationCountry = InternalSchema.LocationCountry;

		[Autoload]
		public static readonly StorePropertyDefinition LocationPostalCode = InternalSchema.LocationPostalCode;

		[Autoload]
		public static readonly StorePropertyDefinition LocationAddressInternal = InternalSchema.LocationAddressInternal;

		[Autoload]
		internal static readonly StorePropertyDefinition LidWhere = InternalSchema.LidWhere;

		[Autoload]
		public static readonly StorePropertyDefinition MapiIsAllDayEvent = InternalSchema.MapiIsAllDayEvent;

		[Autoload]
		public static readonly StorePropertyDefinition MeetingRequestWasSent = InternalSchema.MeetingRequestWasSent;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition Organizer = InternalSchema.From;

		[Autoload]
		public static readonly StorePropertyDefinition OrganizerDisplayName = InternalSchema.SentRepresentingDisplayName;

		[Autoload]
		public static readonly StorePropertyDefinition OrganizerEmailAddress = InternalSchema.SentRepresentingEmailAddress;

		[Autoload]
		public static readonly StorePropertyDefinition OrganizerEntryId = InternalSchema.SentRepresentingEntryId;

		[Autoload]
		public static readonly StorePropertyDefinition OrganizerType = InternalSchema.SentRepresentingType;

		[Autoload]
		public static readonly StorePropertyDefinition CalendarOriginatorId = InternalSchema.CalendarOriginatorId;

		[Autoload]
		public static readonly StorePropertyDefinition OwnerAppointmentID = InternalSchema.OwnerAppointmentID;

		[Autoload]
		public static readonly StorePropertyDefinition RecurrencePattern = InternalSchema.RecurrencePattern;

		[Autoload]
		public static readonly StorePropertyDefinition RecurrenceType = InternalSchema.CalculatedRecurrenceType;

		[Autoload]
		public static readonly StorePropertyDefinition ResponseState = InternalSchema.ResponseState;

		[Autoload]
		public static readonly StorePropertyDefinition ResponseType = InternalSchema.MapiResponseType;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition TimeZone = InternalSchema.TimeZone;

		[Autoload]
		public static readonly StorePropertyDefinition StartTimeZone = InternalSchema.StartTimeZone;

		[Autoload]
		public static readonly StorePropertyDefinition EndTimeZone = InternalSchema.EndTimeZone;

		public static readonly StorePropertyDefinition StartTimeZoneId = InternalSchema.StartTimeZoneId;

		public static readonly StorePropertyDefinition EndTimeZoneId = InternalSchema.EndTimeZoneId;

		[Autoload]
		public static readonly StorePropertyDefinition TimeZoneBlob = InternalSchema.TimeZoneBlob;

		[Autoload]
		public static readonly StorePropertyDefinition TimeZoneDefinitionEnd = InternalSchema.TimeZoneDefinitionEnd;

		[Autoload]
		public static readonly StorePropertyDefinition TimeZoneDefinitionRecurring = InternalSchema.TimeZoneDefinitionRecurring;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition When = InternalSchema.When;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentReplyTime = InternalSchema.AppointmentReplyTime;

		[Autoload]
		public static readonly StorePropertyDefinition IntendedFreeBusyStatus = InternalSchema.IntendedFreeBusyStatus;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentExtractVersion = InternalSchema.AppointmentExtractVersion;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentExtractTime = InternalSchema.AppointmentExtractTime;

		[Autoload]
		public static readonly StorePropertyDefinition IsOrganizer = InternalSchema.IsOrganizer;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentCounterProposalCount = InternalSchema.AppointmentCounterProposalCount;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentCounterProposal = InternalSchema.AppointmentCounterProposal;

		[Autoload]
		public static readonly StorePropertyDefinition ClipStartTime = InternalSchema.ClipStartTime;

		[Autoload]
		public static readonly StorePropertyDefinition ClipEndTime = InternalSchema.ClipEndTime;

		[Autoload]
		internal static readonly StorePropertyDefinition OriginalStoreEntryId = InternalSchema.OriginalStoreEntryId;

		[Autoload]
		internal static readonly StorePropertyDefinition LocationUrl = InternalSchema.LocationURL;

		[Autoload]
		internal static readonly StorePropertyDefinition Contact = InternalSchema.Contact;

		[Autoload]
		internal static readonly StorePropertyDefinition ContactUrl = InternalSchema.ContactURL;

		[Autoload]
		internal static readonly StorePropertyDefinition Keywords = InternalSchema.Keywords;

		[Autoload]
		public static readonly StorePropertyDefinition ClientIntent = InternalSchema.ClientIntent;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentAuxiliaryFlags = InternalSchema.AppointmentAuxiliaryFlags;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentReplyName = InternalSchema.AppointmentReplyName;

		internal static readonly StorePropertyDefinition AttachCalendarFlags = InternalSchema.AttachCalendarFlags;

		internal static readonly StorePropertyDefinition AttachCalendarHidden = InternalSchema.AttachCalendarHidden;

		internal static readonly StorePropertyDefinition AttachCalendarLinkId = InternalSchema.AttachCalendarLinkId;

		internal static readonly StorePropertyDefinition AttachEncoding = InternalSchema.AttachEncoding;

		[Autoload]
		internal static readonly StorePropertyDefinition AppointmentClass = InternalSchema.AppointmentClass;

		public static readonly StorePropertyDefinition AppointmentColor = InternalSchema.AppointmentColor;

		internal static readonly StorePropertyDefinition AppointmentExceptionEndTime = InternalSchema.AppointmentExceptionEndTime;

		internal static readonly StorePropertyDefinition AppointmentExceptionStartTime = InternalSchema.AppointmentExceptionStartTime;

		public static readonly StorePropertyDefinition ChangeList = InternalSchema.ChangeList;

		[Autoload]
		public static readonly StorePropertyDefinition CalendarLogTriggerAction = InternalSchema.CalendarLogTriggerAction;

		[Autoload]
		public static readonly StorePropertyDefinition ItemVersion = InternalSchema.ItemVersion;

		internal static readonly StorePropertyDefinition OutlookVersion = InternalSchema.OutlookVersion;

		internal static readonly StorePropertyDefinition OutlookInternalVersion = InternalSchema.OutlookInternalVersion;

		public static readonly StorePropertyDefinition CalendarItemType = InternalSchema.CalendarItemType;

		[Autoload]
		internal static readonly StorePropertyDefinition AcceptLanguage = InternalSchema.AcceptLanguage;

		[Autoload]
		public static readonly StorePropertyDefinition StartRecurTime = InternalSchema.StartRecurTime;

		[Autoload]
		public static readonly StorePropertyDefinition StartRecurDate = InternalSchema.StartRecurDate;

		[Autoload]
		internal static readonly StorePropertyDefinition DisplayAttendeesAll = InternalSchema.DisplayAttendeesAll;

		[Autoload]
		public static readonly StorePropertyDefinition DisplayAttendeesTo = InternalSchema.DisplayAttendeesTo;

		[Autoload]
		public static readonly StorePropertyDefinition DisplayAttendeesCc = InternalSchema.DisplayAttendeesCc;

		[LegalTracking]
		public static readonly StorePropertyDefinition BirthdayContactAttributionDisplayName = InternalSchema.BirthdayContactAttributionDisplayName;

		public static readonly StorePropertyDefinition BirthdayContactPersonId = InternalSchema.PersonId;

		public static readonly StorePropertyDefinition BirthdayContactId = InternalSchema.BirthdayContactId;

		[LegalTracking]
		public static readonly StorePropertyDefinition Birthday = InternalSchema.BirthdayLocal;

		public static readonly StorePropertyDefinition IsBirthdayContactWritable = InternalSchema.IsBirthdayContactWritable;

		public static readonly StorePropertyDefinition OriginalLastModifiedTime = InternalSchema.OriginalLastModifiedTime;

		public static readonly StorePropertyDefinition ResponsibleUserName = InternalSchema.ResponsibleUserName;

		public static readonly StorePropertyDefinition SenderEmailAddress = InternalSchema.SenderEmailAddress;

		public static readonly StorePropertyDefinition ClientInfoString = InternalSchema.ClientInfoString;

		public static readonly StorePropertyDefinition IsProcessed = InternalSchema.IsProcessed;

		public static readonly StorePropertyDefinition MiddleTierServerName = InternalSchema.MiddleTierServerName;

		public static readonly StorePropertyDefinition MiddleTierServerBuildVersion = InternalSchema.MiddleTierServerBuildVersion;

		public static readonly StorePropertyDefinition MailboxServerName = InternalSchema.MailboxServerName;

		public static readonly StorePropertyDefinition MiddleTierProcessName = InternalSchema.MiddleTierProcessName;

		[Autoload]
		public static readonly StorePropertyDefinition UCOpenedConferenceID = InternalSchema.UCOpenedConferenceID;

		[Autoload]
		public static readonly StorePropertyDefinition OnlineMeetingExternalLink = InternalSchema.OnlineMeetingExternalLink;

		[Autoload]
		public static readonly StorePropertyDefinition OnlineMeetingInternalLink = InternalSchema.OnlineMeetingInternalLink;

		[Autoload]
		public static readonly StorePropertyDefinition OnlineMeetingConfLink = InternalSchema.OnlineMeetingConfLink;

		[Autoload]
		public static readonly StorePropertyDefinition UCCapabilities = InternalSchema.UCCapabilities;

		[Autoload]
		public static readonly StorePropertyDefinition UCInband = InternalSchema.UCInband;

		[Autoload]
		public static readonly StorePropertyDefinition UCMeetingSetting = InternalSchema.UCMeetingSetting;

		[Autoload]
		public static readonly StorePropertyDefinition UCMeetingSettingSent = InternalSchema.UCMeetingSettingSent;

		[Autoload]
		public static readonly StorePropertyDefinition ConferenceTelURI = InternalSchema.ConferenceTelURI;

		[Autoload]
		public static readonly StorePropertyDefinition ConferenceInfo = InternalSchema.ConferenceInfo;

		public static readonly StorePropertyDefinition EventTimeBasedInboxReminders = InternalSchema.EventTimeBasedInboxReminders;

		public static readonly StorePropertyDefinition EventTimeBasedInboxRemindersState = InternalSchema.EventTimeBasedInboxRemindersState;

		[Autoload]
		public static readonly StorePropertyDefinition EventEmailReminderTimer = InternalSchema.EventEmailReminderTimer;

		[Autoload]
		public static readonly StorePropertyDefinition HasAttendees = InternalSchema.HasAttendees;

		[Autoload]
		public static readonly StorePropertyDefinition CharmId = InternalSchema.CharmId;

		private static readonly PropertyRule[] CalendarItemBasePropertyRules;

		private static CalendarItemBaseSchema instance;

		private ICollection<PropertyRule> propertyRulesCache;
	}
}
