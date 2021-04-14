using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class IsOrganizerProperty : SmartPropertyDefinition
	{
		internal IsOrganizerProperty() : base("IsOrganizerProperty", typeof(bool), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.AppointmentStateInternal, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ItemClass, PropertyDependencyType.NeedForRead)
		})
		{
		}

		internal static bool GetForCalendarItem(string messageClass, AppointmentStateFlags flags)
		{
			if (!ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(messageClass) && !ObjectClass.IsCalendarItemSeries(messageClass))
			{
				throw new ArgumentException(string.Format("[IsOrganizerProperty.GetForCalendarItem] Message class MUST be a calendar item occurrence or recurrence exception in order to call this method.  ItemClass: {0}", messageClass));
			}
			return (flags & AppointmentStateFlags.Received) == AppointmentStateFlags.None;
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			bool? flag = null;
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
			if (ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(valueOrDefault) || ObjectClass.IsCalendarItemSeries(valueOrDefault))
			{
				AppointmentStateFlags valueOrDefault2 = propertyBag.GetValueOrDefault<AppointmentStateFlags>(InternalSchema.AppointmentStateInternal);
				flag = new bool?(IsOrganizerProperty.GetForCalendarItem(valueOrDefault, valueOrDefault2));
			}
			else if (ObjectClass.IsMeetingMessage(valueOrDefault))
			{
				MeetingMessage meetingMessage = propertyBag.Context.StoreObject as MeetingMessage;
				if (meetingMessage != null)
				{
					CalendarItemBase calendarItemBase = null;
					try
					{
						calendarItemBase = meetingMessage.GetCorrelatedItemInternal(true);
					}
					catch (CorruptDataException)
					{
					}
					catch (CorrelationFailedException)
					{
					}
					if (calendarItemBase != null)
					{
						flag = new bool?(calendarItemBase.IsOrganizer());
					}
					else if (!(meetingMessage is MeetingResponse))
					{
						flag = new bool?(meetingMessage.IsMailboxOwnerTheSender());
					}
				}
			}
			if (flag != null)
			{
				return flag;
			}
			return new PropertyError(this, PropertyErrorCode.NotFound);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			throw new NotSupportedException(ServerStrings.PropertyIsReadOnly("IsOrganizer"));
		}
	}
}
