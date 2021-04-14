using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class IsTaskRecurringProperty : SmartPropertyDefinition
	{
		internal IsTaskRecurringProperty() : base("IsTaskRecurring", typeof(bool), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.TaskRecurrence, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.IsOneOff, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			bool valueOrDefault = propertyBag.GetValueOrDefault<bool>(InternalSchema.IsOneOff);
			if (valueOrDefault)
			{
				return false;
			}
			byte[] valueOrDefault2 = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.TaskRecurrence);
			return valueOrDefault2 != null;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			propertyBag.SetValueWithFixup(InternalSchema.MapiIsTaskRecurring, (bool)value);
		}

		protected override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			propertyBag.Delete(InternalSchema.MapiIsTaskRecurring);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return base.SinglePropertySmartFilterToNativeFilter(filter, InternalSchema.MapiIsTaskRecurring);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			return base.SinglePropertyNativeFilterToSmartFilter(filter, InternalSchema.MapiIsTaskRecurring);
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
			return InternalSchema.MapiIsTaskRecurring;
		}
	}
}
