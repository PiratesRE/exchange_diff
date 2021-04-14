using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.AirSync;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.SchemaConverter.Entity;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.PrototypeSchemasV160
{
	internal class CalendarPrototypeSchemaState : AirSyncEntitySchemaState
	{
		public CalendarPrototypeSchemaState() : base(CalendarPrototypeSchemaState.supportedClassFilter)
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
			string xmlNodeNamespace2 = "AirSyncBase:";
			EventSchema schemaInstance = SchematizedObject<EventSchema>.SchemaInstance;
			IProperty[] array = new IProperty[2];
			array[0] = new AirSyncExDateTimeProperty(xmlNodeNamespace, "DtStamp", true);
			array[1] = new EntityExDateTimeProperty(new EntityPropertyDefinition(schemaInstance.LastModifiedTimeProperty, (IItem item) => ((Event)item).LastModifiedTime), PropertyType.ReadOnly, false);
			base.AddProperty(array);
			IProperty[] array2 = new IProperty[2];
			array2[0] = new AirSyncExDateTimeProperty(xmlNodeNamespace, "StartTime", true);
			array2[1] = new EntityExDateTimeProperty(new EntityPropertyDefinition(schemaInstance.StartProperty, (IItem item) => ((Event)item).Start, (IItem item, object value) => ((Event)item).Start = (ExDateTime)value), true);
			base.AddProperty(array2);
			IProperty[] array3 = new IProperty[2];
			array3[0] = new AirSyncStringProperty(xmlNodeNamespace, "Subject", true);
			array3[1] = new EntityStringProperty(new EntityPropertyDefinition(schemaInstance.SubjectProperty, (IItem item) => item.Subject, (IItem item, object value) => item.Subject = (string)value), false);
			base.AddProperty(array3);
			IProperty[] array4 = new IProperty[2];
			array4[0] = new AirSyncStringProperty(xmlNodeNamespace, "UID", true);
			array4[1] = new EntityStringProperty(new EntityPropertyDefinition(schemaInstance.IdProperty, (IItem item) => item.Id), PropertyType.ReadOnly, true);
			base.AddProperty(array4);
			IProperty[] array5 = new IProperty[2];
			array5[0] = new AirSyncStringProperty(xmlNodeNamespace, "SeriesMasterId", true);
			array5[1] = new EntityStringProperty(new EntityPropertyDefinition(schemaInstance.SeriesMasterIdProperty, (IItem item) => ((Event)item).SeriesMasterId), PropertyType.ReadOnly, true);
			base.AddProperty(array5);
			IProperty[] array6 = new IProperty[2];
			array6[0] = new AirSyncStringProperty(xmlNodeNamespace, "OrganizerName", false);
			array6[1] = new EntityStringProperty(new EntityPropertyDefinition(schemaInstance.OrganizerProperty, (IItem item) => ((Event)item).Organizer.Name), PropertyType.ReadOnly, false);
			base.AddProperty(array6);
			IProperty[] array7 = new IProperty[2];
			array7[0] = new AirSyncStringProperty(xmlNodeNamespace, "OrganizerEmail", false);
			array7[1] = new EntityStringProperty(new EntityPropertyDefinition(schemaInstance.OrganizerProperty, (IItem item) => ((Event)item).Organizer.EmailAddress), PropertyType.ReadOnly, false);
			base.AddProperty(array7);
			base.AddProperty(new IProperty[]
			{
				new AirSyncExtendedAttendeesProperty(xmlNodeNamespace, "Attendees", false),
				new EntityAttendeesProperty()
			});
			IProperty[] array8 = new IProperty[2];
			array8[0] = new AirSyncEnhancedLocationProperty(xmlNodeNamespace2, "Location", true, 160);
			array8[1] = new EntityEnhancedLocationProperty(160, new EntityPropertyDefinition(schemaInstance.LocationProperty, (IItem item) => ((Event)item).Location, (IItem item, object value) => ((Event)item).Location = (Location)value), PropertyType.ReadWrite);
			base.AddProperty(array8);
			IProperty[] array9 = new IProperty[2];
			array9[0] = new AirSyncExDateTimeProperty(xmlNodeNamespace, "EndTime", true);
			array9[1] = new EntityExDateTimeProperty(new EntityPropertyDefinition(schemaInstance.EndProperty, (IItem item) => ((Event)item).End, (IItem item, object value) => ((Event)item).End = (ExDateTime)value), true);
			base.AddProperty(array9);
			IProperty[] array10 = new IProperty[2];
			array10[0] = new AirSyncRecurrenceProperty(xmlNodeNamespace, "Recurrence", TypeOfRecurrence.Calendar, true, 160);
			array10[1] = new EntityRecurrenceProperty(TypeOfRecurrence.Calendar, 160, new EntityPropertyDefinition(schemaInstance.PatternedRecurrenceProperty, (IItem item) => ((Event)item).PatternedRecurrence, (IItem item, object value) => ((Event)item).PatternedRecurrence = (PatternedRecurrence)value), PropertyType.ReadWrite);
			base.AddProperty(array10);
			base.AddProperty(new IProperty[]
			{
				new AirSyncContent16Property(xmlNodeNamespace2, "Body", false),
				new EntityContentProperty()
			});
			IProperty[] array11 = new IProperty[2];
			array11[0] = new AirSyncMultiValuedStringProperty(xmlNodeNamespace, "Categories", "Category", true);
			array11[1] = new EntityMultiValuedStringProperty(new EntityPropertyDefinition(schemaInstance.CategoriesProperty, (IItem item) => item.Categories, (IItem item, object value) => item.Categories = (List<string>)value));
			base.AddProperty(array11);
			IProperty[] array12 = new IProperty[2];
			array12[0] = new AirSyncIntegerProperty(xmlNodeNamespace, "Sensitivity", true);
			array12[1] = new EntityEnumProperty(new EntityPropertyDefinition(schemaInstance.SensitivityProperty, (IItem item) => item.Sensitivity, (IItem item, object value) => item.Sensitivity = (Sensitivity)value), false);
			base.AddProperty(array12);
			IProperty[] array13 = new IProperty[2];
			array13[0] = new AirSyncIntegerProperty(xmlNodeNamespace, "BusyStatus", true);
			array13[1] = new EntityEnumProperty(new EntityPropertyDefinition(schemaInstance.ShowAsProperty, (IItem item) => ((Event)item).ShowAs, (IItem item, object value) => ((Event)item).ShowAs = (FreeBusyStatus)value), false);
			base.AddProperty(array13);
			IProperty[] array14 = new IProperty[2];
			array14[0] = new AirSyncBooleanProperty(xmlNodeNamespace, "AllDayEvent", true);
			array14[1] = new EntityBooleanProperty(new EntityPropertyDefinition(schemaInstance.IsAllDayProperty, (IItem item) => ((Event)item).IsAllDay, (IItem item, object value) => ((Event)item).IsAllDay = (bool)value));
			base.AddProperty(array14);
			base.AddProperty(new IProperty[]
			{
				new AirSyncReminder160Property(xmlNodeNamespace, "Reminder", true),
				new EntityReminderOffsetProperty()
			});
			IProperty[] array15 = new IProperty[2];
			array15[0] = new AirSyncIntegerProperty(xmlNodeNamespace, "EventType", false);
			array15[1] = new EntityEnumProperty(new EntityPropertyDefinition(schemaInstance.TypeProperty, (IItem item) => ((Event)item).Type), PropertyType.ReadOnly, true);
			base.AddProperty(array15);
			base.AddProperty(new IProperty[]
			{
				new AirSyncIntegerProperty(xmlNodeNamespace, "MeetingStatus", false),
				new EntityMeetingStatusProperty()
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncIntegerProperty(xmlNodeNamespace2, "NativeBodyType", false),
				new EntityNativeBodyProperty()
			});
			IProperty[] array16 = new IProperty[2];
			array16[0] = new AirSyncBooleanProperty(xmlNodeNamespace, "DisallowNewTimeProposal", false);
			array16[1] = new EntityBooleanProperty(new EntityPropertyDefinition(schemaInstance.DisallowNewTimeProposalProperty, (IItem item) => ((Event)item).DisallowNewTimeProposal, (IItem item, object value) => ((Event)item).DisallowNewTimeProposal = (bool)value));
			base.AddProperty(array16);
			IProperty[] array17 = new IProperty[2];
			array17[0] = new AirSyncBooleanProperty(xmlNodeNamespace, "ResponseRequested", false);
			array17[1] = new EntityBooleanProperty(new EntityPropertyDefinition(schemaInstance.ResponseRequestedProperty, (IItem item) => ((Event)item).ResponseRequested, (IItem item, object value) => ((Event)item).ResponseRequested = (bool)value));
			base.AddProperty(array17);
			IProperty[] array18 = new IProperty[2];
			array18[0] = new AirSyncExDateTimeProperty(xmlNodeNamespace, "AppointmentReplyTime", false);
			array18[1] = new EntityExDateTimeProperty(new EntityPropertyDefinition(schemaInstance.ResponseStatusProperty, (IItem item) => ((Event)item).ResponseStatus.Time), PropertyType.ReadOnly, false);
			base.AddProperty(array18);
			IProperty[] array19 = new IProperty[2];
			array19[0] = new AirSyncIntegerProperty(xmlNodeNamespace, "ResponseType", false);
			array19[1] = new EntityEnumProperty(new EntityPropertyDefinition(schemaInstance.ResponseStatusProperty, (IItem item) => ((Event)item).ResponseStatus.Response), PropertyType.ReadOnly, false);
			base.AddProperty(array19);
			IProperty[] array20 = new IProperty[2];
			array20[0] = new AirSyncStringProperty(xmlNodeNamespace, "OnlineMeetingConfLink", false);
			array20[1] = new EntityStringProperty(new EntityPropertyDefinition(schemaInstance.OnlineMeetingConfLinkProperty, (IItem item) => ((Event)item).OnlineMeetingConfLink), PropertyType.ReadOnly, false);
			base.AddProperty(array20);
			IProperty[] array21 = new IProperty[2];
			array21[0] = new AirSyncStringProperty(xmlNodeNamespace, "OnlineMeetingExternalLink", false);
			array21[1] = new EntityStringProperty(new EntityPropertyDefinition(schemaInstance.OnlineMeetingExternalLinkProperty, (IItem item) => ((Event)item).OnlineMeetingExternalLink), PropertyType.ReadOnly, false);
			base.AddProperty(array21);
			base.AddProperty(new IProperty[]
			{
				new AirSync16AttachmentsProperty(xmlNodeNamespace2, "Attachments", false),
				new EntityAttachmentsProperty()
			});
		}

		private static readonly string[] supportedClassTypes = new string[]
		{
			"IPM.APPOINTMENT"
		};

		private static readonly QueryFilter supportedClassFilter = AirSyncXsoSchemaState.BuildMessageClassFilter(CalendarPrototypeSchemaState.supportedClassTypes);
	}
}
