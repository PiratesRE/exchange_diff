using System;

namespace Microsoft.Exchange.Data.Transport
{
	public static class EnumUtils
	{
		public static string[] DeliveryPriorityEnumNames
		{
			get
			{
				return EnumUtils.deliveryPriorityEnumNames;
			}
		}

		public static DeliveryPriority[] DeliveryPriorityEnumValues
		{
			get
			{
				return EnumUtils.deliveryPriorityEnumValues;
			}
		}

		private static readonly string[] deliveryPriorityEnumNames = Enum.GetNames(typeof(DeliveryPriority));

		private static readonly DeliveryPriority[] deliveryPriorityEnumValues = (DeliveryPriority[])Enum.GetValues(typeof(DeliveryPriority));
	}
}
