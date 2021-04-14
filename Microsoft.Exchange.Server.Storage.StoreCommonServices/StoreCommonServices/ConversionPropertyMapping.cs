using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal sealed class ConversionPropertyMapping : PropertyMapping
	{
		internal ConversionPropertyMapping(StorePropTag propertyTag, Column column, Func<object, object> conversionFunction, StorePropTag argumentPropertyTag, PropertyMapping argumentPropertyMapping, Func<Context, ISimplePropertyBag, object, ErrorCode> valueSetter, StreamGetterDelegate readStreamGetter, StreamGetterDelegate writeStreamGetter, bool primary, bool reservedPropId, bool list) : base(PropertyMappingKind.Convert, propertyTag, column, valueSetter, readStreamGetter, writeStreamGetter, primary, reservedPropId, list)
		{
			this.conversionFunction = conversionFunction;
			this.argumentPropertyTag = argumentPropertyTag;
			this.argumentPropertyMapping = argumentPropertyMapping;
		}

		public StorePropTag ArgumentPropertyTag
		{
			get
			{
				return this.argumentPropertyTag;
			}
		}

		public PropertyMapping ArgumentPropertyMapping
		{
			get
			{
				return this.argumentPropertyMapping;
			}
		}

		public Func<object, object> ConversionFunction
		{
			get
			{
				return this.conversionFunction;
			}
		}

		public override bool CanBeSet
		{
			get
			{
				return base.ValueSetter != null;
			}
		}

		public override object GetPropertyValue(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			object obj = this.ArgumentPropertyMapping.GetPropertyValue(context, bag);
			if (obj != null)
			{
				obj = this.ConversionFunction(obj);
			}
			return obj;
		}

		public override ErrorCode SetPropertyValue(Context context, ISimplePropertyBag bag, object value)
		{
			if (!this.CanBeSet)
			{
				return ErrorCode.CreateNoAccess((LID)47676U, base.PropertyTag.PropTag);
			}
			return base.ValueSetter(context, bag, value);
		}

		public override bool IsPropertyChanged(Context context, ISimplePropertyBagWithChangeTracking bag)
		{
			object propertyValue = this.GetPropertyValue(context, bag.OriginalBag);
			object propertyValue2 = this.GetPropertyValue(context, bag);
			return !ValueHelper.ValuesEqual(propertyValue, propertyValue2);
		}

		private readonly StorePropTag argumentPropertyTag;

		private readonly PropertyMapping argumentPropertyMapping;

		private readonly Func<object, object> conversionFunction;
	}
}
