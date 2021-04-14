using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal sealed class FullComputedPropertyMapping : ComputedPropertyMapping
	{
		internal FullComputedPropertyMapping(StorePropTag propertyTag, Column column, Func<Context, ISimpleReadOnlyPropertyBag, object> valueGetter, StorePropTag[] dependentPropertyTags, PropertyMapping[] dependentPropertyMappings, Func<Context, ISimplePropertyBag, object, ErrorCode> valueSetter, StreamGetterDelegate readStreamGetter, StreamGetterDelegate writeStreamGetter, bool primary, bool reservedPropId, bool list) : base(propertyTag, column, valueGetter, dependentPropertyTags, dependentPropertyMappings, valueSetter, readStreamGetter, writeStreamGetter, true, primary, reservedPropId, list)
		{
		}

		public override bool CanBeSet
		{
			get
			{
				return true;
			}
		}

		public override object GetPropertyValue(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return base.ValueGetter(context, bag);
		}

		public override ErrorCode SetPropertyValue(Context context, ISimplePropertyBag bag, object value)
		{
			return base.ValueSetter(context, bag, value);
		}
	}
}
