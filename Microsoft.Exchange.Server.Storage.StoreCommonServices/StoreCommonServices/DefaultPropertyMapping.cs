using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal sealed class DefaultPropertyMapping : PropertyMapping
	{
		internal DefaultPropertyMapping(StorePropTag propertyTag, Column column, Func<Context, ISimplePropertyBag, object, ErrorCode> valueSetter, StreamGetterDelegate readStreamGetter, StreamGetterDelegate writeStreamGetter, bool primary, bool reservedPropId, bool list, bool tailSet) : base(PropertyMappingKind.Default, propertyTag, column, valueSetter, readStreamGetter, writeStreamGetter, primary, reservedPropId, list)
		{
			this.tailSet = tailSet;
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
			return bag.GetBlobPropertyValue(context, base.PropertyTag);
		}

		public override ErrorCode SetPropertyValue(Context context, ISimplePropertyBag bag, object value)
		{
			if (base.ValueSetter != null)
			{
				ErrorCode errorCode = base.ValueSetter(context, bag, value);
				if (errorCode != ErrorCode.NoError || !this.tailSet)
				{
					return errorCode;
				}
			}
			bag.SetBlobProperty(context, base.PropertyTag, value);
			return ErrorCode.NoError;
		}

		public override bool IsPropertyChanged(Context context, ISimplePropertyBagWithChangeTracking bag)
		{
			return bag.IsBlobPropertyChanged(context, base.PropertyTag);
		}

		private readonly bool tailSet;
	}
}
