using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ReminderDueByProperty : SmartPropertyDefinition
	{
		internal ReminderDueByProperty() : base("ReminderDueByProperty", typeof(ExDateTime), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.ItemClass, PropertyDependencyType.NeedToReadForWrite),
			new PropertyDependency(InternalSchema.ReminderDueByInternal, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.EntryId, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.AppointmentRecurrenceBlob, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.TimeZoneBlob, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.TimeZoneDefinitionRecurring, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ReminderIsSetInternal, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ReminderMinutesBeforeStartInternal, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ReminderNextTime, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.TimeZone, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			StoreSession session = propertyBag.Context.Session;
			if (session == null)
			{
				ExTraceGlobals.StorageTracer.TraceDebug((long)propertyBag.GetHashCode(), "ReminderDueByProperty: StoreObject is not present and the property bag is not a QueryResultsPropertyBag. First should be present when accessing a property on an object, and the second one - when called from a view. All other cases are wrong.");
				return propertyBag.GetValue(InternalSchema.ReminderDueByInternal);
			}
			PropertyError result = new PropertyError(InternalSchema.ReminderDueBy, PropertyErrorCode.GetCalculatedPropertyError);
			CalendarItem calendarItem = propertyBag.Context.StoreObject as CalendarItem;
			InternalRecurrence internalRecurrence = null;
			if (calendarItem != null)
			{
				try
				{
					internalRecurrence = (InternalRecurrence)calendarItem.Recurrence;
					goto IL_206;
				}
				catch (RecurrenceFormatException ex)
				{
					ExTraceGlobals.StorageTracer.TraceDebug<string, StoreObjectId>((long)session.GetHashCode(), "Unable to parse recurrence info. Using the stored value for DueBy.\r\n\tError: {0}\r\n\tID:{1}", ex.Message, (calendarItem.Id != null) ? calendarItem.Id.ObjectId : null);
					return result;
				}
				catch (CorruptDataException ex2)
				{
					ExTraceGlobals.StorageTracer.TraceDebug<string, StoreObjectId>((long)session.GetHashCode(), "Unable to parse the organizer timezone. Using the stored value for DueBy.\r\n\tError: {0}\r\n\tID:{1}", ex2.Message, (calendarItem.Id != null) ? calendarItem.Id.ObjectId : null);
					return result;
				}
			}
			byte[] entryId;
			if (Util.TryConvertTo<byte[]>(propertyBag.GetValue(InternalSchema.EntryId), out entryId))
			{
				VersionedId versionedId = new VersionedId(StoreObjectId.FromProviderSpecificId(entryId), Array<byte>.Empty);
				byte[] array = propertyBag.GetValue(InternalSchema.AppointmentRecurrenceBlob) as byte[];
				if (array != null)
				{
					string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.TimeZone, string.Empty);
					byte[] valueOrDefault2 = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.TimeZoneBlob);
					byte[] valueOrDefault3 = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.TimeZoneDefinitionRecurring);
					ExTimeZone timeZoneFromProperties = TimeZoneHelper.GetTimeZoneFromProperties(valueOrDefault, valueOrDefault2, valueOrDefault3);
					if (timeZoneFromProperties == null)
					{
						ExTraceGlobals.StorageTracer.TraceDebug<string, StoreObjectId>((long)session.GetHashCode(), "Unable to parse the organizer timezone. Using the stored value for DueBy.\r\n\tError: {0}\r\n\tID:{1}", "organizerTimeZone is null", versionedId.ObjectId);
						return result;
					}
					try
					{
						internalRecurrence = InternalRecurrence.GetRecurrence(versionedId, session, array, timeZoneFromProperties, CalendarItem.DefaultCodePage);
					}
					catch (RecurrenceFormatException ex3)
					{
						ExTraceGlobals.StorageTracer.TraceDebug<string, StoreObjectId>((long)session.GetHashCode(), "Unable to parse recurrence info. Using the stored value for DueBy.\r\n\tError: {0}\r\n\tID:{1}", ex3.Message, versionedId.ObjectId);
						return result;
					}
					catch (CorruptDataException ex4)
					{
						ExTraceGlobals.StorageTracer.TraceDebug<string, StoreObjectId>((long)session.GetHashCode(), "Unable to parse the organizer timezone. Using the stored value for DueBy.\r\n\tError: {0}\r\n\tID:{1}", ex4.Message, versionedId.ObjectId);
						return result;
					}
				}
			}
			IL_206:
			bool defaultIsSet;
			int defaultMinutesBeforeStart;
			ExDateTime value;
			if (internalRecurrence == null || !Util.TryConvertTo<bool>(propertyBag.GetValue(InternalSchema.ReminderIsSetInternal), out defaultIsSet) || !Util.TryConvertTo<int>(propertyBag.GetValue(InternalSchema.ReminderMinutesBeforeStartInternal), out defaultMinutesBeforeStart) || !Util.TryConvertTo<ExDateTime>(propertyBag.GetValue(InternalSchema.ReminderNextTime), out value))
			{
				return propertyBag.GetValue(InternalSchema.ReminderDueByInternal);
			}
			ExDateTime probeTime = Reminder.GetProbeTime(Reminder.GetTimeNow(session.ExTimeZone), new ExDateTime?(value));
			OccurrenceInfo mostRecentOccurrence;
			try
			{
				mostRecentOccurrence = CalendarItem.CustomReminder.GetMostRecentOccurrence(internalRecurrence, probeTime, defaultIsSet, defaultMinutesBeforeStart);
			}
			catch (CorruptDataException)
			{
				return result;
			}
			if (mostRecentOccurrence == null)
			{
				return new PropertyError(this, PropertyErrorCode.NotFound);
			}
			return mostRecentOccurrence.StartTime;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			CalendarItemBase calendarItemBase = propertyBag.Context.StoreObject as CalendarItemBase;
			if (calendarItemBase != null)
			{
				calendarItemBase.LocationIdentifierHelperInstance.SetLocationIdentifier(42869U, LastChangeAction.ModifyReminder);
			}
			propertyBag.SetValueWithFixup(InternalSchema.ReminderDueByInternal, value);
			if (ObjectClass.IsMessage(propertyBag.GetValue(InternalSchema.ItemClass) as string, false))
			{
				propertyBag.SetValueWithFixup(InternalSchema.ReplyTime, (ExDateTime)value);
			}
			Reminder.Adjust(propertyBag.Context.StoreObject);
		}
	}
}
