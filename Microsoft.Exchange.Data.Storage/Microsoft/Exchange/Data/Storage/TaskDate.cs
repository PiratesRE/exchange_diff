using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal abstract class TaskDate : SmartPropertyDefinition
	{
		internal TaskDate(string displayName, NativeStorePropertyDefinition firstProperty, NativeStorePropertyDefinition secondProperty) : base(displayName, typeof(ExDateTime), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(firstProperty, PropertyDependencyType.NeedForRead),
			new PropertyDependency(secondProperty, PropertyDependencyType.NeedForRead)
		})
		{
			this.first = firstProperty;
			this.second = secondProperty;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			ExDateTime exDateTime = (ExDateTime)value;
			propertyBag.SetValueWithFixup(this.first, exDateTime);
			if (propertyBag.TimeZone != null)
			{
				propertyBag.SetValueWithFixup(this.second, TaskDate.PersistentLocalTime(new ExDateTime?(exDateTime)));
			}
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object value = propertyBag.GetValue(this.first);
			if (value is ExDateTime)
			{
				return (ExDateTime)value;
			}
			return value;
		}

		protected override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			propertyBag.Delete(this.first);
			propertyBag.Delete(this.second);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return base.SinglePropertySmartFilterToNativeFilter(filter, this.first);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			return base.SinglePropertyNativeFilterToSmartFilter(filter, this.first);
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
			return this.first;
		}

		internal static ExDateTime? PersistentLocalTime(ExDateTime? time)
		{
			if (time == null)
			{
				return null;
			}
			ExDateTime exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, time.Value.LocalTime.Ticks);
			return new ExDateTime?(ExTimeZone.UtcTimeZone.ConvertDateTime(exDateTime));
		}

		private NativeStorePropertyDefinition first;

		private NativeStorePropertyDefinition second;
	}
}
