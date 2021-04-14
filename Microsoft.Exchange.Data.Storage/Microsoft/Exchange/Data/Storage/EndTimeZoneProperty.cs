using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class EndTimeZoneProperty : SmartPropertyDefinition
	{
		internal EndTimeZoneProperty() : base("CreationTimeZone", typeof(ExTimeZone), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.MapiEndTime, PropertyDependencyType.NeedToReadForWrite),
			new PropertyDependency(InternalSchema.TimeZoneDefinitionEnd, PropertyDependencyType.AllRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			return StartTimeZoneProperty.GetExTimeZoneFromLegacyBlob(propertyBag, InternalSchema.TimeZoneDefinitionEnd);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			ExTimeZone exTimeZone = value as ExTimeZone;
			if (exTimeZone == ExTimeZone.UnspecifiedTimeZone)
			{
				throw new InvalidOperationException("unspecified time zone is not allowed to set");
			}
			if (exTimeZone == ExTimeZone.UtcTimeZone)
			{
				if (!(propertyBag.Context.StoreObject is CalendarItemOccurrence))
				{
					propertyBag.Delete(InternalSchema.TimeZoneDefinitionStart);
					return;
				}
			}
			else
			{
				ExDateTime valueOrDefault = propertyBag.GetValueOrDefault<ExDateTime>(InternalSchema.MapiEndTime, ExDateTime.UtcNow);
				propertyBag.SetValueWithFixup(InternalSchema.TimeZoneDefinitionEnd, (exTimeZone != null) ? O12TimeZoneFormatter.GetTimeZoneBlob(exTimeZone, valueOrDefault) : null);
			}
		}
	}
}
