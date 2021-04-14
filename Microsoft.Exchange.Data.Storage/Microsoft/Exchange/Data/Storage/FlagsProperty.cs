using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class FlagsProperty : SmartPropertyDefinition
	{
		private FlagsProperty(string displayName, NativeStorePropertyDefinition nativeProperty, int flag, PropertyDefinitionConstraint[] constraints, params PropertyDependency[] dependencies) : base(displayName, typeof(bool), PropertyFlags.None, constraints, dependencies)
		{
			this.flag = flag;
			this.nativeProperty = nativeProperty;
		}

		internal FlagsProperty(string displayName, NativeStorePropertyDefinition nativeProperty, int flag, PropertyDefinitionConstraint[] constraints) : this(displayName, nativeProperty, flag, constraints, new PropertyDependency[]
		{
			new PropertyDependency(nativeProperty, PropertyDependencyType.AllRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object value = propertyBag.GetValue(this.nativeProperty);
			PropertyError propertyError = value as PropertyError;
			if (propertyError != null)
			{
				return propertyError;
			}
			return BoxedConstants.GetBool(((int)value & this.flag) == this.flag);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			if (!(value is bool))
			{
				string message = ServerStrings.ExInvalidValueForFlagsCalculatedProperty(this.flag);
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), message);
				throw new ArgumentException(message);
			}
			object value2 = propertyBag.GetValue(this.nativeProperty);
			PropertyError propertyError = value2 as PropertyError;
			int num;
			if (propertyError == null)
			{
				num = (int)value2;
			}
			else
			{
				if (propertyError.PropertyErrorCode != PropertyErrorCode.NotFound)
				{
					throw PropertyError.ToException(new PropertyError[]
					{
						propertyError
					});
				}
				num = 0;
			}
			if ((bool)value)
			{
				propertyBag.SetValueWithFixup(this.nativeProperty, num | this.flag);
				return;
			}
			propertyBag.SetValueWithFixup(this.nativeProperty, num & ~this.flag);
		}

		protected override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			this.InternalSetValue(propertyBag, false);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			bool isNonZero = true;
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter != null)
			{
				if ((comparisonFilter.ComparisonOperator == ComparisonOperator.Equal && !(bool)comparisonFilter.PropertyValue) || (comparisonFilter.ComparisonOperator == ComparisonOperator.NotEqual && (bool)comparisonFilter.PropertyValue))
				{
					isNonZero = false;
				}
				else if (comparisonFilter.ComparisonOperator != ComparisonOperator.Equal && comparisonFilter.ComparisonOperator != ComparisonOperator.NotEqual)
				{
					throw base.CreateInvalidFilterConversionException(filter);
				}
				return new BitMaskFilter(this.nativeProperty, (ulong)this.flag, isNonZero);
			}
			if (filter is ExistsFilter)
			{
				return new ExistsFilter(this.nativeProperty);
			}
			return base.SmartFilterToNativeFilter(filter);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			SinglePropertyFilter singlePropertyFilter = filter as SinglePropertyFilter;
			if (singlePropertyFilter != null && singlePropertyFilter.Property.Equals(this.nativeProperty))
			{
				BitMaskFilter bitMaskFilter = filter as BitMaskFilter;
				if (bitMaskFilter != null && bitMaskFilter.Mask == (ulong)this.flag)
				{
					return new ComparisonFilter(ComparisonOperator.Equal, this, bitMaskFilter.IsNonZero);
				}
			}
			return null;
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.CanQuery;
			}
		}

		internal override void RegisterFilterTranslation()
		{
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(BitMaskFilter));
		}

		internal int Flag
		{
			get
			{
				return this.flag;
			}
		}

		internal NativeStorePropertyDefinition NativeProperty
		{
			get
			{
				return this.nativeProperty;
			}
		}

		private readonly NativeStorePropertyDefinition nativeProperty;

		private readonly int flag;
	}
}
