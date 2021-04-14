using System;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class PropertyBagHelpers
	{
		public static bool TestPropertyFlags(Context context, ISimpleReadOnlyPropertyBag bag, StorePropTag propTag, short mask, short desiredValue)
		{
			object propertyValue = bag.GetPropertyValue(context, propTag);
			short num;
			if (propertyValue == null)
			{
				num = 0;
			}
			else
			{
				num = (short)propertyValue;
			}
			return (num & mask) == desiredValue;
		}

		public static bool TestPropertyFlags(Context context, ISimpleReadOnlyPropertyBag bag, StorePropTag propTag, int mask, int desiredValue)
		{
			object propertyValue = bag.GetPropertyValue(context, propTag);
			int num;
			if (propertyValue == null)
			{
				num = 0;
			}
			else
			{
				num = (int)propertyValue;
			}
			return (num & mask) == desiredValue;
		}

		public static bool SetPropertyFlags(Context context, ISimplePropertyBag bag, StorePropTag propTag, object value, short flags)
		{
			if (value != null && value is bool && (bool)value)
			{
				return PropertyBagHelpers.AdjustPropertyFlags(context, bag, propTag, flags, 0);
			}
			return PropertyBagHelpers.AdjustPropertyFlags(context, bag, propTag, 0, flags);
		}

		public static bool SetPropertyFlags(Context context, ISimplePropertyBag bag, StorePropTag propTag, object value, int flags)
		{
			if (value != null && value is bool && (bool)value)
			{
				return PropertyBagHelpers.AdjustPropertyFlags(context, bag, propTag, flags, 0);
			}
			return PropertyBagHelpers.AdjustPropertyFlags(context, bag, propTag, 0, flags);
		}

		public static bool AdjustPropertyFlags(Context context, ISimplePropertyBag bag, StorePropTag propTag, short flagsToSet, short flagsToClear)
		{
			object propertyValue = bag.GetPropertyValue(context, propTag);
			short num;
			if (propertyValue == null)
			{
				num = 0;
			}
			else
			{
				num = (short)propertyValue;
			}
			short num2 = (num | flagsToSet) & ~flagsToClear;
			if (num2 != num)
			{
				bag.SetProperty(context, propTag, num2);
				return true;
			}
			return false;
		}

		public static bool AdjustPropertyFlags(Context context, ISimplePropertyBag bag, StorePropTag propTag, int flagsToSet, int flagsToClear)
		{
			object propertyValue = bag.GetPropertyValue(context, propTag);
			int num;
			if (propertyValue == null)
			{
				num = 0;
			}
			else
			{
				num = (int)propertyValue;
			}
			int num2 = (num | flagsToSet) & ~flagsToClear;
			if (num2 != num)
			{
				bag.SetProperty(context, propTag, num2);
				return true;
			}
			return false;
		}
	}
}
