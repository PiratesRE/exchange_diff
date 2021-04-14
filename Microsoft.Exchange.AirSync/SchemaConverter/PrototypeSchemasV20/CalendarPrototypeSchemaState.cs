using System;
using Microsoft.Exchange.AirSync.SchemaConverter.AirSync;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.SchemaConverter.XSO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.PrototypeSchemasV20
{
	internal class CalendarPrototypeSchemaState : AirSyncXsoSchemaState
	{
		public CalendarPrototypeSchemaState() : base(CalendarPrototypeSchemaState.supportedClassFilter, new ExceptionPrototypeSchemaState())
		{
			base.InitConversionTable(2);
			this.CreatePropertyConversionTable();
		}

		internal static QueryFilter SupportedClassQueryFilter
		{
			get
			{
				return CalendarPrototypeSchemaState.supportedClassFilter;
			}
		}

		private void CreatePropertyConversionTable()
		{
			string xmlNodeNamespace = "Calendar:";
			AirSyncBodyProperty airSyncBodyProperty = new AirSyncBodyProperty(xmlNodeNamespace, "Body", "Rtf", "BodyTruncated", null, false, true, true, true);
			base.AddProperty(new IProperty[]
			{
				new AirSyncBinaryTimeZoneProperty(xmlNodeNamespace, "TimeZone", true),
				new XsoTimeZoneProperty(PropertyType.ReadAndRequiredForWrite)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncUtcDateTimeProperty(xmlNodeNamespace, "DtStamp", false),
				new XsoUtcDtStampProperty(PropertyType.ReadAndRequiredForWrite)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncUtcDateTimeProperty(xmlNodeNamespace, "StartTime", true),
				new XsoLocalDateTimeProperty(CalendarItemInstanceSchema.StartTime, null, PropertyType.ReadAndRequiredForWrite)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "Subject", true),
				new XsoStringProperty(ItemSchema.Subject)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "UID", false),
				new XsoUidProperty(CalendarItemBaseSchema.GlobalObjectId, PropertyType.ReadAndRequiredForWrite)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "OrganizerName", false),
				new XsoOrganizerNameProperty()
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "OrganizerEmail", false),
				new XsoOrganizerEmailProperty()
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncAttendeesProperty(xmlNodeNamespace, "Attendees", false),
				new XsoAttendeesProperty()
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "Location", true),
				new XsoStringProperty(CalendarItemBaseSchema.Location)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncUtcDateTimeProperty(xmlNodeNamespace, "EndTime", true),
				new XsoLocalDateTimeProperty(CalendarItemInstanceSchema.EndTime, CalendarItemBaseSchema.EndTimeZone)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncRecurrenceProperty(xmlNodeNamespace, "Recurrence", TypeOfRecurrence.Calendar, true, 20),
				new XsoRecurrenceProperty(TypeOfRecurrence.Calendar, 20)
			});
			base.AddProperty(new IProperty[]
			{
				airSyncBodyProperty,
				new XsoBodyProperty()
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncMultiValuedStringProperty(xmlNodeNamespace, "Categories", "Category", false),
				new XsoCategoriesProperty()
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncIntegerProperty(xmlNodeNamespace, "Sensitivity", false),
				new XsoSensitivityProperty(ItemSchema.Sensitivity, PropertyType.ReadAndRequiredForWrite)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncBusyStatusProperty(xmlNodeNamespace, "BusyStatus", false),
				new XsoBusyStatusProperty(BusyType.Free, PropertyType.ReadAndRequiredForWrite)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncBooleanProperty(xmlNodeNamespace, "AllDayEvent", true),
				new XsoBooleanProperty(CalendarItemBaseSchema.MapiIsAllDayEvent)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncReminderProperty(xmlNodeNamespace, "Reminder", true),
				new XsoReminderOffsetProperty()
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncExceptionsProperty(xmlNodeNamespace, "Exceptions", true),
				new XsoExceptionsProperty()
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncRtfBodyProperty(xmlNodeNamespace, "Rtf", false, airSyncBodyProperty),
				new XsoBodyProperty()
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncIntegerProperty(xmlNodeNamespace, "MeetingStatus", false),
				new XsoMeetingStatusProperty(CalendarItemBaseSchema.AppointmentState)
			});
		}

		private static readonly string[] supportedClassTypes = new string[]
		{
			"IPM.APPOINTMENT"
		};

		private static readonly QueryFilter supportedClassFilter = AirSyncXsoSchemaState.BuildMessageClassFilter(CalendarPrototypeSchemaState.supportedClassTypes);
	}
}
