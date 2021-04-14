using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class CalendarItemTypeProperty : SmartPropertyDefinition
	{
		internal CalendarItemTypeProperty() : base("CalendarItemType", typeof(CalendarItemType), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.AppointmentRecurring, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.IsException, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ItemClass, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
			if (ObjectClass.IsOfClass(valueOrDefault, "IPM.Appointment.Occurrence") || ObjectClass.IsOfClass(valueOrDefault, "IPM.OLE.CLASS.{00061055-0000-0000-C000-000000000046}"))
			{
				bool valueOrDefault2 = propertyBag.GetValueOrDefault<bool>(InternalSchema.IsException);
				if (valueOrDefault2)
				{
					return CalendarItemType.Exception;
				}
				return CalendarItemType.Occurrence;
			}
			else
			{
				bool valueOrDefault3 = propertyBag.GetValueOrDefault<bool>(InternalSchema.AppointmentRecurring);
				if (valueOrDefault3)
				{
					return CalendarItemType.RecurringMaster;
				}
				return CalendarItemType.Single;
			}
		}
	}
}
