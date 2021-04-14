using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal sealed class ConstantPropertyMapping : PropertyMapping
	{
		internal ConstantPropertyMapping(StorePropTag propertyTag, Column column, Func<Context, ISimplePropertyBag, object, ErrorCode> valueSetter, object propertyValue, bool primary, bool reservedPropId, bool list) : base(PropertyMappingKind.Constant, propertyTag, column, valueSetter, null, null, primary, reservedPropId, list)
		{
			this.propertyValue = propertyValue;
		}

		public override bool CanBeSet
		{
			get
			{
				return false;
			}
		}

		public override object GetPropertyValue(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return this.propertyValue;
		}

		public override ErrorCode SetPropertyValue(Context context, ISimplePropertyBag bag, object value)
		{
			if (base.ValueSetter == null)
			{
				return ErrorCode.CreateNoAccess((LID)37084U, base.PropertyTag.PropTag);
			}
			return base.ValueSetter(context, bag, value);
		}

		public override bool IsPropertyChanged(Context context, ISimplePropertyBagWithChangeTracking bag)
		{
			if (!this.CanBeSet)
			{
				return false;
			}
			object x = this.GetPropertyValue(context, bag.OriginalBag);
			object y = this.GetPropertyValue(context, bag);
			return !ValueHelper.ValuesEqual(x, y);
		}

		private readonly object propertyValue;
	}
}
