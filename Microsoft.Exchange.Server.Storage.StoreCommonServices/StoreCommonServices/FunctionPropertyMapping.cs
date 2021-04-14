using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal sealed class FunctionPropertyMapping : PropertyMapping
	{
		internal FunctionPropertyMapping(StorePropTag propertyTag, Column column, Func<Context, ISimplePropertyBag, object, ErrorCode> valueSetter, Func<object[], object> function, PropertyMapping[] argumentPropertyMappings, bool primary, bool reservedPropId, bool list) : base(PropertyMappingKind.Function, propertyTag, column, valueSetter, null, null, primary, reservedPropId, list)
		{
			this.function = function;
			this.argumentPropertyMappings = argumentPropertyMappings;
		}

		public PropertyMapping[] ArgumentPropertyMappings
		{
			get
			{
				return this.argumentPropertyMappings;
			}
		}

		public Func<object[], object> Function
		{
			get
			{
				return this.function;
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
			object[] array = new object[this.argumentPropertyMappings.Length];
			for (int i = 0; i < this.argumentPropertyMappings.Length; i++)
			{
				array[i] = this.argumentPropertyMappings[i].GetPropertyValue(context, bag);
			}
			return this.function(array);
		}

		public override ErrorCode SetPropertyValue(Context context, ISimplePropertyBag bag, object value)
		{
			if (base.ValueSetter == null)
			{
				return ErrorCode.CreateNoAccess((LID)53468U, base.PropertyTag.PropTag);
			}
			return base.ValueSetter(context, bag, value);
		}

		public override bool IsPropertyChanged(Context context, ISimplePropertyBagWithChangeTracking bag)
		{
			object propertyValue = this.GetPropertyValue(context, bag.OriginalBag);
			object propertyValue2 = this.GetPropertyValue(context, bag);
			return !ValueHelper.ValuesEqual(propertyValue, propertyValue2);
		}

		private readonly PropertyMapping[] argumentPropertyMappings;

		private readonly Func<object[], object> function;
	}
}
