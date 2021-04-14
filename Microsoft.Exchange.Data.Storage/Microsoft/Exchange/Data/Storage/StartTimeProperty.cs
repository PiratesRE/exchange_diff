using System;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class StartTimeProperty : SmartPropertyDefinition
	{
		internal StartTimeProperty() : base("StartTime", typeof(ExDateTime), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.MapiStartTime, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.MapiPRStartDate, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.TimeZoneDefinitionStart, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.MapiEndTime, PropertyDependencyType.NeedToReadForWrite),
			new PropertyDependency(InternalSchema.MapiPREndDate, PropertyDependencyType.NeedToReadForWrite),
			new PropertyDependency(InternalSchema.ReminderNextTime, PropertyDependencyType.NeedToReadForWrite),
			new PropertyDependency(InternalSchema.MapiIsAllDayEvent, PropertyDependencyType.NeedToReadForWrite),
			new PropertyDependency(InternalSchema.AppointmentRecurring, PropertyDependencyType.NeedToReadForWrite),
			new PropertyDependency(InternalSchema.ReminderDueByInternal, PropertyDependencyType.NeedToReadForWrite)
		})
		{
			this.legalTrackingDependencies = (from x in base.Dependencies
			where x.Property != InternalSchema.ReminderNextTime && x.Property != InternalSchema.ReminderDueByInternal
			select x).ToArray<PropertyDependency>();
		}

		internal override PropertyDependency[] LegalTrackingDependencies
		{
			get
			{
				return this.legalTrackingDependencies;
			}
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			return StartTimeProperty.InternalTryGetDateTimeValue(propertyBag, this, InternalSchema.MapiStartTime, InternalSchema.MapiPRStartDate);
		}

		internal static object InternalTryGetDateTimeValue(PropertyBag.BasicPropertyStore propertyBag, StorePropertyDefinition property, GuidIdPropertyDefinition mapiUtcTimeProperty, PropertyTagPropertyDefinition mapiLegacyUtcTimeProperty)
		{
			ExDateTime? normalizedTime = StartTimeProperty.GetNormalizedTime(propertyBag, mapiUtcTimeProperty, mapiLegacyUtcTimeProperty);
			if (normalizedTime != null)
			{
				return normalizedTime.Value;
			}
			return new PropertyError(property, PropertyErrorCode.NotFound);
		}

		internal static ExDateTime? GetNormalizedTime(PropertyBag propertyBag, GuidIdPropertyDefinition utcTimeProperty, PropertyTagPropertyDefinition legacyUtcTimeProperty)
		{
			return StartTimeProperty.GetNormalizedTime((PropertyBag.BasicPropertyStore)propertyBag, utcTimeProperty, legacyUtcTimeProperty);
		}

		internal static ExDateTime? GetNormalizedTime(PropertyBag.BasicPropertyStore propertyBag, GuidIdPropertyDefinition utcTimeProperty, PropertyTagPropertyDefinition legacyUtcTimeProperty)
		{
			ExDateTime? valueAsNullable = propertyBag.GetValueAsNullable<ExDateTime>(utcTimeProperty);
			if (valueAsNullable == null)
			{
				if (legacyUtcTimeProperty != null)
				{
					valueAsNullable = propertyBag.GetValueAsNullable<ExDateTime>(legacyUtcTimeProperty);
				}
				if (valueAsNullable == null)
				{
					return null;
				}
			}
			ExDateTime exDateTime = ExTimeZone.UtcTimeZone.ConvertDateTime(valueAsNullable.Value);
			byte[] valueOrDefault = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.TimeZoneDefinitionStart);
			ExTimeZone legacyTimeZone;
			if (O12TimeZoneFormatter.TryParseTimeZoneBlob(valueOrDefault, string.Empty, out legacyTimeZone))
			{
				exDateTime = TimeZoneHelper.NormalizeUtcTime(exDateTime, legacyTimeZone);
			}
			exDateTime = StartTimeProperty.DynamicAdjustForAllDayEvent(propertyBag, exDateTime, utcTimeProperty == InternalSchema.MapiEndTime);
			return new ExDateTime?(propertyBag.TimeZone.ConvertDateTime(exDateTime));
		}

		private static ExDateTime DynamicAdjustForAllDayEvent(PropertyBag.BasicPropertyStore propertyBag, ExDateTime originalResult, bool isEndTime)
		{
			CalendarItemBase calendarItemBase = propertyBag.Context.StoreObject as CalendarItemBase;
			bool flag = false;
			if (calendarItemBase != null && calendarItemBase.IsAllDayEventCache != null)
			{
				flag = calendarItemBase.IsAllDayEventCache.Value;
			}
			if (!flag || propertyBag.TimeZone == null)
			{
				return originalResult;
			}
			if (isEndTime && originalResult != originalResult.Date)
			{
				return originalResult.Date.IncrementDays(1);
			}
			return originalResult.Date;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			StartTimeProperty.SetCalendarTime(propertyBag, InternalSchema.MapiStartTime, InternalSchema.MapiPRStartDate, InternalSchema.TimeZoneDefinitionStart, false, value);
		}

		internal static void SetCalendarTime(PropertyBag.BasicPropertyStore propertyBag, GuidIdPropertyDefinition utcTimeProperty, PropertyTagPropertyDefinition legacyUtcTimeProperty, GuidIdPropertyDefinition timeZoneDefinition, bool isEndTime, object value)
		{
			StoreObject storeObject = propertyBag.Context.StoreObject;
			CalendarItemBase calendarItemBase = storeObject as CalendarItemBase;
			if (calendarItemBase != null && calendarItemBase.PropertyBag.ExTimeZone != null && calendarItemBase.IsAllDayEventCache == null)
			{
				object obj = IsAllDayEventProperty.CalculateIsAllDayEvent(propertyBag);
				if (obj is bool)
				{
					calendarItemBase.IsAllDayEventCache = new bool?((bool)obj);
				}
			}
			propertyBag.SetValueWithFixup(utcTimeProperty, value);
			propertyBag.SetValueWithFixup(legacyUtcTimeProperty, value);
			ExTimeZone timeZone = propertyBag.TimeZone;
			if (value is ExDateTime && ((ExDateTime)value).TimeZone != null && ((ExDateTime)value).TimeZone != ExTimeZone.UnspecifiedTimeZone)
			{
				timeZone = ((ExDateTime)value).TimeZone;
			}
			if (timeZone == ExTimeZone.UtcTimeZone)
			{
				if (!(storeObject is CalendarItemOccurrence))
				{
					propertyBag.Delete(timeZoneDefinition);
				}
			}
			else if (value is ExDateTime && timeZone != ExTimeZone.UnspecifiedTimeZone)
			{
				if (timeZoneDefinition == InternalSchema.TimeZoneDefinitionStart)
				{
					byte[] timeZoneBlob = O12TimeZoneFormatter.GetTimeZoneBlob(timeZone);
					StartTimeZoneProperty.RecalculateNormalizedTimeProperty(propertyBag, InternalSchema.MapiEndTime, InternalSchema.MapiPREndDate, timeZone);
					StartTimeZoneProperty.RecalculateNormalizedTimeProperty(propertyBag, InternalSchema.ReminderNextTime, null, timeZone);
					StartTimeZoneProperty.SyncRecurringTimeZoneProperties(propertyBag, timeZone, timeZoneBlob);
					propertyBag.SetValueWithFixup(timeZoneDefinition, timeZoneBlob);
				}
				else
				{
					byte[] timeZoneBlob2 = O12TimeZoneFormatter.GetTimeZoneBlob(timeZone, (ExDateTime)value);
					propertyBag.SetValueWithFixup(timeZoneDefinition, timeZoneBlob2);
				}
			}
			if (!isEndTime)
			{
				propertyBag.SetValueWithFixup(InternalSchema.ReminderDueByInternal, value);
			}
			if (storeObject != null)
			{
				Reminder.Adjust(storeObject);
			}
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return base.SinglePropertySmartFilterToNativeFilter(filter, InternalSchema.MapiStartTime);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			return base.SinglePropertyNativeFilterToSmartFilter(filter, InternalSchema.MapiStartTime);
		}

		internal override void RegisterFilterTranslation()
		{
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(ComparisonFilter));
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(ExistsFilter));
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.All;
			}
		}

		protected override NativeStorePropertyDefinition GetSortProperty()
		{
			return InternalSchema.MapiStartTime;
		}

		private readonly PropertyDependency[] legalTrackingDependencies;
	}
}
