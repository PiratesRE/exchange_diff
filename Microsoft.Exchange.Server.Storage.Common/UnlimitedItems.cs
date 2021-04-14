using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class UnlimitedItems : Unlimited<long>
	{
		public static bool TryParse(string input, out UnlimitedItems result)
		{
			long value;
			if (!long.TryParse(input, out value))
			{
				result = UnlimitedItems.UnlimitedValue;
				return false;
			}
			result = new UnlimitedItems(value);
			return true;
		}

		public UnlimitedItems()
		{
		}

		public UnlimitedItems(long value) : base(value)
		{
		}

		public static UnlimitedItems Zero = new UnlimitedItems(0L);

		public new static UnlimitedItems UnlimitedValue = new UnlimitedItems();
	}
}
