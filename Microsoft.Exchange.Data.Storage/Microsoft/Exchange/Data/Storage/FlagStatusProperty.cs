using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class FlagStatusProperty : SmartPropertyDefinition
	{
		internal FlagStatusProperty() : base("FlagStatusProperty", typeof(int), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.ItemClass, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.MapiFlagStatus, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.ItemColor, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.TaskStatus, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
			if (!ObjectClass.IsMeetingRequest(valueOrDefault))
			{
				return propertyBag.GetValueOrDefault<FlagStatus>(InternalSchema.MapiFlagStatus);
			}
			int valueOrDefault2 = propertyBag.GetValueOrDefault<int>(InternalSchema.ItemColor);
			if (valueOrDefault2 > 0)
			{
				return FlagStatus.Flagged;
			}
			int valueOrDefault3 = propertyBag.GetValueOrDefault<int>(InternalSchema.TaskStatus);
			if (valueOrDefault3 == 2)
			{
				return FlagStatus.Complete;
			}
			return FlagStatus.NotFlagged;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("FlagStatusProperty");
			}
			EnumValidator.ThrowIfInvalid<FlagStatus>((FlagStatus)value);
			switch ((FlagStatus)value)
			{
			case FlagStatus.NotFlagged:
				FlagStatusProperty.ClearFlagStatus(propertyBag);
				return;
			case FlagStatus.Complete:
				FlagStatusProperty.CompleteFlagStatus(propertyBag);
				return;
			case FlagStatus.Flagged:
				FlagStatusProperty.SetFlagStatus(propertyBag);
				return;
			default:
				return;
			}
		}

		protected override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			propertyBag.Delete(InternalSchema.MapiFlagStatus);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter != null)
			{
				return new ComparisonFilter(comparisonFilter.ComparisonOperator, InternalSchema.MapiFlagStatus, (int)comparisonFilter.PropertyValue);
			}
			if (filter is ExistsFilter)
			{
				return new ExistsFilter(InternalSchema.MapiFlagStatus);
			}
			return base.SmartFilterToNativeFilter(filter);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			SinglePropertyFilter singlePropertyFilter = filter as SinglePropertyFilter;
			if (singlePropertyFilter != null && singlePropertyFilter.Property.Equals(InternalSchema.MapiFlagStatus))
			{
				ComparisonFilter comparisonFilter = filter as ComparisonFilter;
				if (comparisonFilter != null)
				{
					return new ComparisonFilter(comparisonFilter.ComparisonOperator, this, (int)comparisonFilter.PropertyValue);
				}
				if (filter is ExistsFilter)
				{
					return new ExistsFilter(this);
				}
			}
			return null;
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
			return InternalSchema.MapiFlagStatus;
		}

		private static void CompleteFlagStatus(PropertyBag.BasicPropertyStore propertyBag)
		{
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
			if (ObjectClass.IsMeetingMessage(valueOrDefault))
			{
				propertyBag.Delete(InternalSchema.MapiFlagStatus);
				propertyBag.Delete(InternalSchema.ItemColor);
				propertyBag.SetValueWithFixup(InternalSchema.TaskStatus, TaskStatus.Completed);
				return;
			}
			propertyBag.SetValueWithFixup(InternalSchema.MapiFlagStatus, FlagStatus.Complete);
			propertyBag.SetValueWithFixup(InternalSchema.TaskStatus, TaskStatus.Completed);
			propertyBag.Delete(InternalSchema.ItemColor);
		}

		private static void SetFlagStatus(PropertyBag.BasicPropertyStore propertyBag)
		{
			propertyBag.SetValueWithFixup(InternalSchema.MapiFlagStatus, FlagStatus.Flagged);
			propertyBag.SetValueWithFixup(InternalSchema.TaskStatus, TaskStatus.InProgress);
			propertyBag.SetValueWithFixup(InternalSchema.ItemColor, ItemColor.Red);
		}

		private static void ClearFlagStatus(PropertyBag.BasicPropertyStore propertyBag)
		{
			propertyBag.Delete(InternalSchema.MapiFlagStatus);
			propertyBag.Delete(InternalSchema.TaskStatus);
			propertyBag.Delete(InternalSchema.ItemColor);
		}
	}
}
