using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class EndTimeProperty : SmartPropertyDefinition
	{
		internal EndTimeProperty() : base("EndTime", typeof(ExDateTime), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.MapiEndTime, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.MapiPREndDate, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.TimeZoneDefinitionStart, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.TimeZoneDefinitionEnd, PropertyDependencyType.NeedToReadForWrite),
			new PropertyDependency(InternalSchema.MapiStartTime, PropertyDependencyType.NeedToReadForWrite),
			new PropertyDependency(InternalSchema.MapiPRStartDate, PropertyDependencyType.NeedToReadForWrite),
			new PropertyDependency(InternalSchema.MapiIsAllDayEvent, PropertyDependencyType.NeedToReadForWrite)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			return StartTimeProperty.InternalTryGetDateTimeValue(propertyBag, this, InternalSchema.MapiEndTime, InternalSchema.MapiPREndDate);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			StartTimeProperty.SetCalendarTime(propertyBag, InternalSchema.MapiEndTime, InternalSchema.MapiPREndDate, InternalSchema.TimeZoneDefinitionEnd, true, value);
			EndTimeProperty.DenormalizeTimeProperty(propertyBag, (ExDateTime)propertyBag.GetValue(InternalSchema.MapiEndTime), InternalSchema.MapiEndTime, InternalSchema.MapiPREndDate);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return base.SinglePropertySmartFilterToNativeFilter(filter, InternalSchema.MapiEndTime);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			return base.SinglePropertyNativeFilterToSmartFilter(filter, InternalSchema.MapiEndTime);
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
			return InternalSchema.MapiEndTime;
		}

		private static void DenormalizeTimeProperty(PropertyBag.BasicPropertyStore propertyBag, ExDateTime newTime, GuidIdPropertyDefinition utcTimeProperty, PropertyTagPropertyDefinition legacyUtcTimeProperty)
		{
			byte[] valueOrDefault = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.TimeZoneDefinitionStart);
			ExTimeZone legacyTimeZone;
			if (O12TimeZoneFormatter.TryParseTimeZoneBlob(valueOrDefault, string.Empty, out legacyTimeZone))
			{
				ExDateTime exDateTime = TimeZoneHelper.DeNormalizeToUtcTime(newTime, legacyTimeZone);
				propertyBag.SetValueWithFixup(utcTimeProperty, exDateTime);
				if (legacyUtcTimeProperty != null)
				{
					propertyBag.SetValueWithFixup(legacyUtcTimeProperty, exDateTime);
				}
			}
		}

		internal static void DenormalizeTimeProperty(PropertyBag propertyBag, ExDateTime newTime, GuidIdPropertyDefinition utcTimeProperty, PropertyTagPropertyDefinition legacyUtcTimeProperty)
		{
			byte[] valueOrDefault = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.TimeZoneDefinitionStart);
			ExTimeZone legacyTimeZone;
			if (O12TimeZoneFormatter.TryParseTimeZoneBlob(valueOrDefault, string.Empty, out legacyTimeZone))
			{
				ExDateTime exDateTime = TimeZoneHelper.DeNormalizeToUtcTime(newTime, legacyTimeZone);
				propertyBag.SetProperty(utcTimeProperty, exDateTime);
				if (legacyUtcTimeProperty != null)
				{
					propertyBag.SetProperty(legacyUtcTimeProperty, exDateTime);
				}
			}
		}
	}
}
