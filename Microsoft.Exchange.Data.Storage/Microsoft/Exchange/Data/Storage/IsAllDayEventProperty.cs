using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class IsAllDayEventProperty : SmartPropertyDefinition
	{
		internal IsAllDayEventProperty() : base("IsAllDayEvent", typeof(bool), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.MapiIsAllDayEvent, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.MapiStartTime, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.MapiEndTime, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.MapiPRStartDate, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.MapiPREndDate, PropertyDependencyType.AllRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			CalendarItemBase calendarItemBase = propertyBag.Context.StoreObject as CalendarItemBase;
			if (calendarItemBase != null && calendarItemBase.IsAllDayEventCache != null)
			{
				return calendarItemBase.IsAllDayEventCache.Value;
			}
			return IsAllDayEventProperty.CalculateIsAllDayEvent(propertyBag);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			if (!(value is bool))
			{
				string message = ServerStrings.ObjectMustBeOfType("bool");
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), message);
				throw new ArgumentException(message);
			}
			CalendarItemBase calendarItemBase = propertyBag.Context.StoreObject as CalendarItemBase;
			if (calendarItemBase != null && calendarItemBase.PropertyBag.ExTimeZone != null)
			{
				calendarItemBase.IsAllDayEventCache = new bool?((bool)value);
			}
			propertyBag.SetValueWithFixup(InternalSchema.MapiIsAllDayEvent, value);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return base.SinglePropertySmartFilterToNativeFilter(filter, InternalSchema.MapiIsAllDayEvent);
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.CanQuery;
			}
		}

		internal static object CalculateIsAllDayEvent(PropertyBag.BasicPropertyStore propertyBag)
		{
			object value = propertyBag.GetValue(InternalSchema.MapiIsAllDayEvent);
			if (propertyBag.TimeZone == null)
			{
				return value;
			}
			object value2 = propertyBag.GetValue(InternalSchema.MapiStartTime);
			object value3 = propertyBag.GetValue(InternalSchema.MapiEndTime);
			PropertyError propertyError = value2 as PropertyError;
			if (propertyError != null && propertyError.PropertyErrorCode == PropertyErrorCode.NotFound)
			{
				value2 = propertyBag.GetValue(InternalSchema.MapiPRStartDate);
			}
			propertyError = (value3 as PropertyError);
			if (propertyError != null && propertyError.PropertyErrorCode == PropertyErrorCode.NotFound)
			{
				value3 = propertyBag.GetValue(InternalSchema.MapiPREndDate);
			}
			if (!(value is bool) || !(value2 is ExDateTime) || !(value3 is ExDateTime))
			{
				return value;
			}
			ExDateTime exDateTime = (ExDateTime)value2;
			ExDateTime exDateTime2 = (ExDateTime)value3;
			if ((bool)value && exDateTime == exDateTime.Date && exDateTime2 == exDateTime2.Date && exDateTime2 > exDateTime)
			{
				return true;
			}
			return false;
		}
	}
}
