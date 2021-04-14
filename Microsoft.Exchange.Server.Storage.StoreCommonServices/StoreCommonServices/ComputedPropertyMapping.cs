using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal class ComputedPropertyMapping : PropertyMapping
	{
		internal ComputedPropertyMapping(StorePropTag propertyTag, Column column, Func<Context, ISimpleReadOnlyPropertyBag, object> valueGetter, StorePropTag[] dependentPropertyTags, PropertyMapping[] dependentPropertyMappings, Func<Context, ISimplePropertyBag, object, ErrorCode> valueSetter, StreamGetterDelegate readStreamGetter, StreamGetterDelegate writeStreamGetter, bool canBeOverriden, bool primary, bool reservedPropId, bool list) : base(PropertyMappingKind.Compute, propertyTag, column, valueSetter, readStreamGetter, writeStreamGetter, primary, reservedPropId, list)
		{
			this.valueGetter = valueGetter;
			this.dependentPropertyTags = dependentPropertyTags;
			this.dependentPropertyMappings = dependentPropertyMappings;
			this.canBeOverriden = canBeOverriden;
		}

		public Func<Context, ISimpleReadOnlyPropertyBag, object> ValueGetter
		{
			get
			{
				return this.valueGetter;
			}
		}

		public bool CanBeOverridden
		{
			get
			{
				return this.canBeOverriden;
			}
		}

		public override bool CanBeSet
		{
			get
			{
				return base.ValueSetter != null || this.canBeOverriden;
			}
		}

		public StorePropTag[] DependentPropertyTags
		{
			get
			{
				return this.dependentPropertyTags;
			}
		}

		public PropertyMapping[] DependentPropertyMappings
		{
			get
			{
				return this.dependentPropertyMappings;
			}
		}

		public override object GetPropertyValue(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			object obj = null;
			if (this.CanBeOverridden)
			{
				obj = bag.GetBlobPropertyValue(context, base.PropertyTag);
			}
			if (obj == null)
			{
				obj = this.ValueGetter(context, bag);
			}
			return obj;
		}

		public override ErrorCode SetPropertyValue(Context context, ISimplePropertyBag bag, object value)
		{
			if (!this.CanBeSet)
			{
				return ErrorCode.CreateNoAccess((LID)35608U, base.PropertyTag.PropTag);
			}
			if (base.ValueSetter != null)
			{
				return base.ValueSetter(context, bag, value);
			}
			bag.SetBlobProperty(context, base.PropertyTag, value);
			return ErrorCode.NoError;
		}

		public override bool IsPropertyChanged(Context context, ISimplePropertyBagWithChangeTracking bag)
		{
			object propertyValue = this.GetPropertyValue(context, bag.OriginalBag);
			object propertyValue2 = this.GetPropertyValue(context, bag);
			return !ValueHelper.ValuesEqual(propertyValue, propertyValue2);
		}

		private readonly StorePropTag[] dependentPropertyTags;

		private readonly PropertyMapping[] dependentPropertyMappings;

		private readonly Func<Context, ISimpleReadOnlyPropertyBag, object> valueGetter;

		private readonly bool canBeOverriden;
	}
}
