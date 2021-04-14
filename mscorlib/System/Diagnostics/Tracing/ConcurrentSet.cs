using System;
using System.Threading;

namespace System.Diagnostics.Tracing
{
	internal struct ConcurrentSet<KeyType, ItemType> where ItemType : ConcurrentSetItem<KeyType, ItemType>
	{
		public ItemType TryGet(KeyType key)
		{
			ItemType[] array = this.items;
			ItemType itemType;
			if (array != null)
			{
				int num = 0;
				int num2 = array.Length;
				do
				{
					int num3 = (num + num2) / 2;
					itemType = array[num3];
					int num4 = itemType.Compare(key);
					if (num4 == 0)
					{
						return itemType;
					}
					if (num4 < 0)
					{
						num = num3 + 1;
					}
					else
					{
						num2 = num3;
					}
				}
				while (num != num2);
			}
			itemType = default(ItemType);
			return itemType;
		}

		public ItemType GetOrAdd(ItemType newItem)
		{
			ItemType[] array = this.items;
			ItemType itemType;
			for (;;)
			{
				ItemType[] array2;
				if (array == null)
				{
					array2 = new ItemType[]
					{
						newItem
					};
				}
				else
				{
					int num = 0;
					int num2 = array.Length;
					do
					{
						int num3 = (num + num2) / 2;
						itemType = array[num3];
						int num4 = itemType.Compare(newItem);
						if (num4 == 0)
						{
							return itemType;
						}
						if (num4 < 0)
						{
							num = num3 + 1;
						}
						else
						{
							num2 = num3;
						}
					}
					while (num != num2);
					int num5 = array.Length;
					array2 = new ItemType[num5 + 1];
					Array.Copy(array, 0, array2, 0, num);
					array2[num] = newItem;
					Array.Copy(array, num, array2, num + 1, num5 - num);
				}
				array2 = Interlocked.CompareExchange<ItemType[]>(ref this.items, array2, array);
				if (array == array2)
				{
					break;
				}
				array = array2;
			}
			itemType = newItem;
			return itemType;
		}

		private ItemType[] items;
	}
}
