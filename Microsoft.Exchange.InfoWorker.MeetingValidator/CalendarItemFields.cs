using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CalendarItemFields
	{
		internal static GlobalObjectId GetGlobalObjectId(CalendarItemType remoteItem)
		{
			if (remoteItem != null && remoteItem.ExtendedProperty != null)
			{
				foreach (ExtendedPropertyType extendedPropertyType in remoteItem.ExtendedProperty)
				{
					if (extendedPropertyType.ExtendedFieldURI != null && extendedPropertyType.Item != null && string.Compare(extendedPropertyType.ExtendedFieldURI.PropertySetId, CalendarItemFields.PSETIDMeeting.ToString(), StringComparison.OrdinalIgnoreCase) == 0 && extendedPropertyType.ExtendedFieldURI.PropertyId == CalendarItemFields.GlobalObjectIdProp.PropertyId && extendedPropertyType.ExtendedFieldURI.PropertyType == CalendarItemFields.GlobalObjectIdProp.PropertyType)
					{
						return new GlobalObjectId(Convert.FromBase64String((string)extendedPropertyType.Item));
					}
				}
			}
			return null;
		}

		private const int CleanGlobalObjectIdPropertyId = 35;

		private const int GlobalObjectIdPropertyId = 3;

		private const int AppointmentExtractTimePropertyId = 33325;

		private const int AppointmentExtractVersionPropertyId = 33324;

		private const int AppointmentRecurrenceBlobPropertyId = 33302;

		private const int TimeZoneBlobPropertyId = 33331;

		private const int TimeZoneDefinitionStartPropertyId = 33374;

		private const int TimeZoneDefinitionEndPropertyId = 33375;

		private const int TimeZoneDefinitionRecurringPropertyId = 33376;

		private const int OwnerCriticalChangeTimePropertyId = 26;

		private const int AttendeeCriticalChangeTimePropertyId = 1;

		private const int ItemVersionPropertyId = 22;

		private const int AppointmentRecurringPropertyId = 33315;

		private const int IsExceptionPropertyId = 10;

		private const string OwnerAppointmentIDPropertyTag = "0x0062";

		private const string CreationTimePropertyTag = "0x3007";

		private const string ItemClassPropertyTag = "0x001A";

		internal static readonly Guid PSETIDAppointment = new Guid("{00062002-0000-0000-C000-000000000046}");

		internal static readonly Guid PSETIDMeeting = new Guid("{6ED8DA90-450B-101B-98DA-00AA003F1305}");

		internal static readonly Guid PSETIDCalendarAssistant = new Guid("{11000E07-B51B-40D6-AF21-CAA85EDAB1D0}");

		internal static readonly PathToExtendedFieldType CleanGlobalObjectIdProp = new PathToExtendedFieldType
		{
			PropertySetId = CalendarItemFields.PSETIDMeeting.ToString(),
			PropertyIdSpecified = true,
			PropertyId = 35,
			PropertyType = MapiPropertyTypeType.Binary
		};

		internal static readonly PathToExtendedFieldType GlobalObjectIdProp = new PathToExtendedFieldType
		{
			PropertySetId = CalendarItemFields.PSETIDMeeting.ToString(),
			PropertyIdSpecified = true,
			PropertyId = 3,
			PropertyType = MapiPropertyTypeType.Binary
		};

		internal static readonly PathToExtendedFieldType AppointmentExtractTimeProp = new PathToExtendedFieldType
		{
			PropertySetId = CalendarItemFields.PSETIDAppointment.ToString(),
			PropertyIdSpecified = true,
			PropertyId = 33325,
			PropertyType = MapiPropertyTypeType.SystemTime
		};

		internal static readonly PathToExtendedFieldType AppointmentExtractVersionProp = new PathToExtendedFieldType
		{
			PropertySetId = CalendarItemFields.PSETIDAppointment.ToString(),
			PropertyIdSpecified = true,
			PropertyId = 33324,
			PropertyType = MapiPropertyTypeType.Long
		};

		internal static readonly PathToExtendedFieldType AppointmentRecurrenceBlobProp = new PathToExtendedFieldType
		{
			PropertySetId = CalendarItemFields.PSETIDAppointment.ToString(),
			PropertyIdSpecified = true,
			PropertyId = 33302,
			PropertyType = MapiPropertyTypeType.Binary
		};

		internal static readonly PathToExtendedFieldType TimeZoneBlobProp = new PathToExtendedFieldType
		{
			PropertySetId = CalendarItemFields.PSETIDAppointment.ToString(),
			PropertyIdSpecified = true,
			PropertyId = 33331,
			PropertyType = MapiPropertyTypeType.Binary
		};

		internal static readonly PathToExtendedFieldType TimeZoneDefinitionStartProp = new PathToExtendedFieldType
		{
			PropertySetId = CalendarItemFields.PSETIDAppointment.ToString(),
			PropertyIdSpecified = true,
			PropertyId = 33374,
			PropertyType = MapiPropertyTypeType.Binary
		};

		internal static readonly PathToExtendedFieldType TimeZoneDefinitionEndProp = new PathToExtendedFieldType
		{
			PropertySetId = CalendarItemFields.PSETIDAppointment.ToString(),
			PropertyIdSpecified = true,
			PropertyId = 33375,
			PropertyType = MapiPropertyTypeType.Binary
		};

		internal static readonly PathToExtendedFieldType TimeZoneDefinitionRecurringProp = new PathToExtendedFieldType
		{
			PropertySetId = CalendarItemFields.PSETIDAppointment.ToString(),
			PropertyIdSpecified = true,
			PropertyId = 33376,
			PropertyType = MapiPropertyTypeType.Binary
		};

		internal static readonly PathToExtendedFieldType OwnerCriticalChangeTimeProp = new PathToExtendedFieldType
		{
			PropertySetId = CalendarItemFields.PSETIDMeeting.ToString(),
			PropertyIdSpecified = true,
			PropertyId = 26,
			PropertyType = MapiPropertyTypeType.SystemTime
		};

		internal static readonly PathToExtendedFieldType AttendeeCriticalChangeTimeProp = new PathToExtendedFieldType
		{
			PropertySetId = CalendarItemFields.PSETIDMeeting.ToString(),
			PropertyIdSpecified = true,
			PropertyId = 1,
			PropertyType = MapiPropertyTypeType.SystemTime
		};

		internal static readonly PathToExtendedFieldType ItemVersionProp = new PathToExtendedFieldType
		{
			PropertySetId = CalendarItemFields.PSETIDCalendarAssistant.ToString(),
			PropertyIdSpecified = true,
			PropertyId = 22,
			PropertyType = MapiPropertyTypeType.Integer
		};

		internal static readonly PathToExtendedFieldType OwnerAppointmentIDProp = new PathToExtendedFieldType
		{
			PropertyTag = "0x0062",
			PropertyType = MapiPropertyTypeType.Integer
		};

		internal static readonly PathToExtendedFieldType CreationTimeProp = new PathToExtendedFieldType
		{
			PropertyTag = "0x3007",
			PropertyType = MapiPropertyTypeType.SystemTime
		};

		internal static readonly PathToExtendedFieldType ItemClassProp = new PathToExtendedFieldType
		{
			PropertyTag = "0x001A",
			PropertyType = MapiPropertyTypeType.String
		};

		internal static readonly PathToExtendedFieldType AppointmentRecurringProp = new PathToExtendedFieldType
		{
			PropertySetId = CalendarItemFields.PSETIDAppointment.ToString(),
			PropertyIdSpecified = true,
			PropertyId = 33315,
			PropertyType = MapiPropertyTypeType.Boolean
		};

		internal static readonly PathToExtendedFieldType IsExceptionProp = new PathToExtendedFieldType
		{
			PropertySetId = CalendarItemFields.PSETIDMeeting.ToString(),
			PropertyIdSpecified = true,
			PropertyId = 10,
			PropertyType = MapiPropertyTypeType.Boolean
		};

		internal static readonly ItemResponseShapeType CalendarQueryShape = new ItemResponseShapeType
		{
			BaseShape = DefaultShapeNamesType.IdOnly,
			IncludeMimeContent = false,
			IncludeMimeContentSpecified = false,
			BodyType = BodyTypeResponseType.HTML,
			BodyTypeSpecified = true,
			AdditionalProperties = new BasePathToElementType[]
			{
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarCalendarItemType
				}
			}
		};

		internal static readonly ItemResponseShapeType CalendarItemShape = new ItemResponseShapeType
		{
			BaseShape = DefaultShapeNamesType.IdOnly,
			IncludeMimeContent = false,
			IncludeMimeContentSpecified = false,
			BodyType = BodyTypeResponseType.HTML,
			BodyTypeSpecified = true,
			AdditionalProperties = new BasePathToElementType[]
			{
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.itemItemId
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.itemSubject
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarStart
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarEnd
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarLocation
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarCalendarItemType
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarMyResponseType
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarIsResponseRequested
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarAppointmentReplyTime
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarAppointmentState
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarAppointmentSequenceNumber
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarTimeZone
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarStartTimeZone
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarEndTimeZone
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarRequiredAttendees
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarOptionalAttendees
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.itemLastModifiedTime
				},
				CalendarItemFields.CleanGlobalObjectIdProp,
				CalendarItemFields.GlobalObjectIdProp,
				CalendarItemFields.AppointmentExtractTimeProp,
				CalendarItemFields.AppointmentExtractVersionProp,
				CalendarItemFields.AppointmentRecurrenceBlobProp,
				CalendarItemFields.TimeZoneBlobProp,
				CalendarItemFields.TimeZoneDefinitionStartProp,
				CalendarItemFields.TimeZoneDefinitionEndProp,
				CalendarItemFields.TimeZoneDefinitionRecurringProp,
				CalendarItemFields.OwnerCriticalChangeTimeProp,
				CalendarItemFields.AttendeeCriticalChangeTimeProp,
				CalendarItemFields.ItemVersionProp,
				CalendarItemFields.OwnerAppointmentIDProp,
				CalendarItemFields.CreationTimeProp,
				CalendarItemFields.ItemClassProp,
				CalendarItemFields.AppointmentRecurringProp,
				CalendarItemFields.IsExceptionProp
			}
		};
	}
}
