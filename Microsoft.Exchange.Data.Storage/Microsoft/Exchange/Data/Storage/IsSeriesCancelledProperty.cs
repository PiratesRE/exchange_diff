using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class IsSeriesCancelledProperty : SmartPropertyDefinition
	{
		internal IsSeriesCancelledProperty() : base("IsSeriesCancelled", typeof(bool), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.AppointmentRecurring, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.AppointmentStateInternal, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			bool valueOrDefault = propertyBag.GetValueOrDefault<bool>(InternalSchema.AppointmentRecurring);
			if (valueOrDefault)
			{
				AppointmentStateFlags valueOrDefault2 = propertyBag.GetValueOrDefault<AppointmentStateFlags>(InternalSchema.AppointmentStateInternal);
				return CalendarItemBase.IsAppointmentStateCancelled(valueOrDefault2);
			}
			CalendarItemOccurrence calendarItemOccurrence = propertyBag.Context.StoreObject as CalendarItemOccurrence;
			if (calendarItemOccurrence != null)
			{
				AppointmentStateFlags valueOrDefault3 = calendarItemOccurrence.OccurrencePropertyBag.MasterCalendarItem.GetValueOrDefault<AppointmentStateFlags>(CalendarItemBaseSchema.AppointmentState);
				return CalendarItemBase.IsAppointmentStateCancelled(valueOrDefault3);
			}
			return new PropertyError(this, PropertyErrorCode.NotFound);
		}
	}
}
