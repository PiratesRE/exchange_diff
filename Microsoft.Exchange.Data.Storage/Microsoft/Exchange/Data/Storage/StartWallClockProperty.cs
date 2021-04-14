using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class StartWallClockProperty : SmartPropertyDefinition
	{
		internal StartWallClockProperty() : base("StartWallClock", typeof(ExDateTime), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.MapiStartTime, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.MapiPRStartDate, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.TimeZoneDefinitionStart, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.TimeZoneDefinitionEnd, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.TimeZoneDefinitionRecurring, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			ExDateTime? wallClockTime = StartWallClockProperty.GetWallClockTime(propertyBag, InternalSchema.MapiStartTime, InternalSchema.MapiPRStartDate, new NativeStorePropertyDefinition[]
			{
				InternalSchema.TimeZoneDefinitionStart,
				InternalSchema.TimeZoneDefinitionRecurring,
				InternalSchema.TimeZoneDefinitionEnd
			});
			if (wallClockTime == null)
			{
				return new PropertyError(this, PropertyErrorCode.NotFound);
			}
			return wallClockTime.Value;
		}

		internal static ExDateTime? GetWallClockTime(PropertyBag.BasicPropertyStore propertyBag, GuidIdPropertyDefinition utcTimeProperty, PropertyTagPropertyDefinition legacyUtcTimeProperty, NativeStorePropertyDefinition[] timeZoneBlobPropertyDefinitions)
		{
			ExDateTime? normalizedTime = StartTimeProperty.GetNormalizedTime(propertyBag, utcTimeProperty, legacyUtcTimeProperty);
			if (normalizedTime == null)
			{
				return null;
			}
			byte[] array = null;
			foreach (NativeStorePropertyDefinition propertyDefinition in timeZoneBlobPropertyDefinitions)
			{
				array = propertyBag.GetValueOrDefault<byte[]>(propertyDefinition);
				if (array != null)
				{
					break;
				}
			}
			if (array == null)
			{
				ExTraceGlobals.StorageTracer.TraceWarning(0L, "Could not determine suitable time zone");
			}
			ExTimeZone exTimeZone = StartWallClockProperty.FindBestMatchingTimeZone(array);
			if (exTimeZone != null)
			{
				return new ExDateTime?(exTimeZone.ConvertDateTime(normalizedTime.Value));
			}
			return new ExDateTime?(normalizedTime.Value);
		}

		internal static ExTimeZone FindBestMatchingTimeZone(byte[] timeZoneBlob)
		{
			ExTimeZone exTimeZone = null;
			if (O12TimeZoneFormatter.TryParseTimeZoneBlob(timeZoneBlob, string.Empty, out exTimeZone) && exTimeZone.IsCustomTimeZone)
			{
				exTimeZone = TimeZoneHelper.PromoteCustomizedTimeZone(exTimeZone);
			}
			return exTimeZone;
		}
	}
}
