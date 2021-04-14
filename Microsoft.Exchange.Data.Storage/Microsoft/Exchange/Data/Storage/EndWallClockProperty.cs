using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class EndWallClockProperty : SmartPropertyDefinition
	{
		internal EndWallClockProperty() : base("EndWallClock", typeof(ExDateTime), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.MapiEndTime, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.MapiPREndDate, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.TimeZoneDefinitionStart, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.TimeZoneDefinitionEnd, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.TimeZoneDefinitionRecurring, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			ExDateTime? wallClockTime = StartWallClockProperty.GetWallClockTime(propertyBag, InternalSchema.MapiEndTime, InternalSchema.MapiPREndDate, new NativeStorePropertyDefinition[]
			{
				InternalSchema.TimeZoneDefinitionEnd,
				InternalSchema.TimeZoneDefinitionStart,
				InternalSchema.TimeZoneDefinitionRecurring
			});
			if (wallClockTime == null)
			{
				return new PropertyError(this, PropertyErrorCode.NotFound);
			}
			return wallClockTime.Value;
		}
	}
}
