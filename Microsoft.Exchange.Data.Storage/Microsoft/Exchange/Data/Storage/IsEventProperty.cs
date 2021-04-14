using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class IsEventProperty : SmartPropertyDefinition
	{
		internal IsEventProperty() : base("IsEvent", typeof(bool), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.MapiStartTime, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.MapiEndTime, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			bool flag = false;
			ExDateTime? valueAsNullable = propertyBag.GetValueAsNullable<ExDateTime>(InternalSchema.MapiStartTime);
			ExDateTime? valueAsNullable2 = propertyBag.GetValueAsNullable<ExDateTime>(InternalSchema.MapiEndTime);
			if (valueAsNullable != null && valueAsNullable2 != null && valueAsNullable2.Value - valueAsNullable.Value >= IsEventProperty.Hours24)
			{
				flag = true;
			}
			return flag;
		}

		private static readonly TimeSpan Hours24 = TimeSpan.FromHours(24.0);
	}
}
