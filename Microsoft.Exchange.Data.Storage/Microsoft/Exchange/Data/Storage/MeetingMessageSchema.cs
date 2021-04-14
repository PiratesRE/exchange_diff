using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MeetingMessageSchema : MessageItemSchema
	{
		public new static MeetingMessageSchema Instance
		{
			get
			{
				if (MeetingMessageSchema.instance == null)
				{
					MeetingMessageSchema.instance = new MeetingMessageSchema();
				}
				return MeetingMessageSchema.instance;
			}
		}

		protected MeetingMessageSchema()
		{
		}

		[Autoload]
		internal static readonly StorePropertyDefinition OnlineMeetingChanged = InternalSchema.OnlineMeetingChanged;

		[Autoload]
		public static readonly StorePropertyDefinition CalendarProcessed = InternalSchema.CalendarProcessed;

		public static readonly StorePropertyDefinition IsOutOfDate = InternalSchema.IsOutOfDate;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentAuxiliaryFlags = InternalSchema.AppointmentAuxiliaryFlags;

		[Autoload]
		public static readonly StorePropertyDefinition HijackedMeeting = InternalSchema.HijackedMeeting;

		[Autoload]
		public static readonly StorePropertyDefinition SeriesId = InternalSchema.SeriesId;

		[Autoload]
		public static readonly StorePropertyDefinition SeriesSequenceNumber = InternalSchema.SeriesSequenceNumber;

		[Autoload]
		internal new static readonly StorePropertyDefinition ReceivedRepresentingAddressType = InternalSchema.ReceivedRepresentingAddressType;

		[Autoload]
		internal new static readonly StorePropertyDefinition ReceivedRepresentingDisplayName = InternalSchema.ReceivedRepresentingDisplayName;

		[Autoload]
		internal new static readonly StorePropertyDefinition ReceivedRepresentingEmailAddress = InternalSchema.ReceivedRepresentingEmailAddress;

		[Autoload]
		internal new static readonly StorePropertyDefinition ReceivedRepresentingEntryId = InternalSchema.ReceivedRepresentingEntryId;

		[Autoload]
		internal new static readonly StorePropertyDefinition ReceivedByName = InternalSchema.ReceivedByName;

		[Autoload]
		internal new static readonly StorePropertyDefinition ReceivedByEmailAddress = InternalSchema.ReceivedByEmailAddress;

		[Autoload]
		internal new static readonly StorePropertyDefinition ReceivedByAddrType = InternalSchema.ReceivedByAddrType;

		[Autoload]
		internal new static readonly StorePropertyDefinition ReceivedByEntryId = InternalSchema.ReceivedByEntryId;

		[Autoload]
		internal static readonly StorePropertyDefinition ClipStartTime = InternalSchema.ClipStartTime;

		[Autoload]
		internal static readonly StorePropertyDefinition ClipEndTime = InternalSchema.ClipEndTime;

		[Autoload]
		internal static readonly StorePropertyDefinition Location = InternalSchema.Location;

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

		internal static readonly StorePropertyDefinition ChangeList = InternalSchema.ChangeList;

		[Autoload]
		public static readonly StorePropertyDefinition CalendarLogTriggerAction = InternalSchema.CalendarLogTriggerAction;

		[Autoload]
		internal static readonly StorePropertyDefinition ItemVersion = InternalSchema.ItemVersion;

		[Autoload]
		internal static readonly StorePropertyDefinition EHAMigrationExpirationDate = InternalSchema.EHAMigrationExpirationDate;

		[Autoload]
		internal static readonly StorePropertyDefinition SeriesReminderIsSet = InternalSchema.SeriesReminderIsSet;

		private static MeetingMessageSchema instance = null;
	}
}
